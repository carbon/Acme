using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class CreateOrderRequest
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
            Identifiers = identifiers ?? throw new ArgumentNullException(nameof(identifiers));
            NotBefore = notBefore;
            NotAfter = notAfter;
        }

        [DataMember(Name = "identifiers", IsRequired = true)]
        public Identifier[] Identifiers { get; }

        [DataMember(Name = "notBefore", EmitDefaultValue = false)]
        public DateTime? NotBefore { get; }

        [DataMember(Name = "notAfter", EmitDefaultValue = false)]
        public DateTime? NotAfter { get; }
    }
}

// https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.4