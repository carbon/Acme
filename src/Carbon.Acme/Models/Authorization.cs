using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    [DataContract]
    public class Authorization
    {
        /// <summary>
        /// The identifier that the account is authorized to represent.
        /// </summary>
        [DataMember(Name = "identifier", IsRequired = true)]
        public Identifier Identifier { get; set; }

        /// <summary>
        /// The status of this authorization. 
        /// Possible values are: “pending”, “valid”, “invalid”, “deactivated”, “expired”, and “revoked”.
        /// </summary>
        [DataMember(Name = "status", IsRequired = true)]
        public AuthorizationStatus Status { get; set; }

        /// <summary>
        /// The timestamp after which the server will consider this authorization invalid.
        /// This field is REQUIRED for objects with "valid" in the "status" field.
        /// </summary>
        [DataMember(Name = "expires", EmitDefaultValue = false)]
        public DateTime? Expires { get; set; }

        /// <summary>
        /// For pending authorizations, the challenges that the client
        /// can fulfill in order to prove possession of the identifier. 
        /// For final authorizations (in the "valid" or "invalid" state),
        /// the challenges that were used. 
        /// </summary>
        [DataMember(Name = "challenges", IsRequired = true)]
        public Challenge[] Challenges { get; set; }

        [DataMember(Name = "wildcard")]
        public bool? Wildcard { get; set; }
    }
}