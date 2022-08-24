using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class DeactivateAuthorizationRequest
{
    public DeactivateAuthorizationRequest(string url)
    {
        ArgumentNullException.ThrowIfNull(Url);

        Url = url;
    }

    [JsonIgnore]
    public string Url { get; }

    [JsonPropertyName("status")]
    public string Status => "deactivated";
}