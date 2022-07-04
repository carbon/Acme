using System.Text.Json;

namespace Carbon.Acme.Tests;

public class AuthorizationTest
{
    [Fact]
    public void StatusCodes()
    {
        Assert.Equal(1, (int)AuthorizationStatus.Pending);
        Assert.Equal(2, (int)AuthorizationStatus.Valid);
        Assert.Equal(3, (int)AuthorizationStatus.Invalid);
        Assert.Equal(4, (int)AuthorizationStatus.Deactivated);
        Assert.Equal(5, (int)AuthorizationStatus.Expired);
        Assert.Equal(6, (int)AuthorizationStatus.Revoked);
    }

    [Fact]
    public void CanDeserialize()
    {
        var model = JsonSerializer.Deserialize<Authorization>("""
            {
              "status": "valid",
              "expires": "2015-03-01T14:09:00Z",
              "identifier": {
                "type": "dns",
                "value": "example.org"
              },
              "challenges": [
                {
                  "url": "https://example.com/authz/1234/0",
                  "type": "http-01",
                  "status": "valid",
                  "token": "DGyRejmCefe7v4NfDGDKfA",
                  "validated": "2014-12-01T12:05:00Z",
                  "keyAuthorization": "SXQe-2XODaDxNR...vb29HhjjLPSggwiE"
                }
              ],
              "wildcard": false
            }
            """);
        
        Assert.Equal("2015-03-01T14:09:00.0000000Z", model.Expires.Value.ToString("o"));

        Assert.Equal(AuthorizationStatus.Valid, model.Status);

        Assert.Equal(new Identifier("dns", "example.org"), model.Identifier);

        Assert.False(model.Wildcard.Value);

        var challenge = model.Challenges[0];

        Assert.Equal("http-01", challenge.Type);

        Assert.Equal("https://example.com/authz/1234/0", challenge.Url);
        Assert.Equal(ChallengeStatus.Valid,  challenge.Status);
        Assert.Equal("DGyRejmCefe7v4NfDGDKfA", challenge.Token);
    }
}