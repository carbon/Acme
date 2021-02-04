#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public sealed class Account
    {
        [JsonPropertyName("status")]        
        public AccountStatus Status { get; init; }

        [JsonPropertyName("contact")]
        public string[] Contact { get; init; }

        [JsonPropertyName("termsOfServiceAgreed")]
        public bool TermsOfServiceAgreed { get; init; }

        [JsonPropertyName("orders")]
        public string OrdersUrl { get; init; }

        [JsonIgnore]
        public string Url { get; init; }
    }
}
