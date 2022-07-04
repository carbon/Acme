using System.Text.Json;

namespace Carbon.Acme.Tests;

public class OrderListTests
{
    [Fact]
    public void CanDeserialize()
    {
        var list = JsonSerializer.Deserialize<OrderList>(
            """
            {
              "orders": [
                "https://example.com/acme/acct/1/order/1",
                "https://example.com/acme/acct/1/order/2"
              ]
            }
            """);

        Assert.Equal("https://example.com/acme/acct/1/order/1", list.Urls[0]);
        Assert.Equal("https://example.com/acme/acct/1/order/2", list.Urls[1]);
    }
}