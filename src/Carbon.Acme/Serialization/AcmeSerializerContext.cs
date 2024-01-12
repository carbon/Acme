using System.Text.Json.Serialization;

using Carbon.Acme.Exceptions;

namespace Carbon.Acme.Serialization;

[JsonSerializable(typeof(Account))]
[JsonSerializable(typeof(Authorization))]
[JsonSerializable(typeof(Directory))]
[JsonSerializable(typeof(DirectoryMetadata))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(OrderList))]
[JsonSerializable(typeof(Problem))]
public partial class AcmeSerializerContext : JsonSerializerContext
{
}