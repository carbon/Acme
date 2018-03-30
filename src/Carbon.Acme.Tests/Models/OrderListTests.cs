using Carbon.Json;
using Xunit;

namespace Carbon.Acme.Tests
{
    public class OrderListTests
    {
        [Fact]
        public void A()
        {
            var text = @"{
  ""orders"": [
    ""https://example.com/acme/acct/1/order/1"",
    ""https://example.com/acme/acct/1/order/2""
  ]
}";

            var list = JsonObject.Parse(text).As<OrderList>();

            Assert.Equal("https://example.com/acme/acct/1/order/1", list.Urls[0]);
        }
    }
}
