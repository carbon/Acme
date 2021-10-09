using System.Text.Json;

using Carbon.Extensions;

namespace Carbon.Acme.Tests
{
    internal static class JsonHelper
    {
        public static string GetBase64UrlEncodedJson<T>(T instance)
        {
            byte[] utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(instance, JSO.IgnoreNullValues);

            return Base64Url.Encode(utf8Bytes);
        }
    }
}