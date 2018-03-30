using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class UpdateAccountRequest
    {
        [DataMember(Name = "contact", EmitDefaultValue = false)]
        public string[] Contact { get; set; }
    }
}