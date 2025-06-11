using System.Runtime.InteropServices;

using Unidecode.NET;

namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for converting strings into URL-friendly "slug" formats.
/// </summary>
public static class SlugifyStringExtensions
{
    private const int MaximumStringLength = 1024 * 2; // ~2k chars
    private const char SeparatorChar = '-';
    private const string UnknownChar = "[?]";

    private static readonly Dictionary<char, string> SpecialCharReplacementMap = new()
    {
        ['&'] = "and",
        ['|'] = "or",
        ['<'] = "less",
        ['>'] = "greater",
        // ['\u00a4'] = "currency",
        // ['$'] = "dollar",
        // ['\u00a2'] = "cent",
        // ['\u00a3'] = "pound",
        // ['\u20ac'] = "euro",
        // ['\u00a5'] = "yen",
        // ['\u20b9'] = "rupee",
        // ['%'] = "percent",
        ['\u00a9'] = "(c)",
        ['\u00ae'] = "(r)",
        ['ª'] = "a",
        ['º'] = "o",
        ['\u2665'] = "love"
    };

    private static readonly Lazy<int> LazyMaxReplacementLength =
        new(static () => SpecialCharReplacementMap.Values.Max(x => x.Length) + 1);

    /// <summary>
    /// Converts the specified string to a URL-friendly "slug" format.
    /// </summary>
    /// <param name="s">The input string to be converted to a slug.</param>
    /// <returns>
    /// A URL-friendly version of the input string.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Slugify(this string? s) =>
        string.IsNullOrWhiteSpace(s) ? string.Empty : Slugify(s.AsSpan());

    /// <summary>
    /// Converts the specified string span to a URL-friendly "slug" format.
    /// </summary>
    /// <param name="s">The input span of characters to be converted to a slug.</param>
    /// <returns>
    /// A URL-friendly version of the input span of characters.
    /// </returns>
    [Pure]
    public static string Slugify(this ReadOnlySpan<char> s)
    {
        if (s.IsWhiteSpace())
        {
            return string.Empty;
        }

        s = s.Trim();
        if (s.Length >= MaximumStringLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(s),
                $"Input string exceeds the maximum allowed length of {MaximumStringLength} characters. Please reduce the length of the input string."
            );
        }

        using var normalizedStringOwner = SpanOwner<char>.Allocate(s.Length * 14);
        Normalize(s, normalizedStringOwner.Span, out var charsWritten);

        using var slugOwner = SpanOwner<char>.Allocate(charsWritten * LazyMaxReplacementLength.Value);
        Slugify(
            normalizedStringOwner.Span[..charsWritten],
            slugOwner.Span,
            out charsWritten
        );

