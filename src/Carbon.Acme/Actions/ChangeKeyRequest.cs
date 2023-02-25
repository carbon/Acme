using Carbon.Jose;

namespace Carbon.Acme;

public sealed class ChangeKeyRequest
{
    public required JwsEncodedMessage Message { get; init; }
}
