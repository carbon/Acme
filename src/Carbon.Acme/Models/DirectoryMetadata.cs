using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public sealed class DirectoryMetadata
    {
        /// <summary>
        /// A URL identifying the current terms of service.
        /// </summary>
        [JsonPropertyName("termsOfService")]
        public string? TermsOfService { get; set; }

        /// <summary>
        /// An HTTP or HTTPS URL locating a website providing 
        /// more information about the ACME server.
        /// </summary>
        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("caaIdentities")]
        public string[]? CaaIdentities { get; set; }

        [JsonPropertyName("externalAccountRequired")]
        public bool? ExternalAccountRequired { get; set; }
    }
}