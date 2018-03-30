using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Carbon.Extensions;
using Carbon.Jose;
using Carbon.Json;

namespace Carbon.Acme
{
    public class AcmeClient
    {
        public static readonly string Version = "acme-10";

        private readonly HttpClient httpClient = new HttpClient {
            DefaultRequestHeaders = {
                {  "User-Agent", "Carbon.Acme/2" }
            }
        };

        private readonly string _directoryUrl;
        private readonly RSA _privateKey;

        private string _accountUrl;
        private Directory _directory;

        private readonly ConcurrentQueue<Nonce> nonces = new ConcurrentQueue<Nonce>();

        public AcmeClient(RSA privateKey, string accountUrl = null, string directoryUrl = "https://acme-v02.api.letsencrypt.org/directory")
        {
            _accountUrl   = accountUrl;
            _directoryUrl = directoryUrl ?? throw new ArgumentNullException(nameof(directoryUrl));
            _privateKey   = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
        }

        #region Accounts

        public async Task<string> GetAccountUrlAsync()
        {
            await InitializeDirectoryAsync();

            var message = await GetSignedMessageAsync(
                url: _directory.NewAccountUrl,
                payload: new JsonObject {
                    { "onlyReturnExisting", true }
                }
            );

            var (_, location, _) = await PostAsync(_directory.NewAccountUrl, message);

            // -> 201 (OK) [Account]

            return location;
        }

        // 7.3. Account Creation | https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.3
        public async Task<string> CreateAccountAsync(CreateAccountRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            await InitializeDirectoryAsync();

            var message = await GetSignedMessageAsync(
                url     : _directory.NewAccountUrl, 
                payload : JsonObject.FromObject(request)
            );

            var (_, location, _) = await PostAsync(_directory.NewAccountUrl, message);
            
            this._accountUrl = location;

            // -> 201 (OK) [Account]

            return location;
        }

        // https://tools.ietf.org/html/draft-ietf-acme-acme-09#section-7.3.2
        public async Task<Account> UpdateAccountAsync(UpdateAccountRequest request)
        {
            if (!IsInitialized) await InitializeAsync();

            var message = await GetSignedMessageAsync(
                url     : _accountUrl, 
                payload : JsonObject.FromObject(request)
            );

            var (_, _, responseText) = await PostAsync(_accountUrl, message); 
            
            // -> 200 (OK) [Account]

            return JsonObject.Parse(responseText).As<Account>();
        }

        // https://tools.ietf.org/html/draft-ietf-acme-acme-09#section-7.3.7
        public async Task<Account> DeactivateAccountAsync()
        {
            if (!IsInitialized) await InitializeAsync();

            var message = await GetSignedMessageAsync(
                url     : _accountUrl, 
                payload : JsonObject.FromObject(new DeactivateAccountRequest(_accountUrl))
            );

            var (_, _, responseText) = await PostAsync(_accountUrl, message); 
            
            // -> 200 (OK) [Account]

            return JsonObject.Parse(responseText).As<Account>();
        }

        /*
        public async Task ChangeKeyAsync(ChangeKeyRequest request)
        {
            var responseText = await SendAsync(directory.KeyChangeUrl);
        }
        */


        // TODO: lookup URL

        #endregion

        #region Authorization & Challenges

        // NOTE:

        // Pre-authorization is an optional feature that Let's Encrypt has no plans to implement.
        // V2 clients should use order based issuance without pre-authorization.

        public async Task<Authorization> GetAuthorizationAsync(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            return (await _GetAuthorizationAsync(url)).authorization;
        }

        private async Task<(Authorization authorization, TimeSpan retryAfter)> _GetAuthorizationAsync(string url)
        {
            using (var response = await httpClient.GetAsync(url))
            {
                var responseText = await response.Content.ReadAsStringAsync();

                var result = JsonObject.Parse(responseText).As<Authorization>();

                var retryAfter = TimeSpan.FromSeconds(1);

                if (response.Headers.TryGetValues("Retry-After", out var retryAfterHeader)
                    && int.TryParse(retryAfterHeader.First(), out int retryAfterSeconds))
                {
                    retryAfter = TimeSpan.FromSeconds(retryAfterSeconds);

                    // Cap @ 5 seconds
                    if (retryAfter > TimeSpan.FromSeconds(5))
                    {
                        retryAfter = TimeSpan.FromSeconds(5);
                    }
                }

                return (result, retryAfter);
            }
        }

        public async Task<Authorization> WaitForAuthorizationAsync(string url, TimeSpan timeout)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (timeout > TimeSpan.FromMinutes(5))
            {
                throw new ArgumentException("Must be 5 minutes or less", nameof(timeout));
            }

            var sw = Stopwatch.StartNew();

