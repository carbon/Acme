#nullable disable

using Carbon.Jose;

namespace Carbon.Acme
{
    public class ChangeKeyRequest
    {
        public JwsEncodedMessage Message { get; set; }
    }
}