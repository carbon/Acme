using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class Order
    {
        [IgnoreDataMember]
        public string Url { get; set; }

        /// <summary>
        /// pending, ready, processing, valid, invalid
        /// </summary>
        [DataMember(Name = "status", IsRequired = true)]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// The timestamp after which the server will consider this order invalid
        /// </summary>
        [DataMember(Name = "expires", EmitDefaultValue = false)]
        public DateTime? Expires { get; set; }

        [DataMember(Name = "identifiers", IsRequired = true)]
        public Identifier[] Identifiers { get; set; }

        [DataMember(Name = "notBefore", EmitDefaultValue = false)]
        public DateTime? NotBefore { get; set; }

        [DataMember(Name = "notAfter", EmitDefaultValue = false)]
        public DateTime? NotAfter { get; set; }

        /// <summary>
        /// For pending orders, the authorizations that the client needs to complete before 
        /// the requested certificate can be issued (see {{identifier-authorization}}). 
        /// For final orders (in the "valid" or "invalid" state), the authorizations that were completed.
        /// Each entry is a URL from which an authorization can be fetched with a GET request.
        /// </summary>
        [DataMember(Name = "authorizations", IsRequired = true)]
        public string[] AuthorizationUrls { get; set; }

        /// <summary>
        /// A URL that a CSR must be POSTed to once all of the order’s authorizations are satisfied to finalize the order. 
        /// The result of a successful finalization will be the population of the certificate URL for the order.
        /// </summary>
        [DataMember(Name = "finalize", IsRequired = true)]
        public string FinalizeUrl { get; set; }

        /// <summary>
        /// A URL for the certificate that has been issued in response to this order.
        /// </summary>
        [DataMember(Name = "certificate", EmitDefaultValue = false)]
        public string CertificateUrl { get; set; }

        /// <summary>
        /// The error that occurred while processing the order, if any. 
        /// </summary>
        [DataMember(Name = "problem", EmitDefaultValue = false)]
        public Problem Error { get; set; }
    }
}