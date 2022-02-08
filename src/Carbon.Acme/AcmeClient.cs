using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Carbon.Acme.Exceptions;
using Carbon.Extensions;
using Carbon.Jose;

namespace Carbon.Acme;

// --------------------------------------------------------
// Automatic Certificate Management Environment (ACME)
// RFC 8555 | https://tools.ietf.org/html/rfc8555
// --------------------------------------------------------

public class AcmeClient
{
    private readonly HttpClient httpClient = new () {
        DefaultRequestHeaders = {
            {  "User-Agent", "Carbon.Acme/2" }
        }
    };

    private readonly string _directoryUrl;
    private readonly RSA _privateKey;

    private string? _accountUrl;
    private Directory? _directory;

    private readonly ConcurrentQueue<Nonce> nonces = new ();

    public AcmeClient(RSA privateKey, string? accountUrl = null, string directoryUrl = "https://acme-v02.api.letsencrypt.org/directory")
    {
        ArgumentNullException.ThrowIfNull(privateKey);
        ArgumentNullException.ThrowIfNull(directoryUrl);

        _accountUrl = accountUrl;
        _directoryUrl = directoryUrl;
        _privateKey = privateKey;
    }

    #region Accounts

    public async Task<string> GetAccountUrlAsync()
    {
        await InitializeDirectoryAsync().ConfigureAwait(false);

        var message = await GetSignedMessageAsync(
            url     : _directory!.NewAccountUrl,
            payload : new {
                onlyReturnExisting = true
            }
        ).ConfigureAwait(false);

        (_, string? location, _) = await PostAsync(_directory.NewAccountUrl, message).ConfigureAwait(false);

        // -> 201 (OK) [Account]

        return location!;
    }

    // 7.3. Account Creation | https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.3
    public async Task<string> CreateAccountAsync(CreateAccountRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await InitializeDirectoryAsync().ConfigureAwait(false);

        var message = await GetSignedMessageAsync(
            url     : _directory!.NewAccountUrl,
            payload : request
        ).ConfigureAwait(false);

        (_, string? location, _) = await PostAsync(_directory.NewAccountUrl, message).ConfigureAwait(false);

        _accountUrl = location;

        // -> 201 (OK) [Account]

        return location!;
    }

    // https://tools.ietf.org/html/draft-ietf-acme-acme-09#section-7.3.2
    public async Task<Account> UpdateAccountAsync(UpdateAccountRequest request)
    {
        if (!IsInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        var message = await GetSignedMessageAsync(
            url     : _accountUrl!,
            payload : request
        ).ConfigureAwait(false);

        (_, _, string responseText) = await PostAsync(_accountUrl!, message).ConfigureAwait(false);

        // -> 200 (OK) [Account]

        return JsonSerializer.Deserialize<Account>(responseText)!;
    }

    // https://tools.ietf.org/html/draft-ietf-acme-acme-09#section-7.3.7
    public async Task<Account> DeactivateAccountAsync()
    {
        if (!IsInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        var message = await GetSignedMessageAsync(
            url     : _accountUrl!,
            payload : new DeactivateAccountRequest(_accountUrl!)
        ).ConfigureAwait(false);

        var (_, _, responseText) = await PostAsync(_accountUrl!, message).ConfigureAwait(false);

        // -> 200 (OK) [Account]

        return JsonSerializer.Deserialize<Account>(responseText)!;
    }

    /*
    public async Task ChangeKeyAsync(ChangeKeyRequest request)
    {
        var responseText = await SendAsync(directory.KeyChangeUrl);
    }
    */

    #endregion

    #region Authorization & Challenges

    // NOTE:
    // Pre-authorization is an optional feature that Let's Encrypt has no plans to implement.
    // V2 clients should use order based issuance without pre-authorization.

    public async Task<Authorization> GetAuthorizationAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return (await GetAuthorizationAsyncInternal(url).ConfigureAwait(false)).authorization;
    }

    private async Task<(Authorization authorization, TimeSpan retryAfter)> GetAuthorizationAsyncInternal(string url)
    {
        var result = await PostAsGetAsync<Authorization>(url).ConfigureAwait(false);

        var retryAfter = TimeSpan.FromSeconds(2);

        // Let's Encrypt does not support Retry-After

        /*
        if (response.Headers.TryGetValues("Retry-After", out var retryAfterHeader)
            && int.TryParse(retryAfterHeader.First(), out int retryAfterSeconds))
        {
            retryAfter = TimeSpan.FromSeconds(retryAfterSeconds);

            // cap @ 5 seconds
            if (retryAfter > TimeSpan.FromSeconds(5))
            {
                retryAfter = TimeSpan.FromSeconds(5);
            }
        }
        */

        return (result, retryAfter);
    }

    public async Task<Authorization> WaitForAuthorizationAsync(string url, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(url);

        if (timeout > TimeSpan.FromMinutes(5))
        {
            throw new ArgumentException("Must be 5 minutes or less", nameof(timeout));
        }

        var sw = Stopwatch.StartNew();

        Authorization result;
        TimeSpan retryAfter;

        (result, retryAfter) = await GetAuthorizationAsyncInternal(url).ConfigureAwait(false);

        int retryCount = 0;

        while (result.Status == AuthorizationStatus.Pending && retryCount < 5)
        {
            await Task.Delay(retryAfter).ConfigureAwait(false);

            (result, retryAfter) = await GetAuthorizationAsyncInternal(url).ConfigureAwait(false);

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
            payload : request
        ).ConfigureAwait(false);

        var (_, _, responseText) = await PostAsync(request.Url, message).ConfigureAwait(false);

        // -> 200 (OK) [Authorization]

        return JsonSerializer.Deserialize<Authorization>(responseText)!;
    }

    public async Task<Challenge> GetChallengeAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return await PostAsGetAsync<Challenge>(url).ConfigureAwait(false);
    }

