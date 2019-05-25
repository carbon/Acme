using System;
using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public sealed class Identifier : IEquatable<Identifier>
    {
#nullable disable
        public Identifier() { }
#nullable enable

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
