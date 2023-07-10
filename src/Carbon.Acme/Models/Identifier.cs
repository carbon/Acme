using System.Text.Json.Serialization;

namespace Carbon.Acme;

[method: JsonConstructor]
public readonly struct Identifier(string type, string value) : IEquatable<Identifier>
{
    [JsonPropertyName("type")]
    public string Type { get; } = type ?? throw new ArgumentNullException(nameof(type));

    [JsonPropertyName("value")]
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

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
