using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;

using AnxiousAnt.Json;

namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for the <see cref="string"/> class.
/// </summary>
public static class StringExtensions
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
    [return: NotNullIfNotNull(nameof(s))]
    public static string? JsonUnescape(this string? s) =>
        string.IsNullOrWhiteSpace(s)
            ? s
            : s is "null"
                ? null
                : JsonSerializer.Deserialize($"\"{s}\"", LazyStringJsonTypeInfo.Value);

    /// <summary>
    /// Determines whether two specified strings are equal using a specific comparison option for case sensitivity.
    /// </summary>
    /// <param name="s">The first string to compare, or <c>null</c>.</param>
    /// <param name="value">The second string to compare, or <c>null</c>.</param>
    /// <param name="ignoreCase">If set to <c>true</c>, the comparison ignores case; otherwise, it considers case.</param>
    /// <returns>
    /// <c>true</c> if the value of the first string is equal to the value of the second string; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool OrdinalEquals(
        [NotNullIfNotNull(nameof(value))] this string? s,
        [NotNullIfNotNull(nameof(s))] string? value,
        bool ignoreCase = false) =>
        s is null
            ? value is null
            : value is not null &&
              (
                  ReferenceEquals(s, value) ||
                  (string.IsNullOrWhiteSpace(s) && string.IsNullOrWhiteSpace(value) && s.Length == value.Length) ||
                  string.Equals(s, value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
              );

    /// <summary>
    /// Determines whether the specified character occurs within the string, using an ordinal comparison.
    /// </summary>
    /// <param name="s">The string to search within, or <c>null</c>.</param>
    /// <param name="value">The character to seek within <paramref name="s"/>.</param>
    /// <param name="ignoreCase">If set to <c>true</c>, the comparison ignores case; otherwise, it considers case.</param>
    /// <returns>
    /// <c>true</c> if the character occurs within the string; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool OrdinalContains([NotNullWhen(true)] this string? s, char value, bool ignoreCase = false) =>
        s is not null &&
        s.Contains(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the specified substring occurs within this string using a specific comparison
    /// option for case sensitivity.
    /// </summary>
    /// <param name="s">The string to search, or <c>null</c>.</param>
    /// <param name="value">The substring to locate within the main string, or <c>null</c>.</param>
    /// <param name="ignoreCase">If set to <c>true</c>, the comparison ignores case; otherwise, it considers case.</param>
    /// <returns>
    /// <c>true</c> if the value of the substring is found within the main string; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool OrdinalContains(
        [NotNullWhen(true)] this string? s,
        [NotNullWhen(true)] string? value,
        bool ignoreCase = false) =>
        s is not null &&
        value is not null &&
        s.Contains(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the beginning of a specified string instance matches a specified string using a
    /// specific comparison option for case sensitivity.
    /// </summary>
    /// <param name="s">The string to compare.</param>
    /// <param name="value">The string to compare to the substring at the beginning of this instance.</param>
    /// <param name="ignoreCase">If set to <c>true</c>, the comparison ignores case; otherwise, it considers case.</param>
    /// <returns>
    /// <c>true</c> if the beginning of this instance matches the specified string; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool OrdinalStartsWith(
        [NotNullWhen(true)] this string? s,
        [NotNullWhen(true)] string? value,
        bool ignoreCase = false) =>
        s is not null &&
        value is not null &&
        s.StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the end of this string instance matches a specified string when compared using
    /// ordinal comparison rules.
    /// </summary>
    /// <param name="s">The input string to check.</param>
    /// <param name="value">The string to compare to the substring at the end of this instance.</param>
    /// <param name="ignoreCase">If true, the comparison ignores case; otherwise, the comparison considers case.</param>
    /// <returns>
    /// <c>true</c> if the value parameter matches the end of this string; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool OrdinalEndsWith(
        [NotNullWhen(true)] this string? s,
        [NotNullWhen(true)] string? value,
        bool ignoreCase = false) =>
        s is not null &&
        value is not null &&
        s.EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

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