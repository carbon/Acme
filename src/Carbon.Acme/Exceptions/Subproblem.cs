#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme.Exceptions;

public sealed class Subproblem
{
    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("detail")]
    public string Detail { get; init; }

#nullable enable

    [JsonPropertyName("identifier")]
    public Identifier? Identifier { get; init; }
}