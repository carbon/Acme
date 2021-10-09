using System;

namespace Carbon.Acme;

public readonly struct Nonce
{
    public Nonce(string value, DateTime created)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Created = created;
    }

    public string Value { get; }

    public DateTime Created { get; }

    public TimeSpan Age => DateTime.UtcNow - Created;
}