using System.Text.Json.Serialization;

namespace AnxiousAnt.Json;

/// <summary>
/// Provides a centralized instance of <see cref="JsonSerializerOptions"/> that can be used throughout the application.
/// </summary>
/// <remarks>
/// This class serves as a single source of truth for JSON serialization settings, ensuring consistent behavior
/// when serializing and deserializing JSON data across different components. It defines a pre-configured, lazily
/// initialized set of default options, accessible via the <c>Default</c> field.
/// </remarks>
[ExcludeFromCodeCoverage]
public static class JsonOptions
{
    private static readonly Lazy<JsonSerializerOptions> LazyDefaultOptions = new(static () =>
    {
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            IncludeFields = false,
            IgnoreReadOnlyProperties = false,
            WriteIndented = false,
        };

        options.Converters.Add(new Converters.JsonIpAddressConverter());
        options.Converters.Add(new Converters.NameValueCollectionConverter());

        return options;
    });

    /// <summary>
    /// Provides a default set of options for JSON serialization and deserialization.
    /// </summary>
    public static readonly JsonSerializerOptions Default = LazyDefaultOptions.Value;
}