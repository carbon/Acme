#pragma warning disable CA1822 // Mark members as static

using System;
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
