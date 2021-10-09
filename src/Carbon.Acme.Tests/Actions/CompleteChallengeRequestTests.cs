namespace Carbon.Acme.Tests
{
    public class CompleteChallengeRequestTests
    {
        [Fact]
        public void Construct()
        {
            var action = new CompleteChallengeRequest("https://test.com");
            
            Assert.Equal("https://test.com", action.Url);
        }
    }
}