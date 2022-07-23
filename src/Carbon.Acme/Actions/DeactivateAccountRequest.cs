#pragma warning disable CA1822 // Mark members as static

using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class DeactivateAccountRequest
{
    public DeactivateAccountRequest(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        Url = url;
    }

    [JsonIgnore]
    public string Url { get; }

    [JsonPropertyName("status")]
    public string Status => "deactivated";
}
