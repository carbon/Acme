using Xunit;

namespace Carbon.Acme.Tests
{
    public class CompleteChallengeRequestTests
    {
        [Fact]
        public void Construct()
        {
            var action = new CompleteChallengeRequest("url", "123");
            
            Assert.Equal("url", action.Url);
            Assert.Equal("123", action.KeyAuthorization);
        }
    }
}