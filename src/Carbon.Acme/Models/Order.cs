#nullable disable

using System;
using System.Text.Json.Serialization;

using Carbon.Acme.Exceptions;

namespace Carbon.Acme
{
    public sealed class Order
    {
        [JsonIgnore]
        public string Url { get; set; }

        /// <summary>
        /// pending, ready, processing, valid, invalid
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// The timestamp after which the server will consider this order invalid
        /// </summary>
        [JsonPropertyName("expires")]
        public DateTime? Expires { get; set; }

        [JsonPropertyName("identifiers")]
        public Identifier[] Identifiers { get; set; }

        [JsonPropertyName("notBefore")]
        public DateTime? NotBefore { get; set; }

        [JsonPropertyName("notAfter")]
        public DateTime? NotAfter { get; set; }

        /// <summary>
        /// For pending orders, the authorizations that the client needs to complete before 
        /// the requested certificate can be issued (see {{identifier-authorization}}). 
        /// For final orders (in the "valid" or "invalid" state), the authorizations that were completed.
        /// Each entry is a URL from which an authorization can be fetched with a GET request.
        /// </summary>
        [JsonPropertyName("authorizations")]
        public string[] AuthorizationUrls { get; set; }

        /// <summary>
        /// A URL that a CSR must be POSTed to once all of the order’s authorizations are satisfied to finalize the order. 
        /// The result of a successful finalization will be the population of the certificate URL for the order.
        /// </summary>
        [JsonPropertyName("finalize")]
        public string FinalizeUrl { get; set; }

#nullable enable

        /// <summary>
        /// A URL for the certificate that has been issued in response to this order.
        /// </summary>
        [JsonPropertyName("certificate")]
        public string? CertificateUrl { get; set; }

        /// <summary>
        /// The error that occurred while processing the order, if any. 
        /// </summary>
        [JsonPropertyName("problem")]
        public Problem? Error { get; set; }
    }
}