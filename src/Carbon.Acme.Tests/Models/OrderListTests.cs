using System.Text.Json;

using Xunit;

namespace Carbon.Acme.Tests
{
    public class OrderListTests
    {
        [Fact]
        public void Deserialize()
        {
            var text = @"{
  ""orders"": [
    ""https://example.com/acme/acct/1/order/1"",
    ""https://example.com/acme/acct/1/order/2""
  ]
}";

            var list = JsonSerializer.Deserialize<OrderList>(text);

            Assert.Equal("https://example.com/acme/acct/1/order/1", list.Urls[0]);
            Assert.Equal("https://example.com/acme/acct/1/order/2", list.Urls[1]);
        }
    }
}
