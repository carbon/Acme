using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class DeactivateAuthorizationRequest
    {
        public DeactivateAuthorizationRequest(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        [IgnoreDataMember]
        public string Url { get; }

        [DataMember(Name = "status")]
        public string Status => "deactivated";
    }
}
