#nullable disable

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Carbon.Acme.Exceptions
{
    [DataContract]
    public sealed class Subproblem
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("identifier")]
        public Identifier Identifier { get; set; }
    }
}