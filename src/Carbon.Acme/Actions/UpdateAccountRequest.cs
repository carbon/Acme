#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public sealed class UpdateAccountRequest
    {
        [JsonPropertyName("contact")]
        public string[] Contact { get; init; }
    }
}