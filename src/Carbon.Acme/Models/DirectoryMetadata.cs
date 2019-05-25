using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class DirectoryMetadata
    {
        /// <summary>
        /// A URL identifying the current terms of service.
        /// </summary>
        [DataMember(Name = "termsOfService")]
        public string? TermsOfService { get; set; }

        /// <summary>
        /// An HTTP or HTTPS URL locating a website providing 
        /// more information about the ACME server.
        /// </summary>
        [DataMember(Name = "website")]
        public string? Website { get; set; }

        [DataMember(Name = "caaIdentities")]
        public string[]? CaaIdentities { get; set; }

        [DataMember(Name = "externalAccountRequired")]
        public bool? ExternalAccountRequired { get; set; }
    }
}