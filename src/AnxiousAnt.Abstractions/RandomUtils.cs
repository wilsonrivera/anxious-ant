using System.Security.Cryptography;

namespace AnxiousAnt;

/// <summary>
/// Provides utilities for generating randomized data, such as random strings, with support for additional
/// customization options.
/// </summary>
public static partial class RandomUtils
{
    // List of characters obtained from
    // https://github.com/1Password/spg/blob/6cf2846c26cac127cdb107df2a37be698fa0888c/char_gen.go#L13
    private static readonly char[] RandomStringAlphabet =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    private static readonly char[] RandomStringAlphabetWithSymbols =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@.-_*".ToCharArray();

    /// <summary>
    /// Generates a random string of the specified length, optionally including symbols in the character set.
    /// </summary>
    /// <param name="length">The length of the random string to generate.</param>
    /// <param name="includeSymbols">A boolean that specifies whether symbols should be included in the character set.</param>
    /// <returns>
    /// A randomly generated string of the specified length, using a predefined character set. If
    /// <paramref name="includeSymbols"/> is true, symbols are included alongside alphanumeric characters in
    /// the character selection.
    /// </returns>
    public static string GetRandomString(int length = 24, bool includeSymbols = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        return RandomNumberGenerator.GetString(
            includeSymbols
                ? RandomStringAlphabetWithSymbols.AsSpan()
                : RandomStringAlphabet.AsSpan(),
            length
        );
    }

    /// <summary>
    /// Generates a random name from the list of adjectives and surnames formatted as "adjective_surname".
    /// For example 'focused_turing'.
    /// </summary>
    /// <returns>
    /// A randomly generated name.
    /// </returns>
    public static string GetRandomName() => GetRandomName(false);

    /// <summary>
    /// <para>
    /// Generates a random name from the list of adjectives and surnames formatted as "adjective_surname".
    /// For example 'focused_turing'.
    /// </para>
    /// <para>
    /// If <paramref name="retry"/> is <c>true</c>, a random integer between 0 and 10 will be added to the end of
    /// the name, e.g 'focused_turing3'.
    /// </para>
    /// </summary>
    /// <param name="retry">A flag indicating whether to append a random digit to teh generated name.</param>
    /// <returns>
    /// A randomly generated name.
    /// </returns>
    public static string GetRandomName(bool retry)
    {
        using var leftDestination = SpanOwner<string>.Allocate(1);
        using var rightDestination = SpanOwner<string>.Allocate(1);

        /* Steve Wozniak is not boring */
        var attempt = 0;
        do
        {
            RandomNumberGenerator.GetItems(AdjectivesPool, leftDestination.Span[..1]);
            RandomNumberGenerator.GetItems(SurnamesPool, rightDestination.Span[..1]);
            attempt++;
        } while (leftDestination.Span[0] is "boring" && rightDestination.Span[0] is "wozniak" && attempt < 100);

        return retry
            ? $"{leftDestination.Span[0]}_{rightDestination.Span[0]}{RandomNumberGenerator.GetInt32(10)}"
            : $"{leftDestination.Span[0]}_{rightDestination.Span[0]}";
    }
}