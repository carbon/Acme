using System.Security.Cryptography.X509Certificates;

namespace Carbon.Acme.Tests
{
    public partial class CertificateChainTests
    {
        [Fact]
        public void Reader()
        {
            var reader = new CertificateChainReader(TestData.CertificateChain);

            var pems = new List<byte[]>();

            while (reader.TryReadNext(out var der))
            {
                pems.Add(der);
            }

            Assert.Equal(1516, pems[0].Length); // Cert
            Assert.Equal(1199, pems[1].Length); // Root

            Assert.Equal(2, pems.Count);

            Assert.Equal("CN=acmev2.wulf.eu.org", new X509Certificate2(pems[0]).Subject);
            Assert.Equal("CN=Fake LE Intermediate X1", new X509Certificate2(pems[1]).Subject);

            Assert.Equal("CB10335FBFBBFDFC359A76559B46F0A4BA221736", new X509Certificate2(pems[0]).Thumbprint);
            Assert.Equal("4EEE7398C1A3DAF91DA16689DB8243927A271B9A", new X509Certificate2(pems[1]).Thumbprint);

            Assert.Equal(pems[0], new X509Certificate2(pems[0]).RawData);
        }
    }
}
