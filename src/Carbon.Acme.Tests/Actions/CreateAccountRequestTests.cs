namespace Carbon.Acme.Tests;

public class CreateAccountRequestTests
{
    [Fact]
    public void CanConstruct()
    {
        var request = new CreateAccountRequest(true, [ "mailto:test@test.com" ], false);

        Assert.True(request.TermsOfServiceAgreed);
        Assert.Equal("mailto:test@test.com", request.Contact[0]);
    }
}