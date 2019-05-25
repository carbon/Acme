#nullable disable

using System;
using System.Runtime.Serialization;

using Carbon.Acme.Exceptions;

namespace Carbon.Acme
{
    public class Challenge
    {
        // http-01 | dns-01 | tls-sni-02
        [DataMember(Name = "type", IsRequired = true)]
        public string Type { get; set; }

        /// <summary>
        /// The URL to which a response can be posted.
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// The status of this challenge. Possible values are: "pending", "valid", and "invalid".
        /// </summary>
        [DataMember(Name = "status")]
        public ChallengeStatus Status { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        /// <summary>
        /// The time at which the server validated this challenge, 
        /// encoded in the format specified in RFC 3339 {{RFC3339}}. 
        /// This field is REQUIRED if the "status" field is "valid".
        /// </summary>
        [DataMember(Name = "validated")]
        public DateTime? Validated { get; set; }

        [DataMember(Name = "keyAuthorization", EmitDefaultValue = false)]
        public string KeyAuthorization { get; set; }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        public Problem Error { get; set; }
    }
}

/*
HTTP: Provision files under .well-known on a web server for the domain
TLS SNI: Configure a TLS server for the domain
DNS: Provision DNS resource records for the domain
*/
