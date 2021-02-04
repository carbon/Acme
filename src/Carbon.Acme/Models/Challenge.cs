#nullable disable

using System;
using System.Text.Json.Serialization;

using Carbon.Acme.Exceptions;

namespace Carbon.Acme
{
    public sealed class Challenge
    {
        // http-01 | dns-01 | tls-sni-02
        [JsonPropertyName("type")]
        public string Type { get; init; }

        /// <summary>
        /// The URL to which a response can be posted.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; init; }

        /// <summary>
        /// The status of this challenge. Possible values are: "pending", "valid", and "invalid".
        /// </summary>
        [JsonPropertyName("status")]
        public ChallengeStatus Status { get; init; }

        [JsonPropertyName("token")]
        public string Token { get; init; }

        /// <summary>
        /// The time at which the server validated this challenge, 
        /// encoded in the format specified in RFC 3339 {{RFC3339}}. 
        /// This field is REQUIRED if the "status" field is "valid".
        /// </summary>
        [JsonPropertyName("validated")]
        public DateTime? Validated { get; init; }

#nullable enable

        [JsonPropertyName("keyAuthorization")]
        public string? KeyAuthorization { get; init; }

        [JsonPropertyName("error")]
        public Problem? Error { get; init; }
    }
}

/*
HTTP: Provision files under .well-known on a web server for the domain
TLS SNI: Configure a TLS server for the domain
DNS: Provision DNS resource records for the domain
*/
