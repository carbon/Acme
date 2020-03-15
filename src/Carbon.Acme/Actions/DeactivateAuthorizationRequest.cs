using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public sealed class DeactivateAuthorizationRequest
    {
        public DeactivateAuthorizationRequest(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        [JsonIgnore]
        public string Url { get; }

        [JsonPropertyName("status")]
        public string Status => "deactivated";
    }
}
