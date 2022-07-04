namespace Carbon.Acme;

public readonly struct Nonce
{
    public Nonce(string value, DateTime created)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;
        Created = created;
    }

    public string Value { get; }

    public DateTime Created { get; }

    public TimeSpan Age => DateTime.UtcNow - Created;
}