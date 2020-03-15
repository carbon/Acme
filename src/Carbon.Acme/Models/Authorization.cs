#nullable disable

using System;
using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public sealed class Authorization
    {
        /// <summary>
        /// The identifier that the account is authorized to represent.
        /// </summary>
        [JsonPropertyName("identifier")]
        public Identifier Identifier { get; set; }

        /// <summary>
        /// The status of this authorization. 
        /// Possible values are: “pending”, “valid”, “invalid”, “deactivated”, “expired”, and “revoked”.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AuthorizationStatus Status { get; set; }

        /// <summary>
        /// The timestamp after which the server will consider this authorization invalid.
        /// This field is REQUIRED for objects with "valid" in the "status" field.
        /// </summary>
        [JsonPropertyName("expires")]
        public DateTime? Expires { get; set; }

        /// <summary>
        /// For pending authorizations, the challenges that the client
        /// can fulfill in order to prove possession of the identifier. 
        /// For final authorizations (in the "valid" or "invalid" state),
        /// the challenges that were used. 
        /// </summary>
        [JsonPropertyName("challenges")]
        public Challenge[] Challenges { get; set; }

        [JsonPropertyName("wildcard")]
        public bool? Wildcard { get; set; }
    }
}