using System.Text.Json;

using Xunit;

namespace Carbon.Acme.Tests
{
    public class IdentifierTests
    {
        [Fact]
        public void Construct()
        {
            var identifier = new Identifier("dns", "abc.com");

            Assert.Equal("dns", identifier.Type);
            Assert.Equal("abc.com", identifier.Value);

            Assert.Equal("dns", JsonSerializer.Deserialize <Identifier>(JsonSerializer.Serialize(identifier)).Type);
        }
    }
}