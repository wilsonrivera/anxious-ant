using System.Text.Json.Serialization;

namespace AnxiousAnt;

[JsonConverter(typeof(EmailAddressJsonConverter))]
partial struct EmailAddress
{
    internal sealed class EmailAddressJsonConverter : JsonConverter<EmailAddress>
    {
        /// <inheritdoc />
        public override EmailAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return Empty;
                case JsonTokenType.String when reader.GetString() is { } emailString:
                    return Parse(emailString);
            }

            throw new JsonException();
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, EmailAddress value, JsonSerializerOptions options)
        {
            if (value._toString is not { } emailStringValue)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(emailStringValue);
            }
        }
    }
}