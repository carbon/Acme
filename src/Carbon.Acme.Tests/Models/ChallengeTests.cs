﻿using System.Text.Json;

using Carbon.Acme.Serialization;

namespace Carbon.Acme.Tests;

public class ChallengeTests
{
    [Fact]
    public void CanDeserialize_Dns01()
    {
        Challenge model = JsonSerializer.Deserialize(
            """
            {
              "type": "dns-01",
              "url": "https://example.com/acme/authz/1234/2",
              "status": "pending",
              "token": "evaGxfADs6pSRb2LAv9IZf17Dt3juxGJ-PCt92wr-oA"
            }
            """, AcmeSerializerContext.Default.Challenge);

        Assert.Equal("dns-01",                                      model.Type);
        Assert.Equal("https://example.com/acme/authz/1234/2",       model.Url);
        Assert.Equal(ChallengeStatus.Pending,                       model.Status);
        Assert.Equal("evaGxfADs6pSRb2LAv9IZf17Dt3juxGJ-PCt92wr-oA", model.Token);
    }

    [Fact]
    public void CanDeserialize_Http01()
    {
        Challenge model = JsonSerializer.Deserialize(
            """
            {
              "type": "http-01",
              "url": "https://example.com/acme/authz/0",
              "status": "valid",
              "token": "LoqXcYV8q5ONbJQxbmR7SCTNo3tiAXDfowyjxAjEuX0"
            }
            """, AcmeSerializerContext.Default.Challenge);

        Assert.Equal("http-01", model.Type);
        Assert.Equal("https://example.com/acme/authz/0", model.Url);
        Assert.Equal(ChallengeStatus.Valid, model.Status);
        Assert.Equal("LoqXcYV8q5ONbJQxbmR7SCTNo3tiAXDfowyjxAjEuX0", model.Token);
    }

    [Fact]
    public void CanDeserializeError()
    {
        Challenge model = JsonSerializer.Deserialize(
            """
            {
              "type": "dns-01",
              "status": "invalid",
              "error": {
                "type": "urn:ietf:params:acme:error:unauthorized",
                "detail": "No TXT record found at _acme-challenge.x.net",
                "status": 403
              },
              "url": "https://acme-v02.api.letsencrypt.org/acme/challenge/x/x",
              "token": "x"
            }
            """, AcmeSerializerContext.Default.Challenge);

        Assert.Equal("dns-01", model.Type);
        Assert.Equal(ChallengeStatus.Invalid, model.Status);
        Assert.Equal("urn:ietf:params:acme:error:unauthorized", model.Error.Type);
        Assert.Equal("No TXT record found at _acme-challenge.x.net", model.Error.Detail);
        Assert.Equal(403, model.Error.Status);
    }
}