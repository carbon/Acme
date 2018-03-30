using Carbon.Json;
using Xunit;

namespace Carbon.Acme.Tests
{
    public class CreateOrderRequestTests
    {
        [Fact]
        public void Construct()
        {
            var action = new CreateOrderRequest("test.com");
            
            Assert.Equal(new Identifier("dns", "test.com"), action.Identifiers[0]);

            Assert.Equal(@"{
  ""identifiers"": [
    {
      ""type"": ""dns"",
      ""value"": ""test.com""
    }
  ]
}", JsonObject.FromObject(action).ToString());
        }
    }
}