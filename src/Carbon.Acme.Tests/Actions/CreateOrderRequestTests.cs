using System.Text.Json;

namespace Carbon.Acme.Tests;

public class CreateOrderRequestTests
{
    [Fact]
    public void CanConstruct()
    {
        var request = new CreateOrderRequest("test.com");
        
        Assert.Equal(new Identifier("dns", "test.com"), request.Identifiers[0]);

        Assert.Equal(
            """
            {
              "identifiers": [
                {
                  "type": "dns",
                  "value": "test.com"
                }
              ]
            }
            """, JsonSerializer.Serialize(request, JSO.Indented).ReplaceLineEndings("\n"));
    }
}