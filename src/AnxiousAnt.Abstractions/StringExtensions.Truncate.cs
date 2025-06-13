namespace AnxiousAnt;

partial class StringExtensions
{
    private const string DefaultTruncateOmissionIndicator = "…";

    /// <summary>
    /// Truncates the specified string and appends the omission indicator while at the same time, guaranteeing
    /// that the resulting string never exceeds the specified maximum length.
    /// </summary>
    /// <param name="s">The string to be truncated.</param>
    /// <param name="maximumLength">The maximum character length. If zero or negative, the string is not truncated.</param>
    /// <param name="omissionIndicator">The string to append to indicate truncation, such as an ellipsis.</param>
    /// <returns>
    /// A string that has been truncated and includes the omission indicator if truncation occurs, or the original
    /// string if it is shorter than the maximum length.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maximumLength"/> is negative or zero.</exception>
    [Pure]
    public static string Truncate(
        this string? s,
        int maximumLength,
        string? omissionIndicator = DefaultTruncateOmissionIndicator)
        => string.IsNullOrEmpty(s)
            ? string.Empty
            : Truncate(s.AsSpan().Trim(WhiteSpaces.Chars), maximumLength, omissionIndicator);

    /// <summary>
    /// Truncates a string and appends the omission indicator, ensuring the result does not exceed the
    /// specified maximum length.
    /// </summary>
    /// <param name="s">The string to be truncated.</param>
    /// <param name="maximumLength">The maximum character length.</param>
    /// <param name="omissionIndicator">The string to append to indicate truncation, such as an ellipsis.</param>
    /// <returns>
    /// A string that has been truncated and includes the omission indicator if truncation occurs, or the original
    /// string if it is shorter than the maximum length.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maximumLength"/> is negative or zero.</exception>
    [Pure]
    public static string Truncate(
        this ReadOnlySpan<char> s,
        int maximumLength,
        string? omissionIndicator = DefaultTruncateOmissionIndicator)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumLength);
        if (s.IsWhiteSpace())
        {
            return string.Empty;
        }

        omissionIndicator ??= DefaultTruncateOmissionIndicator;
        var omissionIndicatorLength = omissionIndicator.Length;
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumLength, omissionIndicatorLength + 1);

        if (maximumLength >= s.Length)
        {
            return s.ToString();
        }

        var indexOfFinalChar = maximumLength - omissionIndicatorLength;
        var result = s[..indexOfFinalChar]
            .TrimEnd(WhiteSpaces.Chars)
            .TrimEnd('.');

        return string.Concat(result, omissionIndicator);
    }
}