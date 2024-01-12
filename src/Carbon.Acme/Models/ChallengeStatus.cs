using System.Text.Json.Serialization;

namespace Carbon.Acme;

[JsonConverter(typeof(JsonStringEnumConverter<ChallengeStatus>))]
public enum ChallengeStatus
{
    Unknown    = 0,
    Pending    = 1,
    Processing = 2,
    Valid      = 3,
    Invalid    = 4
}

/*
            pending
               |
               | Receive
               | response
               V
           processing<-+
               |   |    | Server retry or
               |   |    | client retry request
               |   +----+
               |
               |
   Successful  |   Failed
   validation  |   validation
     +---------+---------+
     |                   |
     V                   V
   valid              invalid'
   
 
*/