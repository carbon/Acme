using System.Text.Json.Serialization;

using Carbon.Extensions;

namespace Carbon.Acme;

public sealed class RevokeCertificateRequest
{
    public RevokeCertificateRequest(byte[] certificate, int? reason = null)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        Certificate = Base64Url.Encode(certificate);
        Reason = reason;
    }

    public RevokeCertificateRequest(string certificate, int? reason = null)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        Certificate = certificate;
        Reason = reason;
    }

    /// <summary>
    /// The certificate to be revoked, in the base64url-encoded version of the DER format. 
    /// Note: Because this field uses base64url, and does not include headers, it is different from PEM.
    /// </summary>
    [JsonPropertyName("certificate")]
    public string Certificate { get; }

    [JsonPropertyName("reason")]
    public int? Reason { get; }
}

// https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.6