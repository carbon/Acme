#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme.Exceptions
{
    public sealed class Problem
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("instance")]
        public string Instance { get; set; }

        [JsonPropertyName("subproblems")]
        public Subproblem[] Subproblems { get; set; }
    }
}