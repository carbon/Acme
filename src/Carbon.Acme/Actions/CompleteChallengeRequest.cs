using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class CompleteChallengeRequest
    {
        public CompleteChallengeRequest(string url, string keyAuthorization)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            KeyAuthorization = keyAuthorization ?? throw new ArgumentNullException(nameof(keyAuthorization));
        }

        [IgnoreDataMember]
        public string Url { get; }

        [DataMember(Name = "keyAuthorization", IsRequired = true)]
        public string KeyAuthorization { get; }
    }
}