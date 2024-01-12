using System.Text.Json;

using Carbon.Acme.Serialization;

namespace Carbon.Acme.Tests;

public class IdentifierTests
{
    [Fact]
    public void CanConstruct()
    {
        var identifier = new Identifier("dns", "abc.com");

        Assert.Equal("dns", identifier.Type);
        Assert.Equal("abc.com", identifier.Value);
    }

    [Fact]
    public void CanDeserialize()
    {
        var json = """{"type":"dns","value":"abc.com"}""";

        var identifier = JsonSerializer.Deserialize(json, AcmeSerializerContext.Default.Identifier);

        Assert.Equal("dns", identifier.Type);
        Assert.Equal("abc.com", identifier.Value);

        Assert.Equal(json, JsonSerializer.Serialize(identifier, AcmeSerializerContext.Default.Identifier));
    }
}