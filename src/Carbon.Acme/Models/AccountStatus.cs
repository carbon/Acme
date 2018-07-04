namespace Carbon.Acme
{
    public enum AccountStatus : byte
    {
        Unknown     = 0,
        Valid       = 1,
        Deactivated = 2,
        Revoked     = 3
    }
    // Status: valid | deactivated | revoked
}
