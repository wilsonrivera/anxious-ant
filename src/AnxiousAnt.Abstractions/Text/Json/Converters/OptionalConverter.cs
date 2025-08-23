using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AnxiousAnt.Text.Json.Converters;

/// <summary>
/// Represents JSON converter for <see cref="Optional{T}"/> data type.
/// </summary>
/// <typeparam name="T">The type of the value in <see cref="Optional{T}"/> container.</typeparam>
public sealed class OptionalConverter<T> : JsonConverter<Optional<T>>
    where T : notnull
{
    private static readonly Type ValueType = typeof(T);

    /// <inheritdoc />
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        JsonTypeInfo jsonTypeInfo;
        return Optional.OfNullable(
            reader.TokenType is JsonTokenType.Null
                ? default
                : (jsonTypeInfo = options.GetTypeInfo(ValueType)) is JsonTypeInfo<T> jsonTypeInfoOfT
                    ? JsonSerializer.Deserialize(ref reader, jsonTypeInfoOfT)
                    : (T?)JsonSerializer.Deserialize(ref reader, jsonTypeInfo)
        );
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        switch (value.ValueKind)
        {
            case Optional.UninitializedValueKind:
                throw new InvalidOperationException("Optional value has not been initialized.");
            case Optional.EmptyValueKind:
            case Optional.NullValueKind:
                writer.WriteNullValue();
                break;
            case Optional.NotEmptyValueKind:
                JsonTypeInfo jsonTypeInfo;
                if ((jsonTypeInfo = options.GetTypeInfo(ValueType)) is JsonTypeInfo<T> jsonTypeInfoOfT)
                {
                    JsonSerializer.Serialize(writer, value.Value, jsonTypeInfoOfT);
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.Value, jsonTypeInfo);
                }

                break;
            default:
                throw new InvalidOperationException($"Optional is in an invalid state: {value.ValueKind}.");
        }
    }
}