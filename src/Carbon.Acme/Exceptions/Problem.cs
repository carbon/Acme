#nullable disable

using System.Runtime.Serialization;

namespace Carbon.Acme.Exceptions
{
    [DataContract]
    public class Problem
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "detail")]
        public string Detail { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public int? Status { get; set; }

        [DataMember(Name = "instance", EmitDefaultValue = false)]
        public string Instance { get; set; }

        [DataMember(Name = "subproblems", EmitDefaultValue = false)]
        public Subproblem[] Subproblems { get; set; }
    }
}