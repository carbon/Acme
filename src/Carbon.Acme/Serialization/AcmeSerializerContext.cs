using System.Text.Json.Serialization;

using Carbon.Acme.Exceptions;

namespace Carbon.Acme.Serialization;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Account))]
[JsonSerializable(typeof(Authorization))]
[JsonSerializable(typeof(Directory))]
[JsonSerializable(typeof(DirectoryMetadata))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(OrderList))]
[JsonSerializable(typeof(Problem))]
[JsonSerializable(typeof(AcmeMessageHeader))]
public partial class AcmeSerializerContext : JsonSerializerContext
{
}