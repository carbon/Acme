using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Carbon.Json;
using Carbon.Pkcs;
using Xunit;

namespace Carbon.Acme.Tests
{
    public class ClientTests
    {
        // [Fact]
        public async Task B()
        {
            var privateKey = RSA.Create(RSAPrivateKey.Decode(TestData.PrivateRSA256KeyText));
            
            var client = new AcmeClient(privateKey, directoryUrl: "https://acme-staging-v02.api.letsencrypt.org/directory");
            
            var accountUrl = await client.CreateAccountAsync(new CreateAccountRequest(termsOfServiceAgreed: true, new[] {
                "mailto:test@processor.ai"
            }, false));

            throw new Exception(JsonObject.FromObject(new { accountUrl }).ToString());
        }

        // [Fact]
        public async Task D()
        {
            var privateKey = RSA.Create(RSAPrivateKey.Decode(TestData.PrivateRSA256KeyText));

            var client = new AcmeClient(privateKey, directoryUrl: "https://acme-staging-v02.api.letsencrypt.org/directory");

            var accountUrl = await client.GetAccountUrlAsync();

            throw new Exception(JsonObject.FromObject(new { accountUrl }).ToString());
        }

        // [Fact]
        public async Task C()
        {            
            var client = new AcmeClient(
                privateKey   : RSA.Create(RSAPrivateKey.Decode(TestData.PrivateRSA256KeyText)),
                accountUrl   : "https://acme-staging-v02.api.letsencrypt.org/acme/acct/5363173",
                directoryUrl : "https://acme-staging-v02.api.letsencrypt.org/directory"
            );

            var request = new CreateOrderRequest(new[] {
                new Identifier("dns", "test.accelerator.net")
            }, null, null);

            var order = await client.CreateOrderAsync(request);

            throw new Exception(JsonObject.FromObject(new { order, order.Url }).ToString());
        }


        [Fact]
        public async Task GetNonce()
        {
            var privateKey = RSA.Create(RSAPrivateKey.Decode(TestData.PrivateRSA256KeyText));

            var client = new AcmeClient(privateKey, directoryUrl: "https://acme-staging-v02.api.letsencrypt.org/directory");

            await client.InitializeAsync();

            var nonce = await client.GetNonceAsync();

            await Task.Delay(1100);

            Assert.True(nonce.Age.TotalSeconds > 1);
            Assert.NotNull(nonce.Value);
        }
    }
}
