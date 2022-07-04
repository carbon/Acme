namespace Carbon.Acme.Tests;

public class CompleteChallengeRequestTests
{
    [Fact]
    public void CanConstruct()
    {
        var request = new CompleteChallengeRequest("https://test.com");
        
        Assert.Equal("https://test.com", request.Url);
    }
}