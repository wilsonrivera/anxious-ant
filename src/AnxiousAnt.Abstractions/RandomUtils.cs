using System.Security.Cryptography;

using AnxiousAnt.Text;

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
    /// Fills the provided span with random characters, optionally including symbols in the character set.
    /// </summary>
    /// <param name="destination">The span of characters to populate with random values.</param>
    /// <param name="includeSymbols">A boolean that specifies whether symbols should be included in the character set.</param>
    public static void GetRandomChars(Span<char> destination, bool includeSymbols = false)
    {
        if (destination.IsEmpty)
        {
            return;
        }

        RandomNumberGenerator.GetItems(
            includeSymbols
                ? RandomStringAlphabetWithSymbols.AsSpan()
                : RandomStringAlphabet.AsSpan(),
            destination
        );
    }

    /// <summary>
    /// Generates a random name from the list of adjectives and surnames formatted as "adjective-noun".
    /// For example 'envious-otter'.
    /// </summary>
    /// <returns>
    /// A randomly generated name.
    /// </returns>
    public static string GetRandomName()
    {
        using var leftDestination = SpanOwner<string>.Allocate(1);
        using var rightDestination = SpanOwner<string>.Allocate(1);

        RandomNumberGenerator.GetItems(AdjectivesPool, leftDestination.Span);
        RandomNumberGenerator.GetItems(NounsPool, rightDestination.Span);

        var sb = StringBuilderPool.Rent()
            .Append(leftDestination.Span[0])
            .Append('-')
            .Append(rightDestination.Span[0]);

        return StringBuilderPool.ToStringAndReturn(sb);
    }

    /// <summary>
    /// Calculates the maximum possible length of a random name based on the longest adjective and noun
    /// available in their respective pools.
    /// </summary>
    /// <returns>
    /// The maximum length of a randomly generated name, which is computed as the sum of the longest adjective,
    /// the longest noun, and an additional character for the separator.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static int GetMaxPossibleRandomNameLength() =>
        LazyMaxAdjectiveLength.Value + LazyMaxNounLength.Value + 1;

    /// <summary>
    /// Attempts to generate a random name combining a randomly selected adjective and noun.
    /// </summary>
    /// <param name="destination">The span where the generated random name will be written.</param>
    /// <param name="charsWritten">The number of characters written into the <paramref name="destination"/>.</param>
    /// <returns>
    /// <c>true</c> if a random name was successfully generated and written to the provided
    /// <paramref name="destination"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryGetRandomName(Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;
        if (destination.IsEmpty)
        {
            return false;
        }

        using var leftDestination = SpanOwner<string>.Allocate(1);
        using var rightDestination = SpanOwner<string>.Allocate(1);

        RandomNumberGenerator.GetItems(AdjectivesPool, leftDestination.Span);
        RandomNumberGenerator.GetItems(NounsPool, rightDestination.Span);

        var leftSpan = leftDestination.Span[0].AsSpan();
        var rightSpan = rightDestination.Span[0].AsSpan();
        if (destination.Length < leftSpan.Length + rightSpan.Length + 1)
        {
            return false;
        }

        leftSpan.CopyTo(destination);
        destination[leftSpan.Length] = '-';
        rightSpan.CopyTo(destination[(leftSpan.Length + 1)..]);
        charsWritten = leftSpan.Length + rightSpan.Length + 1;

        return true;
    }
}