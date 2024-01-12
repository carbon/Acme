using System.Text.Json;

using Carbon.Acme.Serialization;

namespace Carbon.Acme.Tests;

public class DirectoryTests
{
    [Fact]
    public void CanDeserialize()
    {
        Directory directory = JsonSerializer.Deserialize(
            """
            {
              "newNonce": "https://example.com/acme/new-nonce",
              "newAccount": "https://example.com/acme/new-account",
              "newOrder": "https://example.com/acme/new-order",
              "newAuthz": "https://example.com/acme/new-authz",
              "revokeCert": "https://example.com/acme/revoke-cert",
              "keyChange": "https://example.com/acme/key-change",
              "meta": {
                "termsOfService": "https://example.com/acme/terms/2017-5-30",
                "website": "https://www.example.com/",
                "caaIdentities": ["example.com"],
                "externalAccountRequired": false
                }
            }
            """, AcmeSerializerContext.Default.Directory);
        
        Assert.Equal("https://example.com/acme/new-nonce",   directory.NewNonceUrl);
        Assert.Equal("https://example.com/acme/new-authz",   directory.NewAuthorizationUrl);
        Assert.Equal("https://example.com/acme/revoke-cert", directory.RevokeCertificateUrl);
        Assert.Equal("https://example.com/acme/key-change",  directory.KeyChangeUrl);

        var meta = directory.Meta;

        Assert.Equal("https://example.com/acme/terms/2017-5-30", meta.TermsOfService);
        Assert.Equal("https://www.example.com/",                 meta.Website);
        Assert.Equal("example.com",                              meta.CaaIdentities[0]);
        Assert.False(meta.ExternalAccountRequired);
    }
}