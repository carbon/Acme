using System.Text.Json;

namespace Carbon.Acme.Tests;

public class AccountTests
{
    [Fact]
    public void CanDeserialize()
    {
        Account model = JsonSerializer.Deserialize<Account>(
            """
            {
              "status": "valid",
              "contact": [
                "mailto:cert-admin@example.com",
                "tel:+12025551212"
              ],
              "termsOfServiceAgreed": true,
              "orders": "https://example.com/acme/acct/1/orders"
            }
            """);

        Assert.Equal("mailto:cert-admin@example.com", model.Contact[0]);
        Assert.Equal("tel:+12025551212",              model.Contact[1]);
        Assert.Equal(AccountStatus.Valid,             model.Status);

        Assert.True(model.TermsOfServiceAgreed);
        Assert.Equal("https://example.com/acme/acct/1/orders", model.OrdersUrl);
    }
}
