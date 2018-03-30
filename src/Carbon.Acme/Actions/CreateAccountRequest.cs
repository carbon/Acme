using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class CreateAccountRequest
    {
        public CreateAccountRequest(
            bool termsOfServiceAgreed, 
            string[] contact, 
            bool? onlyReturnExisting = null)
        {
            TermsOfServiceAgreed = termsOfServiceAgreed;
            Contact              = contact;
            OnlyReturnExisting   = onlyReturnExisting;
        }

        [DataMember(Name = "termsOfServiceAgreed")]
        public bool TermsOfServiceAgreed { get; }

        [DataMember(Name = "contact", EmitDefaultValue = false)]
        public string[] Contact { get; }

        [DataMember(Name = "onlyReturnExisting", EmitDefaultValue = false)]
        public bool? OnlyReturnExisting { get; }
    }
}

// TODO: externalAccountBinding support

/*
{
  "protected": base64url({
    "alg": "ES256",
    "jwk": {...},
    "nonce": "6S8IqOGY7eL2lsGoTZYifg",
    "url": "https://example.com/acme/new-account"
  }),
  "payload": base64url({
    "termsOfServiceAgreed": true,
    "contact": [
      "mailto:cert-admin@example.com",
      "mailto:admin@example.com"
    ]
  }),
  "signature": "RZPOnYoPs1PhjszF...-nh6X1qtOFPB519I"
}
*/
