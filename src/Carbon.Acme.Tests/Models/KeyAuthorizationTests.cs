namespace Carbon.Acme.Tests;

public class KeyAuthorizationTests
{
    [Fact]
    public void ComputeKeyAuthorization()
    {
        var client = new AcmeClient(TestData.GetPrivateKey());

        Assert.Equal("token.aWqHvejQhMmUunCZtp_2yV_bOOR0DEdpDRqn8VgjjYY", client.GetKeyAuthorization("token"));
    }

    [Fact]
    public void GetBase64UrlEncodedKeyAuthorizationSha256Digest()
    {
        var client = new AcmeClient(TestData.GetPrivateKey());

        Assert.Equal("aHvlxDWdGMGph9aDnvqKGyO1b2Zfz4rH7VyX9F8xelY", client.GetBase64UrlEncodedKeyAuthorizationSha256Digest("token"));
    }
}