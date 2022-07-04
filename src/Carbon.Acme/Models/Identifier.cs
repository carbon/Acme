using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class Identifier : IEquatable<Identifier>
{
#nullable disable
    public Identifier() { }
#nullable enable

    public Identifier(string type, string value)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(value);

        Type = type;
        Value = value;
    }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("value")]
    public string Value { get; init; }

    public bool Equals(Identifier? other)
    {
        if (other is null) return this is null;

        return string.Equals(Type, other.Type, StringComparison.Ordinal)
            && string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is Identifier other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Value);
    }
}