    // 7.5.1. Responding to Challenges
    // https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.5.1
    public async Task<Challenge> CompleteChallengeAsync(CompleteChallengeRequest request)
    {
        if (!IsInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        // The client indicates to the server that it is ready for the challenge validation by
        // sending an empty JSON body ({}) carried in a POST request to the challenge URL

        JwsEncodedMessage message = await GetSignedMessageAsync(request.Url, new { } ).ConfigureAwait(false);

        (_, _, string responseText) = await PostAsync(request.Url, message).ConfigureAwait(false);

        // -> 200 (OK) [Challenge]

        return JsonSerializer.Deserialize<Challenge>(responseText)!;
    }

    #endregion

    #region Orders

    public async Task<Order> GetOrderAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return await PostAsGetAsync<Order>(url).ConfigureAwait(false);
    }

    public async Task<OrderList> ListOrdersAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        // Link: <https://example.com/acme/acct/1/orders?cursor=2>, rel="next"

        return await PostAsGetAsync<OrderList>(url).ConfigureAwait(false);
    }

    // POST /acme/new-order
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!IsInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        var message = await GetSignedMessageAsync(
            url     : _directory!.NewOrderUrl,
            payload : request
        ).ConfigureAwait(false);

        var (_, location, responseText) = await PostAsync(_directory.NewOrderUrl, message).ConfigureAwait(false);

        // -> 201 (Created) [Order]

        var result = JsonSerializer.Deserialize<Order>(responseText)!;

        result.Url = location!;

        return result;
    }

    //  POST /acme/order/{id}/finalize
    public async Task<Order> FinalizeOrderAsync(FinalizeOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!IsInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        var message = await GetSignedMessageAsync(
            url     : request.Url,
            payload : request
        ).ConfigureAwait(false);

        (_, _, string responseText) = await PostAsync(request.Url, message).ConfigureAwait(false);

        // -> 200 (OK) [Order]

        return JsonSerializer.Deserialize<Order>(responseText)!;
    }

    public async Task<Order> WaitForCertificateAsync(string orderUrl, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(orderUrl);

        if (timeout > TimeSpan.FromMinutes(5))
        {
            throw new ArgumentException("Must be 5 minutes or less", nameof(timeout));
        }

        var sw = Stopwatch.StartNew();

        Order result = await GetOrderAsync(orderUrl).ConfigureAwait(false);

        var retryAfter = TimeSpan.FromSeconds(1);

        int retryCount = 0;

        // Once the certificate is issued, the order enters the "valid" state.

        while (!result.IsValid && retryCount < 5)
        {
            await Task.Delay(retryAfter).ConfigureAwait(false);

            result = await GetOrderAsync(orderUrl).ConfigureAwait(false);

            if (sw.Elapsed >= timeout) break;

            retryCount++;
        }

        return result;
    }

    #endregion        

    #region Certificates

    public async Task<DownloadCertificateResult> DownloadCertificateChainAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        if (!IsInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        // TODO: [Accept] : application/pem-certificate-chain
        string body = await PostAsGetAsync(url).ConfigureAwait(false);

        // -> 200 
        // | Content-Type: application/pem-certificate-chain
        // | Link: <https://example.com/acme/some-directory>;rel="index"

        return new DownloadCertificateResult(
            contentType : "application/pem-certificate-chain",
            body        : body
        );
    }

    // 7.6 https://tools.ietf.org/html/draft-ietf-acme-acme-09#section-7.6
    public async Task RevokeCertificateAsync(RevokeCertificateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!IsInitialized)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        var message = await GetSignedMessageAsync(
            url     : _directory!.RevokeCertificateUrl,
            payload : request
        ).ConfigureAwait(false);

        await PostAsync(_directory.RevokeCertificateUrl, message).ConfigureAwait(false);
    }

    #endregion

    #region Key Authoriztion

    private string? _thumbprint;

    private string GetSha256Thumbprint()
    {
        if (_thumbprint is null)
        {
            _jwk ??= Jwk.FromRSAPublicParameters(_privateKey.ExportParameters(false));

            string e = Base64Url.Encode(_jwk.Exponent);
            string n = Base64Url.Encode(_jwk.Modulus);

            string json = "{\"e\":\"" + e + "\",\"kty\":\"RSA\",\"n\":\"" + n + "\"}";

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));

            _thumbprint = Base64Url.Encode(hash);
        }

        return _thumbprint;
    }

    public string GetKeyAuthorization(string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return token + "." + GetSha256Thumbprint();
    }

    // FOR DNS VALUE
    // Should still be quoted
    public string GetBase64UrlEncodedKeyAuthorizationSha256Digest(string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        // TXT _acme-challenge.{host} "{value}"

        string result = GetKeyAuthorization(token);

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(result));

        return Base64Url.Encode(hash);
    }

    #endregion

    #region Nonces

    // A nonce is included with every request to prevent replay.

    public async ValueTask<Nonce> GetNonceAsync()
    {
        if (_directory is null)
        {
            await InitializeDirectoryAsync().ConfigureAwait(false);
        }

        while (nonces.TryDequeue(out Nonce nonce))
        {
            // Ingore nonces older than 1 minute old
            if (nonce.Age > TimeSpan.FromMinutes(1))
            {
                continue;
            }

            return nonce;
        }

        // HEAD /acme/new-nonce HTTP/1.1 ->
        // Replay-Nonce: oFvnlFP1wIhRlYS2jTaXbA

        using HttpResponseMessage response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _directory!.NewNonceUrl)).ConfigureAwait(false);

        string replayNonce = response.Headers.GetValues("Replay-Nonce").First();

        return new Nonce(replayNonce, DateTime.UtcNow);
    }

    #endregion

    private Jwk? _jwk;

    internal Jwk Jwk => _jwk ??= Jwk.FromRSAPublicParameters(_privateKey.ExportParameters(includePrivateParameters: false));

    public bool IsInitialized => _accountUrl is not null && _directory is not null;

    public async Task InitializeAsync()
    {
        await InitializeDirectoryAsync().ConfigureAwait(false);

        if (_accountUrl is null)
        {
            _accountUrl = await GetAccountUrlAsync().ConfigureAwait(false);
        }
    }

    private async Task InitializeDirectoryAsync()
    {
        // NOTE: The server MUST allow GET requests for the directory and newNonce resources(see Section 7.1)

        if (_directory is null)
        {
            var directoryStream = await httpClient.GetStreamAsync(_directoryUrl).ConfigureAwait(false);

            _directory = await JsonSerializer.DeserializeAsync<Directory>(directoryStream).ConfigureAwait(false);
        }
    }

    private async Task<string> PostAsGetAsync(string url)
    {
        var message = await GetSignedMessageAsync(url).ConfigureAwait(false);

        (_, _, string responseText) = await PostAsync(url, message).ConfigureAwait(false);

        return responseText;
    }

    private async Task<T> PostAsGetAsync<T>(string url)
        where T : new()
    {
        var message = await GetSignedMessageAsync(url).ConfigureAwait(false);

        (_, _, string responseText) = await PostAsync(url, message).ConfigureAwait(false);

        return JsonSerializer.Deserialize<T>(responseText)!;
    }

    private async Task<(HttpStatusCode statusCode, string? location, string responseText)> PostAsync(
        string url,
        JwsEncodedMessage message)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url) {
            Content = new StringContent(
                content   : JsonSerializer.Serialize(message),
                encoding  : Encoding.UTF8,
                mediaType : "application/jose+json"
            )
        };

        // Remove the charset
        request.Content.Headers.ContentType!.CharSet = null;

        return await SendAsync(request).ConfigureAwait(false);
    }

    private async Task<(HttpStatusCode statusCode, string? location, string responseText)> SendAsync(HttpRequestMessage request)
    {
        using HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

        // capture the replay nonce for the next request
        if (request.Method == HttpMethod.Post &&
            response.Headers.TryGetValues("Replay-Nonce", out var replayNonce))
        {
            // ensure the queue does not exceed 3 entries
            if (nonces.Count > 3)
            {
                nonces.TryDequeue(out _); // remove the oldest nonce when the queue exceeds three entries
            }

            nonces.Enqueue(new Nonce(string.Join(",", replayNonce), DateTime.UtcNow));
        }

        // Content-Type is null when only a Location is returned
        string? contentType = response.Content.Headers.ContentType?.ToString();

        string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        string? location = null;

        if (response.Headers.TryGetValues("Location", out var locationHeader))
        {
            location = locationHeader.FirstOrDefault();
        }

        if (contentType is "application/problem+json")
        {
            var problem = JsonSerializer.Deserialize<Problem>(responseText)!;

            throw new AcmeException(problem);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new AcmeException("Invalid response: " + responseText);
        }

        // TODO: error handling
        // retry if invalid nonce?

        return (response.StatusCode, location, responseText);
    }

    private async Task<JwsEncodedMessage> GetSignedMessageAsync<T>(string url, T payload)
        where T: notnull
    {
        Nonce nonce = await GetNonceAsync().ConfigureAwait(false);

        AcmeMessageHeader header = GetMessageHeader(url, nonce);

        return Jws.Sign(
            integrityProtectedHeader : header,
            payload                  : payload,
            privateKey               : _privateKey
        );
    }

    private async Task<JwsEncodedMessage> GetSignedMessageAsync(string url)
    {
        Nonce nonce = await GetNonceAsync().ConfigureAwait(false);

        AcmeMessageHeader header = GetMessageHeader(url, nonce);

        return Jws.Sign(
            base64EncodedIntegrityProtectedHeader : GetBase64UrlEncodedJson(header),
            base64UrlEncodedPayload               : string.Empty,
            privateKey                            : _privateKey
        );
    }

    private AcmeMessageHeader GetMessageHeader(string url, Nonce nonce)
    {
        var header = new AcmeMessageHeader(nonce, url);

        if (_accountUrl is not null)
        {
            header.Kid = _accountUrl;
        }
        else
        {
            header.Jwk = Jwk;
        }

        return header;
    }

    private static readonly JsonSerializerOptions jso = new () { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    private static string GetBase64UrlEncodedJson<T>(T instance)
    {
        byte[] utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(instance, jso);

        return Base64Url.Encode(utf8Bytes);
    }
}