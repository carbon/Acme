using System;

namespace Carbon.Acme
{
    public readonly struct Nonce
    {
        public Nonce(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }
    }
}