            Authorization result;
            TimeSpan retryAfter;

            (result, retryAfter) = await _GetAuthorizationAsync(url);

            int retryCount = 0;

            while (result.Status == AuthorizationStatus.Pending && retryCount < 5)
            {
                await Task.Delay(retryAfter);

                (result, retryAfter) = await _GetAuthorizationAsync(url);

                if (sw.Elapsed >= timeout) break;

                retryCount++;
            }

            return result;
        }

        public async Task<Authorization> DeactivateAuthorizationAsync(DeactivateAuthorizationRequest request)
        {
            if (!IsInitialized) await InitializeAsync();

            var message = await GetSignedMessageAsync(
                url     : request.Url, 
                payload : JsonObject.FromObject(request)
            );

            var (_, _, responseText) = await PostAsync(request.Url, message); 
            
            // -> 200 (OK) [Authorization]

            return JsonObject.Parse(responseText).As<Authorization>();
        }

        public async Task<Challenge> GetChallengeAsync(string url)
        {
            var responseText = await httpClient.GetStringAsync(url);

            return JsonObject.Parse(responseText).As<Challenge>();
        }

        // 7.5.1.  Responding to Challenges
        // https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.5.1
        public async Task<Challenge> CompleteChallengeAsync(CompleteChallengeRequest request)
        {
            if (!IsInitialized) await InitializeAsync();

            var message = await GetSignedMessageAsync(
                url     : request.Url, 
                payload : JsonObject.FromObject(request)
            );

            var (_, _, responseText) = await PostAsync(request.Url, message);

            // -> 200 (OK) [Challenge]

            return JsonObject.Parse(responseText).As<Challenge>();
        }

        #endregion

        #region Orders

        public async Task<Order> GetOrderAsync(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var responseText = await httpClient.GetStringAsync(url);

            return JsonObject.Parse(responseText).As<Order>();
        }

        public async Task<OrderList> ListOrdersAsync(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            // Link: <https://example.com/acme/acct/1/orders?cursor=2>, rel="next"

            return await GetAsync<OrderList>(url.ToString());
        }

