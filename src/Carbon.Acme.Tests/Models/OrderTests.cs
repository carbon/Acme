using System.Text.Json;

using Carbon.Acme.Serialization;

namespace Carbon.Acme.Tests;

public class OrderTests
{
    [Fact] // ensure these do not changed
    public void StatusCodes()
    {
        Assert.Equal(1, (int)OrderStatus.Pending);
        Assert.Equal(2, (int)OrderStatus.Ready);
        Assert.Equal(3, (int)OrderStatus.Processing);
        Assert.Equal(4, (int)OrderStatus.Valid);
        Assert.Equal(5, (int)OrderStatus.Invalid);
    }

    [Fact]
    public void CanDeserialize()
    {
        Order order = JsonSerializer.Deserialize(
            """
            {
              "status": "pending",
              "expires": "2015-03-01T14:09:00Z",
              "identifiers": [
                { "type": "dns", "value": "example.com" },
                { "type": "dns", "value": "www.example.com" }
              ],
              "notBefore": "2016-01-01T00:00:00Z",
              "notAfter": "2016-01-08T00:00:00Z",
              "authorizations": [
                "https://example.com/acme/authz/1234",
                "https://example.com/acme/authz/2345"
              ],
              "finalize": "https://example.com/acme/acct/1/order/1/finalize",
              "certificate": "https://example.com/acme/cert/1234"
            }
            """, AcmeSerializerContext.Default.Order);

        Assert.Equal(OrderStatus.Pending,                                       order.Status);
        // Assert.Equal(IsoDate.Parse("2015-03-01T14:09:00Z").ToUtcDateTime(),     model.Expires);
        Assert.Equal("https://example.com/acme/acct/1/order/1/finalize",        order.FinalizeUrl);
        Assert.Equal("https://example.com/acme/cert/1234",                      order.CertificateUrl);

        Assert.Equal(new Identifier("dns", "example.com"),                      order.Identifiers[0]);
        Assert.Equal(new Identifier("dns", "www.example.com"),                  order.Identifiers[1]);

        Assert.Equal("https://example.com/acme/authz/1234", order.AuthorizationUrls[0]);
        Assert.Equal("https://example.com/acme/authz/2345", order.AuthorizationUrls[1]);
    }

    [Fact]
    public void CanDeserialize_Ready()
    {
        var model = JsonSerializer.Deserialize(
            """
            {
              "status": "ready",
              "finalize": "https://example.com/acme/acct/1/order/1/finalize"
            }
            """, AcmeSerializerContext.Default.Order);

        Assert.Equal(OrderStatus.Ready, model.Status);
    }
}