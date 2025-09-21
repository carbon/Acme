using System.Text.Json;

namespace Carbon.Acme.Tests
{
    public class ClientTests
    {

        // [Fact]
        public async Task B()
        {
            var privateKey = TestData.GetPrivateKey();

            var client = new AcmeClient(privateKey, directoryUrl: "https://acme-staging-v02.api.letsencrypt.org/directory");

            var accountUrl = await client.CreateAccountAsync(new CreateAccountRequest(termsOfServiceAgreed: true, new[] {
                "mailto:test@processor.ai"
            }, false));

            throw new Exception(JsonSerializer.Serialize(new { accountUrl }));
        }

        // [Fact]
        public async Task D()
        {
            var privateKey = TestData.GetPrivateKey();

            var client = new AcmeClient(privateKey, directoryUrl: "https://acme-staging-v02.api.letsencrypt.org/directory");

            var accountUrl = await client.GetAccountUrlAsync();

            throw new Exception(JsonSerializer.Serialize(new { accountUrl }));
        }

        // [Fact]
        public async Task C()
        {   
            var privateKey = TestData.GetPrivateKey();   

            var client = new AcmeClient(
                privateKey   :  privateKey,
                accountUrl   : "https://acme-staging-v02.api.letsencrypt.org/acme/acct/5363173",
                directoryUrl : "https://acme-staging-v02.api.letsencrypt.org/directory"
            );

            var request = new CreateOrderRequest(new[] {
                new Identifier("dns", "test.accelerator.net")
            }, null, null);

            var order = await client.CreateOrderAsync(request);

            throw new Exception(JsonSerializer.Serialize(new { order, order.Url }));
        }


        [Fact]
        public async Task GetNonce()
        {
            var privateKey = TestData.GetPrivateKey();

            var client = new AcmeClient(privateKey, directoryUrl: "https://acme-staging-v02.api.letsencrypt.org/directory");

            await client.InitializeAsync();

            var nonce = await client.GetNonceAsync();

            await Task.Delay(1100);

            Assert.True(nonce.Age.TotalSeconds > 1);
            Assert.NotNull(nonce.Value);
        }
    }
}
