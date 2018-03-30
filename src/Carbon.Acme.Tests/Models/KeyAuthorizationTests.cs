using System.Security.Cryptography;
using Carbon.Pkcs;
using Xunit;

namespace Carbon.Acme.Tests
{
    public class KeyAuthorizationTests
    {
        [Fact]
        public void A()
        {
            var client = new AcmeClient(RSA.Create(RSAPrivateKey.Decode(TestData.PrivateRSA256KeyText)));

            Assert.Equal("token.aWqHvejQhMmUunCZtp_2yV_bOOR0DEdpDRqn8VgjjYY", client.GetKeyAuthorization("token"));
        }
    }
}