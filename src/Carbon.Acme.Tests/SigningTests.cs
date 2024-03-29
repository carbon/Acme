﻿using Carbon.Jose;

namespace Carbon.Acme.Tests;

public class SigningTests
{
    [Fact]
    public void A()
    {
        var messageHeader = GetMessageHeader("https://test.com", new Nonce("234", DateTime.UtcNow));

        Assert.Equal("eyJhbGciOiJSUzI1NiIsIm5vbmNlIjoiMjM0IiwidXJsIjoiaHR0cHM6Ly90ZXN0LmNvbSIsImp3ayI6eyJraWQiOiJraWQifX0", JsonHelper.GetBase64UrlEncodedJson(messageHeader));
    }

    private static Dictionary<string, object> GetMessageHeader(string url, in Nonce nonce)
    {
        var header = new Dictionary<string, object>(5) {
            { "alg",   AlgNames.RS256 },
            { "nonce", nonce.Value },
            { "url",   url }
        };

        var jwk = new Jwk { KeyId = "kid" };

        header.Add("jwk", jwk);

        return header;
    }
}