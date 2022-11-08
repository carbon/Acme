using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class CompleteChallengeRequest
{
    public CompleteChallengeRequest(string url)
    {
        ArgumentException.ThrowIfNullOrEmpty(url);

        Url = url;
    }

    [JsonIgnore]
    public string Url { get; }
}