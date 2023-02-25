using System.Text.Json.Serialization;

namespace Carbon.Acme;

public readonly struct Identifier : IEquatable<Identifier>
{
    [JsonConstructor]
    public Identifier(string type, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(type);
        ArgumentException.ThrowIfNullOrEmpty(value);

        Type = type;
        Value = value;
    }

    [JsonPropertyName("type")]
    public string Type { get; }

    [JsonPropertyName("value")]
    public string Value { get; }

    public bool Equals(Identifier other)
    {
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

    public static bool operator ==(Identifier left, Identifier right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Identifier left, Identifier right)
    {
        return !left.Equals(right);
    }
}
