using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class Identifier : IEquatable<Identifier>
    {
        public Identifier() { }

        public Identifier(string type, string value)
        {
            Type  = type  ?? throw new ArgumentNullException(nameof(type));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [DataMember(Name = "type", IsRequired = true)]
        public string Type { get; set; }

        [DataMember(Name = "value", IsRequired = true)]
        public string Value { get; set; }

        #region IEquatable

        public bool Equals(Identifier other) =>
            Type == other.Type &&
            Value == other.Value;

        #endregion
    }
}
