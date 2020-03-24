using System.Text.Json;

using Carbon.Acme.Exceptions;

using Xunit;

namespace Carbon.Acme.Tests
{
    public class ProblemTests
    {
        [Fact]
        public void A()
        {
            var model = JsonSerializer.Deserialize<Problem>(@"{
  ""type"": ""urn:ietf:params:acme:error:unauthorized"",
  ""detail"": ""No authorization provided for name example.net""
}");

            Assert.Equal("urn:ietf:params:acme:error:unauthorized",         model.Type);
            Assert.Equal("No authorization provided for name example.net",  model.Detail);
        }

        [Fact]
        public void CanParseNestedSubproblems()
        {
            string json = @"{
    ""type"": ""urn:ietf:params:acme:error:malformed"",
    ""detail"": ""Some of the identifiers requested were rejected"",
    ""subproblems"": [
        {
            ""type"": ""urn:ietf:params:acme:error:malformed"",
            ""detail"": ""Invalid underscore in DNS name \""_example.com\"""",
            ""identifier"": {
                ""type"": ""dns"",
                ""value"": ""_example.com""
            }
        },
        {
            ""type"": ""urn:ietf:params:acme:error:rejectedIdentifier"",
            ""detail"": ""This CA will not issue for \""example.net\"""",
            ""identifier"": {
                ""type"": ""dns"",
                ""value"": ""example.net""
            }
        }
    ]
}";
            var model = JsonSerializer.Deserialize<Problem>(json);

            Assert.Equal("urn:ietf:params:acme:error:malformed",            model.Type);
            Assert.Equal("Some of the identifiers requested were rejected", model.Detail);

            Assert.Equal(2, model.Subproblems.Length);
            Assert.Equal("urn:ietf:params:acme:error:malformed",            model.Subproblems[0].Type);
            Assert.Equal(new Identifier("dns", "_example.com"),             model.Subproblems[0].Identifier);

            Assert.Equal("urn:ietf:params:acme:error:rejectedIdentifier",    model.Subproblems[1].Type);
            
        }
    }
}