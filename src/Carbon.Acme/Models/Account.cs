#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public sealed class Account
    {
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountStatus Status { get; set; }

        [JsonPropertyName("contact")]
        public string[] Contact { get; set; }

        [JsonPropertyName("termsOfServiceAgreed")]
        public bool TermsOfServiceAgreed { get; set; }

        [JsonPropertyName("orders")]
        public string OrdersUrl { get; set; }

        [JsonIgnore]
        public string Url { get; set; }
    }
}
