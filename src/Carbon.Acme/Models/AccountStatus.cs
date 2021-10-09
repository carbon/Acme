using System.Text.Json.Serialization;

namespace Carbon.Acme;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountStatus
{
    Unknown     = 0,
    Valid       = 1,
    Deactivated = 2,
    Revoked     = 3
}

// Status: valid | deactivated | revoked