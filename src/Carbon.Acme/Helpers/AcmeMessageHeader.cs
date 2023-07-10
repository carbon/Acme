using System.Text.Json.Serialization;

using Carbon.Jose;

namespace Carbon.Acme;

public sealed class AcmeMessageHeader(Nonce nonce, string url)
{
    [JsonPropertyName("alg")]
    public string Alg => AlgNames.RS256;

    [JsonPropertyName("nonce")]
    public string Nonce { get; } = nonce.Value;

    [JsonPropertyName("url")]
    public string Url { get; } = url;

#nullable enable

    [JsonPropertyName("kid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Kid { get; set; }

    [JsonPropertyName("jwk")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Jwk? Jwk { get; set; }
}
