#nullable disable

using Carbon.Jose;

namespace Carbon.Acme;

public sealed class ChangeKeyRequest
{
    public JwsEncodedMessage Message { get; init; }
}
