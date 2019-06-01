using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public sealed class CompleteChallengeRequest
    {
        public CompleteChallengeRequest(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        [IgnoreDataMember]
        public string Url { get; }
    }
}