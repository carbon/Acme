using System.Text.Json.Serialization;

namespace Carbon.Acme
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuthorizationStatus
    {
        Pending     = 1,
        Valid       = 2,
        Invalid     = 3,
        Deactivated = 4,
        Expired     = 5,
        Revoked     = 6
    }
}


/*
             pending --------------------+
                |                        |
                |                        |
    Error       |  Challenge valid       |
      +---------+---------+              |
      |                   |              |
      V                   V              |
   invalid              valid            |
                          |              |
                          |              |
                          |              |
           +--------------+--------------+
           |              |              |
           |              |              |
    Server |       Client |   Time after |
    revoke |   deactivate |    "expires" |
           V              V              V
        revoked      deactivated      expired

*/