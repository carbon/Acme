using System.Text.Json.Serialization;

using Carbon.Extensions;

namespace Carbon.Acme;

public sealed class FinalizeOrderRequest
{
    public FinalizeOrderRequest(string url, byte[] csr)
    {
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(csr);

        Url = url;
        Csr = Base64Url.Encode(csr);
    }

    public FinalizeOrderRequest(string url, string csr)
    {
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(csr);

        Url = url;
        Csr = csr;
    }

    [JsonIgnore]
    public string Url { get; }

    /// <summary>
    /// A CSR encoding the parameters for the certificate being requested {{!RFC2986}}.
    /// The CSR is sent in the base64url-encoded version of the DER format. 
    /// Note: Because this field uses base64url, and does not include headers, it is different from PEM.
    /// </summary>
    [JsonPropertyName("csr")]
    public string Csr { get; }
}