        return charsWritten == 0
            ? string.Empty
            : string.Create(
                charsWritten,
                slugOwner.DangerousGetArray().Array!,
                StringExtensions.WriteToStringMemorySpanAction
            );
    }

    private static void Normalize(ReadOnlySpan<char> s, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        ref var searchSpace = ref MemoryMarshal.GetReference(s);
        for (var i = 0; i < s.Length; i++)
        {
            ref char chr = ref Unsafe.Add(ref searchSpace, i);
            switch (chr)
            {
                case < '\u0080':
                    destination[charsWritten++] = chr;
                    break;
                case '\u180E' or '\u200B' or '\u200C' or '\u200D' or '\u2060' or '\uFEFF':
                    destination[charsWritten++] = WhiteSpaces.Char;
                    break;
                default:
                    if (SpecialCharReplacementMap.ContainsKey(chr))
                    {
                        destination[charsWritten++] = chr;
                        break;
                    }

                    var unidecoded = chr.Unidecode().AsSpan();
                    if (unidecoded.IsWhiteSpace() || unidecoded.SequenceEqual(UnknownChar.AsSpan()))
                    {
                        break;
                    }

                    ref char unidecodedSearchSpace = ref MemoryMarshal.GetReference(unidecoded);
                    for (var j = 0; j < unidecoded.Length; j++)
                    {
                        destination[charsWritten++] = Unsafe.Add(ref unidecodedSearchSpace, j);
                    }

                    break;
            }
        }
    }

    private static void Slugify(ReadOnlySpan<char> s, Span<char> charsBuffer, out int charsWritten)
    {
        var state = new State { Length = s.Length, Source = s };
        Slugify(s, charsBuffer, ref state);
        if (state.WasLastCharacterSeparator)
        {
            state.CharsWritten--;
        }

        charsWritten = state.CharsWritten;
    }

    private static void Slugify(ReadOnlySpan<char> s, Span<char> charsBuffer, ref State state)
    {
        ref var searchSpace = ref MemoryMarshal.GetReference(s);
        for (var i = 0; i < s.Length; i++)
        {
            state.Index = i;
            state.PrevChar = i > 0 ? state.Char : '\0';
            state.Char = Unsafe.Add(ref searchSpace, i);
            state.NextChar = i + 1 < s.Length ? Unsafe.Add(ref searchSpace, i + 1) : '\0';

            switch (state.Char)
            {
                case SeparatorChar:
                case WhiteSpaces.Char:
                    if (state.WasLastCharacterSeparator)
                    {
                        break;
                    }

                    charsBuffer[state.CharsWritten++] = SeparatorChar;
                    state.WasLastCharacterSeparator = true;
                    break;
                case >= 'a' and <= 'z' or >= '0' and <= '9':
                    charsBuffer[state.CharsWritten++] = state.Char;
                    state.WasLastCharacterSeparator = false;
                    state.WasLastCharacterUppercase = false;
                    break;
                case >= 'A' and <= 'Z':
                    SlugifyUppercase(charsBuffer, ref state);
                    break;
                case '\'':
                    if (state.NextChar is 't' or 'T' or 's' or 'S' &&
                        (
                            state.Index + 2 >= state.Length ||
                            state.Source[state.Index + 2] is ' ' or '\u180E' or '\u200B' or '\u200C' or '\u200D'
                                or '\u2060' or '\uFEFF'
                        ))
                    {
                        break;
                    }

                    charsBuffer[state.CharsWritten++] = SeparatorChar;
                    state.WasLastCharacterSeparator = true;
                    break;
                default:
                    if (SpecialCharReplacementMap.TryGetValue(state.Char, out var replacement))
                    {
                        if (!state.WasLastCharacterSeparator)
                        {
                            charsBuffer[state.CharsWritten++] = SeparatorChar;
                        }

                        var replacementSpan = replacement.AsSpan();
                        ref char replacementSearchSpace = ref MemoryMarshal.GetReference(replacementSpan);
                        for (var j = 0; j < replacementSpan.Length; j++)
                        {
                            charsBuffer[state.CharsWritten++] = Unsafe.Add(ref replacementSearchSpace, j);
                        }

                        charsBuffer[state.CharsWritten++] = SeparatorChar;
                        state.WasLastCharacterSeparator = true;
                        break;
                    }

                    if (state.WasLastCharacterSeparator || state.CharsWritten == 0)
                    {
                        break;
                    }

                    charsBuffer[state.CharsWritten++] = SeparatorChar;
                    state.WasLastCharacterSeparator = true;
                    break;
            }
        }
    }

    private static void SlugifyUppercase(Span<char> charsBuffer, ref State state)
    {
        if ((state.PrevChar is >= 'a' and <= 'z' or >= '0' and <= '9' && state.NextChar is >= 'A' and <= 'Z') ||
            (state.PrevChar is >= 'a' and <= 'z') ||
            (state.PrevChar is >= 'A' and <= 'Z' && state.NextChar is >= 'a' and <= 'z' and not 's'))
        {
            charsBuffer[state.CharsWritten++] = SeparatorChar;
        }

        charsBuffer[state.CharsWritten++] = (char)(state.Char + 32);
        if (state.PrevChar is >= 'A' and <= 'Z' && state.NextChar is >= '0' and <= '9')
        {
            state.WasLastCharacterSeparator = true;
            state.WasLastCharacterUppercase = false;
            charsBuffer[state.CharsWritten++] = SeparatorChar;
        }
        else
        {
            state.WasLastCharacterUppercase = true;
            state.WasLastCharacterSeparator = false;
        }
    }

    private ref struct State
    {
        public int Length;
        public int Index;
        public ReadOnlySpan<char> Source;
        public char PrevChar;
        public char Char;
        public char NextChar;
        public int CharsWritten;
        public bool WasLastCharacterSeparator;
        public bool WasLastCharacterUppercase;
    }
}