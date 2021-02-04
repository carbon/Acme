#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public sealed class ValidationRecord
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; init; }

        [JsonPropertyName("port")]
        public int Port { get; init; }

        [JsonPropertyName("addressesResolved")]
        public string[] AddressesResolved { get; init; }

        [JsonPropertyName("addressUsed")]
        public string AddressUsed { get; init; }
    }
}