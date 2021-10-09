namespace Carbon.Acme.Tests
{
    public class CreateAccountRequestTests
    {
        [Fact]
        public void Construct()
        {
            var action = new CreateAccountRequest(true, new[] { "mailto:test@test.com" }, false);

            Assert.True(action.TermsOfServiceAgreed);
            Assert.Equal("mailto:test@test.com", action.Contact[0]);
        }
    }
}