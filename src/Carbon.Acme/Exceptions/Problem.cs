#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme.Exceptions
{
    public sealed class Problem
    {
        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("detail")]
        public string Detail { get; init; }

        [JsonPropertyName("status")]
        public int? Status { get; init; }

        [JsonPropertyName("instance")]
        public string Instance { get; init; }

#nullable enable

        [JsonPropertyName("subproblems")]
        public Subproblem[]? Subproblems { get; init; }
    }
}