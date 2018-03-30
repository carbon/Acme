namespace Carbon.Acme
{
    public enum OrderStatus
    {
        Pending    = 1,
        Ready      = 2,
        Processing = 3,
        Valid      = 4,
        Invalid    = 5
    }
}

/*
     pending --------------+
       |                  |
       | All authz        |
       | "valid"          |
       V                  |
     ready ---------------+
       |                  |
       | Receive          |
       | finalize         |
       | request          |
       V                  |
   processing ------------+
       |                  |
       | Certificate      | Error or
       | issued           | Authorization failure
       V                  V
     valid             invalid

*/