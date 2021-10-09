using System;
using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class CompleteChallengeRequest
{
    public CompleteChallengeRequest(string url)
    {
        Url = url ?? throw new ArgumentNullException(nameof(url));
    }

    [JsonIgnore]
    public string Url { get; }
}