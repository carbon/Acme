#nullable disable

using System.Runtime.Serialization;

namespace Carbon.Acme
{
    [DataContract]
    public class ValidationRecord
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "hostname")]
        public string Hostname { get; set; }

        [DataMember(Name = "port")]
        public int Port { get; set; }

        [DataMember(Name = "addressesResolved")]
        public string[] AddressesResolved { get; set; }

        [DataMember(Name = "addressUsed")]
        public string AddressUsed { get; set; }
    }
}