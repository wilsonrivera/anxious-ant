using System.Text.Json.Serialization;

namespace AnxiousAnt;

[JsonConverter(typeof(UrlJsonConverter))]
partial class Url
{
    internal sealed class UrlJsonConverter : JsonConverter<Url>
    {
        /// <inheritdoc />
        public override Url? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String when reader.GetString() is { } urlString:
                    return Parse(urlString);
            }

            throw new JsonException();
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Url? value, JsonSerializerOptions options)
        {
            if (value is not null)
            {
                writer.WriteStringValue(value.ToString());
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}