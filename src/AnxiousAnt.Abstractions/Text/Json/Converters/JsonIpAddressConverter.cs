using System.Net;
using System.Text.Json.Serialization;

namespace AnxiousAnt.Text.Json.Converters;

/// <summary>
/// A JSON converter for serializing and deserializing <see cref="IPAddress"/> objects.
/// </summary>
/// <remarks>
/// This converter handles both serialization and deserialization of <see cref="IPAddress"/> objects.
/// During the deserialization process, it expects IP address strings in a valid format
/// and converts them back into <see cref="IPAddress"/> instances.
/// During the serialization process, it converts <see cref="IPAddress"/> instances into their string representations.
/// </remarks>
public sealed class JsonIpAddressConverter : JsonConverter<IPAddress>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    /// <inheritdoc />
    public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String when reader.GetString() is { } ipString:
                return IPAddress.Parse(ipString);
        }

        throw new JsonException();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IPAddress? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}