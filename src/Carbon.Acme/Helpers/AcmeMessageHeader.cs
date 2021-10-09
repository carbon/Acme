using System.Text.Json.Serialization;

using Carbon.Jose;

namespace Carbon.Acme
{
    public sealed class AcmeMessageHeader
    {
        public AcmeMessageHeader(Nonce nonce, string url)
        {
            Nonce = nonce.Value;
            Url = url;
        }

        [JsonPropertyName("alg")]
        public string Alg => AlgorithmNames.RS256;

        [JsonPropertyName("nonce")]
        public string Nonce { get; }

        [JsonPropertyName("url")]
        public string Url { get; }

#nullable enable

        [JsonPropertyName("kid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Kid { get; set; }

        [JsonPropertyName("jwk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Jwk? Jwk { get; set; }
    }
}