        // POST /acme/new-order
        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!IsInitialized) await InitializeAsync();

            var message = await GetSignedMessageAsync(
                url     : _directory.NewOrderUrl,
                payload : JsonObject.FromObject(request)
            );

            var (_, location, responseText) = await PostAsync(_directory.NewOrderUrl, message);

            // -> 201 (Created) [Order]

            var result = JsonObject.Parse(responseText).As<Order>();

            result.Url = location;

            return result;
        }

        //  POST /acme/order/{id}/finalize
        public async Task<Order> FinalizeOrderAsync(FinalizeOrderRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!IsInitialized) await InitializeAsync();

            var message = await GetSignedMessageAsync(
                url     : request.Url,
                payload : JsonObject.FromObject(request)
            );

            var (_, _, responseText) = await PostAsync(request.Url, message);

            // -> 200 (OK) [Order]

            return JsonObject.Parse(responseText).As<Order>();
        }
        
        public async Task<Order> WaitForCertificateAsync(string orderUrl, TimeSpan timeout)
        {
            if (orderUrl == null)
            {
                throw new ArgumentNullException(nameof(orderUrl));
            }

            if (timeout > TimeSpan.FromMinutes(5))
            {
                throw new ArgumentException("Must be 5 minutes or less", nameof(timeout));
            }

            var sw = Stopwatch.StartNew();

            Order result = await GetOrderAsync(orderUrl);

            var retryAfter = TimeSpan.FromSeconds(1);
            
            int retryCount = 0;
            
            while ((result.Status == OrderStatus.Pending || 
                    result.Status == OrderStatus.Processing ||
                    result.Status == OrderStatus.Ready) && retryCount < 5)
            {
                await Task.Delay(retryAfter);

                result = await GetOrderAsync(orderUrl);

                if (sw.Elapsed >= timeout) break;

                retryCount++;
            }

            return result;
        }

        #endregion        

        #region Certificates

        public async Task<string> DownloadCertificateChainAsync(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            if (!IsInitialized) await InitializeAsync();

            using (var response = await httpClient.GetAsync(url))
            {
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode + ":" + responseText);
                }

                // -> 200 | application/pem-certificate-chain
                
                return responseText;
            }
        }

        // 7.6 https://tools.ietf.org/html/draft-ietf-acme-acme-09#section-7.6
        public async Task RevokeCertificateAsync(RevokeCertificateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!IsInitialized) await InitializeAsync();

            var message = await GetSignedMessageAsync(
              url     : _directory.RevokeCertificateUrl,
              payload : JsonObject.FromObject(request)
            );

            var responseText = await PostAsync(_directory.RevokeCertificateUrl, message);
        }

        #endregion

        #region Key Authoriztion

        private string _thumbprint;

        private string GetSha256Thumbprint()
        {
            if (_thumbprint == null)
            {
                var key = Jwk.FromPublicKey(_privateKey.ExportParameters(false));

                string json = "{\"e\":\"" + key.Exponent + "\",\"kty\":\"RSA\",\"n\":\"" + key.Modulus + "\"}";

                using (var sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));

                    _thumbprint = Base64Url.Encode(hash);
                }
            }

            return _thumbprint;
        }

        public string GetKeyAuthorization(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            return token + "." + GetSha256Thumbprint();
        }

        #endregion

        #region Nonces

        // A nonce is included with every request to prevent replay.

        private async Task<Nonce> GetNonceAsync()
        {
            if (nonces.TryDequeue(out Nonce nonce))
            {
                return nonce;
            }

            // HEAD /acme/new-nonce HTTP/1.1 ->
            // Replay-Nonce: oFvnlFP1wIhRlYS2jTaXbA
            
            using (var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _directory.NewNonceUrl)))
            {
                string replayNonce = response.Headers.GetValues("Replay-Nonce").First();

                return new Nonce(replayNonce);
            }
        }

        #endregion

        public bool IsInitialized => _accountUrl != null && _directory != null;

        public async Task InitializeAsync()
        {
             await InitializeDirectoryAsync();
            
            if (_accountUrl == null)
            {
                this._accountUrl = await GetAccountUrlAsync();
            }
        }

        private async Task InitializeDirectoryAsync()
        {
            if (_directory == null)
            {
                var responseText = await httpClient.GetStringAsync(_directoryUrl);

                this._directory = JsonObject.Parse(responseText).As<Directory>();
            }
        }

        private async Task<T> GetAsync<T>(string url)
            where T: new()
        {
            var (_, _, responseText) = await SendAsync(
                request: new HttpRequestMessage(HttpMethod.Get, url)
            );

            return JsonObject.Parse(responseText).As<T>();
        }

        private async Task<(HttpStatusCode statusCode, string location, string responseText)> PostAsync(
            string url, 
            JwsEncodedMessage message)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new StringContent(
                    content   : JsonObject.FromObject(message).ToString(false),
                    encoding  : Encoding.UTF8,
                    mediaType : "application/jose+json"
                )
            };

            // Remove the charset
            request.Content.Headers.ContentType.CharSet = null;

            return await SendAsync(request);
        }

        private async Task<(HttpStatusCode statusCode, string location, string responseText)> SendAsync(HttpRequestMessage request)
        {
            using (var response = await httpClient.SendAsync(request))
            {
                // capture the replay nonce for the next request
                if (request.Method == HttpMethod.Post && 
                    response.Headers.TryGetValues("Replay-Nonce", out var replayNonce))
                {
                    // ensure the queue does not exceed 3 entries
                    if (nonces.Count > 3)
                    {
                        nonces.TryDequeue(out _);
                    }

                    nonces.Enqueue(new Nonce(string.Join(",", replayNonce)));
                }
                
                // Content-Type is null when only a Location is returned
                string contentType = response.Content.Headers.ContentType?.ToString();

                string responseText = await response.Content.ReadAsStringAsync();

                string location = null;

                if (response.Headers.TryGetValues("Location", out var locationHeader))
                {
                    location = string.Join(";", locationHeader);
                }

                if (contentType == "application/problem+json")
                {
                    var problem = JsonObject.Parse(responseText).As<Problem>();

                    throw new AcmeException(problem);
                }

                // TODO: error handling
                // retry if invalid nonce?

                return (response.StatusCode, location, responseText);
            }           
        }

        private async Task<JwsEncodedMessage> GetSignedMessageAsync(string url, JsonObject payload)
        {
            var nonce = await GetNonceAsync();

            var header = GetMessageHeader(url, nonce);

            return Jws.Sign(
                integrityProtectedHeader : header,
                payload                  : payload,
                privateKey               : _privateKey
            );
        }

        private JsonObject GetMessageHeader(string url, Nonce nonce)
        {
            var header = new JsonObject {
                { "alg",   JwtAlgorithms.RS256 },
                { "nonce", nonce.Value },
                { "url",   url }
            };

            if (_accountUrl != null)
            {
                header.Add("kid", _accountUrl);
            }
            else
            {
                var jwk = Jwk.FromPublicKey(_privateKey.ExportParameters(includePrivateParameters: false));

                header.Add("jwk", JsonObject.FromObject(jwk));
            }

            return header;
        }
    }
}

// LATEST : https://tools.ietf.org/html/draft-ietf-acme-acme-10
// LIVING : https://github.com/ietf-wg-acme/acme/blob/master/draft-ietf-acme-acme.md