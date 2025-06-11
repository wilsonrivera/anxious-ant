using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace AnxiousAnt.Json;

[JsonSerializable(typeof(string))]
internal sealed partial class InternalJsonSerializerContext : JsonSerializerContext
{
    static InternalJsonSerializerContext()
    {
        s_defaultOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    }
}