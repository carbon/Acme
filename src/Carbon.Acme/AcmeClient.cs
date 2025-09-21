using System.Buffers;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

using Carbon.Acme.Exceptions;
using Carbon.Acme.Serialization;
using Carbon.Jose;
using Carbon.Jose.Serialization;

namespace Carbon.Acme;

// --------------------------------------------------------
// Automatic Certificate Management Environment (ACME)
// RFC 8555 | https://tools.ietf.org/html/rfc8555
// --------------------------------------------------------

public class AcmeClient
{
    private readonly HttpClient _httpClient = new () {
        DefaultRequestHeaders = {
            {  "User-Agent", "Carbon.Acme/2" }
        }
    };

    private readonly string _directoryUrl;
    private readonly RSA _privateKey;

    private string? _accountUrl;
    private Directory? _directory;

    private readonly ConcurrentQueue<Nonce> _nonces = new();

    public AcmeClient(
        RSA privateKey,
        string? accountUrl = null,
        string directoryUrl = "https://acme-v02.api.letsencrypt.org/directory")
    {
        ArgumentNullException.ThrowIfNull(privateKey);
        ArgumentException.ThrowIfNullOrEmpty(directoryUrl);

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

        return JsonSerializer.Deserialize(responseText, AcmeSerializerContext.Default.Account)!;
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

        return JsonSerializer.Deserialize(responseText, AcmeSerializerContext.Default.Account)!;
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
        Authorization result = await PostAsGetAsync(url, AcmeSerializerContext.Default.Authorization).ConfigureAwait(false);

        var retryAfter = TimeSpan.FromSeconds(2);

        // Let's Encrypt does not support Retry-After

        /*
        if (response.Headers.NonValidated.TryGetValues("Retry-After", out var retryAfterHeader)
            && int.TryParse(retryAfterHeader.ToString(), out int retryAfterSeconds))
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

        while (result.Status is AuthorizationStatus.Pending && retryCount < 5)
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

        return JsonSerializer.Deserialize(responseText, AcmeSerializerContext.Default.Authorization)!;
    }

    public async Task<Challenge> GetChallengeAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return await PostAsGetAsync(url, AcmeSerializerContext.Default.Challenge).ConfigureAwait(false);
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

        return JsonSerializer.Deserialize(responseText, AcmeSerializerContext.Default.Challenge)!;
    }

    #endregion

    #region Orders

    public async Task<Order> GetOrderAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return await PostAsGetAsync(url, AcmeSerializerContext.Default.Order).ConfigureAwait(false);
    }

    public async Task<OrderList> ListOrdersAsync(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        // Link: <https://example.com/acme/acct/1/orders?cursor=2>, rel="next"

        return await PostAsGetAsync(url, AcmeSerializerContext.Default.OrderList).ConfigureAwait(false);
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

        Order result = JsonSerializer.Deserialize(responseText, AcmeSerializerContext.Default.Order)!;

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

        return JsonSerializer.Deserialize(responseText, AcmeSerializerContext.Default.Order)!;
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

            string e = Base64Url.EncodeToString(_jwk.Exponent);
            string n = Base64Url.EncodeToString(_jwk.Modulus);

            var rentedBuffer = ArrayPool<byte>.Shared.Rent(e.Length + n.Length + 64);

            Utf8.TryWrite(rentedBuffer, $$"""{"e":"{{e}}","kty":"RSA","n":"{{n}}"}""", out int messageLength);
            var json = rentedBuffer.AsSpan(0, messageLength);

            Span<byte> hash = stackalloc byte[SHA256.HashSizeInBytes];

            SHA256.HashData(json, hash);

            _thumbprint = Base64Url.EncodeToString(hash);

            ArrayPool<byte>.Shared.Return(rentedBuffer);
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
        // TXT _acme-challenge.{host} "{value}"

        string result = GetKeyAuthorization(token);

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(result));

        return Base64Url.EncodeToString(hash);
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

        while (_nonces.TryDequeue(out Nonce nonce))
        {
            // Ignore nonces older than 1 minute old
            if (nonce.Age > TimeSpan.FromMinutes(1))
            {
                continue;
            }

            return nonce;
        }

        // HEAD /acme/new-nonce HTTP/1.1 ->
        // Replay-Nonce: oFvnlFP1wIhRlYS2jTaXbA

        using HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _directory!.NewNonceUrl)).ConfigureAwait(false);

        string replayNonce = response.Headers.NonValidated["Replay-Nonce"].ToString();

        return new Nonce(replayNonce, DateTime.UtcNow);
    }

    #endregion

    private Jwk? _jwk;

    internal Jwk Jwk => _jwk ??= Jwk.FromRSAPublicParameters(_privateKey.ExportParameters(includePrivateParameters: false));

    public bool IsInitialized => _accountUrl is not null && _directory is not null;

    public async Task InitializeAsync()
    {
        await InitializeDirectoryAsync().ConfigureAwait(false);

        _accountUrl ??= await GetAccountUrlAsync().ConfigureAwait(false);
    }

    private async Task InitializeDirectoryAsync()
    {
        // NOTE: The server MUST allow GET requests for the directory and newNonce resources (see Section 7.1)

        _directory ??= await _httpClient.GetFromJsonAsync(_directoryUrl, AcmeSerializerContext.Default.Directory).ConfigureAwait(false);
    }

    private async Task<string> PostAsGetAsync(string url)
    {
        var message = await GetSignedMessageAsync(url).ConfigureAwait(false);

        (_, _, string responseText) = await PostAsync(url, message).ConfigureAwait(false);

        return responseText;
    }

    private async Task<T> PostAsGetAsync<T>(string url, JsonTypeInfo<T> jsonTypeInfo)
    {
        var message = await GetSignedMessageAsync(url).ConfigureAwait(false);

        (_, _, string responseText) = await PostAsync(url, message).ConfigureAwait(false);

        return JsonSerializer.Deserialize<T>(responseText, jsonTypeInfo)!;
    }

    private async Task<(HttpStatusCode statusCode, string? location, string responseText)> PostAsync(string url, JwsEncodedMessage message)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url) {
            Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(message, JoseSerializerContext.Default.JwsEncodedMessage)) {
                Headers = {
                    { "Content-Type", "application/jose+json" }
                }
            }
        };

        return await SendAsync(request).ConfigureAwait(false);
    }

    private async Task<(HttpStatusCode statusCode, string? location, string responseText)> SendAsync(HttpRequestMessage request)
    {
        using HttpResponseMessage response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        // capture the replay nonce for the next request
        if (request.Method == HttpMethod.Post &&
            response.Headers.NonValidated.TryGetValues("Replay-Nonce", out var replayNonce))
        {
            // ensure the queue does not exceed 3 entries
            if (_nonces.Count > 3)
            {
                _nonces.TryDequeue(out _); // remove the oldest nonce when the queue exceeds three entries
            }

            _nonces.Enqueue(new Nonce(replayNonce.ToString(), DateTime.UtcNow));
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
            var problem = JsonSerializer.Deserialize(responseText, AcmeSerializerContext.Default.Problem)!;

            throw new AcmeException(problem);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new AcmeException($"Invalid response: {responseText}");
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
            base64EncodedIntegrityProtectedHeader: Base64Url.EncodeToString(
                JsonSerializer.SerializeToUtf8Bytes(header, AcmeSerializerContext.Default.AcmeMessageHeader)),
            base64UrlEncodedPayload: string.Empty,
            privateKey: _privateKey
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
}