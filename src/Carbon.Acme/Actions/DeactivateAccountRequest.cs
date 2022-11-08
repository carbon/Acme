using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class DeactivateAccountRequest
{
    public DeactivateAccountRequest(string url)
    {
        ArgumentException.ThrowIfNullOrEmpty(url);

        Url = url;
    }

    [JsonIgnore]
    public string Url { get; }

    [JsonPropertyName("status")]
    public string Status => "deactivated";
}
