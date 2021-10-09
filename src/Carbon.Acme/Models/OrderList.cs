#nullable disable

using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class OrderList
{
    [JsonPropertyName("orders")]
    public string[] Urls { get; init; }

    // TODO Next
    // Link: <https://example.com/acme/acct/1/orders?cursor=2>, rel="next"
}
