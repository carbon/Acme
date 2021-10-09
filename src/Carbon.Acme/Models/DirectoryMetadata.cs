using System.Text.Json.Serialization;

namespace Carbon.Acme;

public sealed class DirectoryMetadata
{
    /// <summary>
    /// A URL identifying the current terms of service.
    /// </summary>
    [JsonPropertyName("termsOfService")]
    public string? TermsOfService { get; init; }

    /// <summary>
    /// An HTTP or HTTPS URL locating a website providing 
    /// more information about the ACME server.
    /// </summary>
    [JsonPropertyName("website")]
    public string? Website { get; init; }

    [JsonPropertyName("caaIdentities")]
    public string[]? CaaIdentities { get; init; }

    [JsonPropertyName("externalAccountRequired")]
    public bool? ExternalAccountRequired { get; init; }
}