using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;

using AnxiousAnt.Text.Json;

namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for the <see cref="string"/> class.
/// </summary>
public static partial class StringExtensions
{
    private static readonly Lazy<JsonTypeInfo<string>> LazyStringJsonTypeInfo = new(static () =>
    {
        JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            TypeInfoResolver = InternalJsonSerializerContext.Default
        };

        return JsonMetadataServices.CreateValueInfo<string>(options, JsonMetadataServices.StringConverter);
    });

    internal static readonly SpanAction<char, char[]> WriteToStringMemorySpanAction =
        // ReSharper disable once ConvertClosureToMethodGroup
        (span, chars) => WriteToStringMemory(span, chars);

    /// <summary>
    /// Returns the current <see cref="string"/> if it is not <c>null</c>, empty, or whitespace; otherwise, it
    /// returns the fallback value provided.
    /// </summary>
    /// <param name="value">The primary string to check.</param>
    /// <param name="fallbackValue">The fallback string to return if the primary string is <c>null</c>, empty, or whitespace.</param>
    /// <returns>
    /// The original string if not <c>null</c>, empty, or whitespace; otherwise, the fallback string.
    /// </returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(value))]
    [return: NotNullIfNotNull(nameof(fallbackValue))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? IfNullOrWhiteSpace(this string? value, string? fallbackValue) =>
        string.IsNullOrWhiteSpace(value) ? fallbackValue : value;

    /// <summary>
    /// Escapes a string for use in JSON encoding by serializing it and trimming surrounding quotes.
    /// </summary>
    /// <param name="s">The input string to escape. If <c>null</c>, an empty string is returned.</param>
    /// <returns>
    /// A JSON-escaped string with the quotes removed, or an empty string if the input is <c>null</c>.
    /// </returns>
    [Pure]
    public static string JsonEscape(this string? s)
    {
        if (s is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(s))
        {
            return s;
        }

        var escaped = JsonSerializer.Serialize(s, LazyStringJsonTypeInfo.Value);
        escaped = escaped[1..^1];

        return escaped;
    }

    /// <summary>
    /// Unescapes a JSON-escaped string, deserializing it to its original form.
    /// </summary>
    /// <param name="s">The input JSON-escaped string to unescape.</param>
    /// <returns>
    /// The unescaped string, <c>null</c> if the input string is "null", or the original input string if it is
    /// empty or whitespace.
    /// </returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(s))]
    public static string? JsonUnescape(this string? s) =>
        string.IsNullOrWhiteSpace(s)
            ? s
            : s is "null"
                ? null
                : JsonSerializer.Deserialize($"\"{s}\"", LazyStringJsonTypeInfo.Value);

    /// <summary>
    /// Converts the given string to its lowercase invariant form if it contains any uppercase characters.
    /// If the string is already lowercase, empty, or whitespace, it is returned as is.
    /// </summary>
    /// <param name="s">The input string to convert to lowercase.</param>
    /// <returns>
    /// A lowercase invariant version of the input string if it contains uppercase characters; otherwise,
    /// the original string if it is already lowercase, empty, or consists of whitespace only.
    /// </returns>
    [Pure]
    public static string AsLowerInvariant(this string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(s))
        {
            return s;
        }

        var span = s.AsSpan();
        var buffer = ArrayPool<char>.Shared.Rent(span.Length);
        try
        {
            var upperCaseCharacterWasReplaced = false;
            ref var searchSpace = ref MemoryMarshal.GetReference(span);
            for (var i = 0; i < span.Length; i++)
            {
                ref var chr = ref Unsafe.Add(ref searchSpace, i);
                if (char.IsUpper(chr))
                {
                    buffer[i] = char.ToLowerInvariant(chr);
                    upperCaseCharacterWasReplaced = true;
                }
                else
                {
                    buffer[i] = chr;
                }
            }

            return !upperCaseCharacterWasReplaced
                ? s
                : string.Create(span.Length, buffer, WriteToStringMemorySpanAction);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static void WriteToStringMemory(Span<char> span, char[] chars)
    {
        ref var searchSpace = ref MemoryMarshal.GetReference(chars.AsSpan());
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Unsafe.Add(ref searchSpace, i);
        }
    }
}