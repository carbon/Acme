using Carbon.Json;
using Xunit;

namespace Carbon.Acme.Tests
{
    public class AccountTests
    {
        [Fact]
        public void A()
        {
            Account model = JsonObject.Parse(@"{
  ""status"": ""valid"",
  ""contact"": [
    ""mailto:cert-admin@example.com"",
    ""tel:+12025551212""
  ],
  ""termsOfServiceAgreed"": true,
  ""orders"": ""https://example.com/acme/acct/1/orders""
}").As<Account>();

            Assert.Equal("mailto:cert-admin@example.com", model.Contact[0]);
            Assert.Equal(AccountStatus.Valid, model.Status);
            Assert.True(model.TermsOfServiceAgreed);
            Assert.Equal("https://example.com/acme/acct/1/orders", model.OrdersUrl);
        }
    }


}
