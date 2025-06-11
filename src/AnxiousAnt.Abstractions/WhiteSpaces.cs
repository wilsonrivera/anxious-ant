namespace AnxiousAnt;

/// <summary>
/// Provides constants and utilities for handling whitespace characters.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class WhiteSpaces
{
    /// <summary>
    /// Represents the ASCII value of the space character as a byte.
    /// </summary>
    public const byte Byte = (byte)' ';

    /// <summary>
    /// Represents the ASCII value of the space character as a char.
    /// </summary>
    public const char Char = ' ';

    /// <summary>
    /// A collection of characters considered as whitespaces in this context.
    /// </summary>
    /// <remarks>
    /// Includes standard whitespaces and non-traditional whitespace characters.
    /// </remarks>
    public static readonly char[] Chars =
    [
        ' ',
        '\r',
        '\n',
        '\t',
        '\u180E', // mongolian vowel separator
        '\u200B', // zero width space
        '\u200C', // zero width non-joiner
        '\u200D', // zero width joiner
        '\u2060', // word joiner
        '\uFEFF' // zero width non-breaking space
    ];

    /// <summary>
    /// An optimized collection of characters representing whitespaces for search operations.
    /// </summary>
    /// <remarks>
    /// Utilizes the predefined whitespace characters for enhanced search performance.
    /// </remarks>
    public static readonly SearchValues<char> SearchValues = System.Buffers.SearchValues.Create(Chars);
}