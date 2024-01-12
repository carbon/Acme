#nullable disable

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

using Carbon.Acme.Serialization;

namespace Carbon.Acme;

public sealed class Directory
{
    [JsonPropertyName("newNonce")]
    public string NewNonceUrl { get; init; }

    [JsonPropertyName("newAccount")]
    public string NewAccountUrl { get; init; }

    [JsonPropertyName("newOrder")]
    public string NewOrderUrl { get; init; }

    [JsonPropertyName("newAuthz")]
    public string NewAuthorizationUrl { get; init; }

    [JsonPropertyName("revokeCert")]
    public string RevokeCertificateUrl { get; init; }

    [JsonPropertyName("keyChange")]
    public string KeyChangeUrl { get; init; }

    [JsonPropertyName("meta")]
    public DirectoryMetadata Meta { get; init; }

    public static async ValueTask<Directory> GetAsync(string url = "https://acme-v02.api.letsencrypt.org/directory")
    {
        using HttpClient http = new();

        var result = await http.GetFromJsonAsync(url, AcmeSerializerContext.Default.Directory).ConfigureAwait(false);

        return result;
    }
}