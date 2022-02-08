using System.Text.Json;
using System.Text.Json.Serialization;

namespace Carbon.Acme.Tests
{
    public class CreateOrderRequestTests
    {
        [Fact]
        public void Construct()
        {
            var request = new CreateOrderRequest("test.com");
            
            Assert.Equal(new Identifier("dns", "test.com"), request.Identifiers[0]);

  
            Assert.Equal(@"{
  ""identifiers"": [
    {
      ""type"": ""dns"",
      ""value"": ""test.com""
    }
  ]
}", JsonSerializer.Serialize(request, new JsonSerializerOptions {
                WriteIndented = true, 
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            } ));


        }
    }
}