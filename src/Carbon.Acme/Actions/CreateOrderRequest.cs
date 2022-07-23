using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class CreateOrderRequest
{
    public CreateOrderRequest(
      string domainName,
      DateTime? notBefore = null,
      DateTime? notAfter = null)
    {
        Identifiers = new[] { new Identifier("dns", domainName) };
        NotBefore = notBefore;
        NotAfter = notAfter;
    }

    public CreateOrderRequest(
        Identifier[] identifiers,
        DateTime? notBefore = null,
        DateTime? notAfter = null)
    {
        ArgumentNullException.ThrowIfNull(identifiers);

        Identifiers = identifiers;
        NotBefore = notBefore;
        NotAfter = notAfter;
    }

    [JsonPropertyName("identifiers")]
    public Identifier[] Identifiers { get; }

    [JsonPropertyName("notBefore")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? NotBefore { get; }

    [JsonPropertyName("notAfter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? NotAfter { get; }
}

// https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.4