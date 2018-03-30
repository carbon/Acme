using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class Account
    {
        [DataMember(Name = "status", IsRequired = true)]
        public AccountStatus Status { get; set; }

        [DataMember(Name = "contact")]
        public string[] Contact { get; set; }

        [DataMember(Name = "termsOfServiceAgreed")]
        public bool TermsOfServiceAgreed { get; set; }

        [DataMember(Name = "orders", IsRequired = true)]
        public string OrdersUrl { get; set; }
        
        [IgnoreDataMember]
        public string Url { get; set; }
    }

    public enum AccountStatus : byte
    {
        Unknown     = 0,
        Valid       = 1,
        Deactivated = 2,
        Revoked     = 3
    }
    // Status: valid | deactivated | revoked
}
