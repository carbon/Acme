#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    public class UpdateAccountRequest
    {
        [JsonPropertyName("contact")]
        public string[] Contact { get; set; }
    }
}