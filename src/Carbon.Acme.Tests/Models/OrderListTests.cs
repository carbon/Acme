using System.Text.Json;

using Carbon.Acme.Serialization;

namespace Carbon.Acme.Tests;

public class OrderListTests
{
    [Fact]
    public void CanDeserialize()
    {
        OrderList list = JsonSerializer.Deserialize(
            """
            {
              "orders": [
                "https://example.com/acme/acct/1/order/1",
                "https://example.com/acme/acct/1/order/2"
              ]
            }
            """, AcmeSerializerContext.Default.OrderList);

        Assert.Equal("https://example.com/acme/acct/1/order/1", list.Urls[0]);
        Assert.Equal("https://example.com/acme/acct/1/order/2", list.Urls[1]);
    }
}