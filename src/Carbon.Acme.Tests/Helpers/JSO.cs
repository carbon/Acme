using System.Text.Json;
using System.Text.Json.Serialization;

namespace Carbon.Acme.Tests;

internal static class JSO
{
    public static readonly JsonSerializerOptions IgnoreNullValues = new () { 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static readonly JsonSerializerOptions Pretty = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };


    public static readonly JsonSerializerOptions Indented = new() {
        WriteIndented = true
    };
}