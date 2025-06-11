using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnxiousAnt.Json.Converters;

public sealed class NameValueCollectionConverter : JsonConverter<NameValueCollection>
{
    /// <inheritdoc />
    public override NameValueCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            return null;
        }

        var jsonElement = JsonElement.ParseValue(ref reader);
        var result = new NameValueCollection();
        foreach (var property in jsonElement.EnumerateObject())
        {
            foreach (var value in property.Value.EnumerateArray())
            {
                result.Add(
                    property.Name,
                    value.ValueKind == JsonValueKind.Null ? null : value.GetString()
                );
            }
        }

        return result;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, NameValueCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var name in value.AllKeys)
        {
            if (name is null)
            {
                continue;
            }

            var entryValues = value.GetValues(name);
            writer.WritePropertyName(name);
            if (entryValues is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartArray();
                foreach (var current in entryValues)
                {
                    writer.WriteStringValue(current);
                }

                writer.WriteEndArray();
            }
        }

        writer.WriteEndObject();
    }
}