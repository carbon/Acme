#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public class ValidationRecord
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("addressesResolved")]
        public string[] AddressesResolved { get; set; }

        [JsonPropertyName("addressUsed")]
        public string AddressUsed { get; set; }
    }
}