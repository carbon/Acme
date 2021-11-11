using System;
using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class CompleteChallengeRequest
{
    public CompleteChallengeRequest(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        Url = url;
    }

    [JsonIgnore]
    public string Url { get; }
}