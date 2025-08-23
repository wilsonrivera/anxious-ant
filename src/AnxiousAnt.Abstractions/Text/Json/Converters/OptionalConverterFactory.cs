using System.Text.Json.Serialization;

namespace AnxiousAnt.Text.Json.Converters;

/// <summary>
/// Represents JSON converter for <see cref="Optional{T}"/> data type.
/// </summary>
/// <remarks>
/// For AOT and self-contained app deployment models, use <see cref="OptionalConverter{T}"/> converter explicitly
/// as an argument for <see cref="JsonConverterAttribute"/>.
/// </remarks>
[RequiresDynamicCode("Runtime binding requires dynamic code compilation.")]
[RequiresUnreferencedCode("This type instantiates OptionalConverter<T> dynamically. Use OptionalConverter<T> instead.")]
public sealed class OptionalConverterFactory : JsonConverterFactory
{
    private static readonly Type OptionalConverterType = typeof(OptionalConverter<>);

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        return Optional.IsOptional(typeToConvert);
    }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);
        ArgumentNullException.ThrowIfNull(options);

        var underlyingType = Optional.GetUnderlyingType(typeToConvert);
        return underlyingType is not null
            ? Activator.CreateInstance(OptionalConverterType.MakeGenericType(underlyingType)) as JsonConverter
            : null;
    }
}