namespace Carbon.Acme;

public readonly struct Nonce(string value, DateTime created)
{
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

    public DateTime Created { get; } = created;

    public TimeSpan Age => DateTime.UtcNow - Created;
}