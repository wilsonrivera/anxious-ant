namespace AnxiousAnt;

partial class StringExtensions
{
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
}