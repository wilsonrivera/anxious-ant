using System.Collections.Frozen;
using System.Runtime.InteropServices;

namespace AnxiousAnt.Text;

/// <summary>
/// Provides an implementation of the Porter stemming algorithm, which is used to reduce words to their root form.
/// </summary>
/// <remarks>
/// Adapted from https://github.com/nemec/porter2-stemmer/blob/5357533f474867ad126ad0aa81a447e5fbcf7576/Porter2Stemmer/EnglishPorter2Stemmer.cs
/// </remarks>
public static class Porter2Stemmer
{
    private static readonly SearchValues<char> Vowels = SearchValues.Create("aeiouy");
    private static readonly SearchValues<char> LiEndings = SearchValues.Create("cdeghkmnrt");
    private static readonly SearchValues<char> NonShortConsonants = SearchValues.Create("wxY");

    private static readonly Dictionary<string, string> Exceptions = new()
    {
        { "skis", "ski" },
        { "skies", "sky" },
        { "dying", "die" },
        { "lying", "lie" },
        { "tying", "tie" },
        { "idly", "idl" },
        { "gently", "gentl" },
        { "ugly", "ugli" },
        { "early", "earli" },
        { "only", "onli" },
        { "singly", "singl" },
        { "sky", "sky" },
        { "news", "news" },
        { "howe", "howe" },
        { "atlas", "atlas" },
        { "cosmos", "cosmos" },
        { "bias", "bias" },
        { "andes", "andes" }
    };

    private static readonly FrozenSet<string> ExceptionsRegion1 = ["gener", "arsen", "commun"];

    private static readonly FrozenSet<string> ExceptionsRegion2 =
    [
        "inning", "outing", "canning", "herring", "earring", "proceed", "exceed", "succeed"
    ];

    private static readonly string[] Step0Suffixes = ["'s'", "'s", "'"];
    private static readonly string[] Step1BSuffixes1 = ["eedly", "eed"];
    private static readonly string[] Step1BSuffixes2 = ["ed", "edly", "ing", "ingly"];
    private static readonly string[] Step1BSuffixes3 = ["at", "bl", "iz"];
    private static readonly string[] Step1BDoubles = ["bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt"];

    private static readonly Dictionary<string, string> Step2Suffixes = new()
    {
        { "ization", "ize" },
        { "ational", "ate" },
        { "ousness", "ous" },
        { "iveness", "ive" },
        { "fulness", "ful" },
        { "tional", "tion" },
        { "lessli", "less" },
        { "biliti", "ble" },
        { "entli", "ent" },
        { "ation", "ate" },
        { "alism", "al" },
        { "aliti", "al" },
        { "fulli", "ful" },
        { "ousli", "ous" },
        { "iviti", "ive" },
        { "enci", "ence" },
        { "anci", "ance" },
        { "abli", "able" },
        { "izer", "ize" },
        { "ator", "ate" },
        { "alli", "al" },
        { "bli", "ble" }
    };

    private static readonly Dictionary<string, string?> Step3Suffixes = new()
    {
        { "ational", "ate" },
        { "tional", "tion" },
        { "alize", "al" },
        { "icate", "ic" },
        { "iciti", "ic" },
        { "ical", "ic" },
        { "ful", null },
        { "ness", null }
    };

    private static readonly string[] Step4Suffixes =
    [
        "al", "ance", "ence", "er", "ic", "able", "ible", "ant", "ement", "ment", "ent", "ism", "ate", "iti", "ous",
        "ive", "ize"
    ];

    /// <summary>
    /// Reduces a word to its root or base form (stem) by applying the Porter2 stemming algorithm.
    /// </summary>
    /// <param name="word">The word to be stemmed.</param>
    /// <returns>
    /// The stemmed version of the input word. If the word is <c>null</c> or empty, an empty string is returned.
    /// Words shorter than three characters are returned unchanged.
    /// </returns>
    public static string Stem(string? word) =>
        string.IsNullOrEmpty(word) ? string.Empty : Stem(word.AsSpan());

    /// <summary>
    /// Applies the Porter2 stemming algorithm to reduce a word to its base or root form.
    /// </summary>
    /// <param name="word">The input word as a read-only span of characters to be stemmed.</param>
    /// <returns>
    /// The stemmed base form of the input word. If the input is empty or has a length of two or less,
    /// the original string is returned.
    /// </returns>
    public static string Stem(in ReadOnlySpan<char> word)
    {
        if (word.IsEmpty)
        {
            return string.Empty;
        }

        var wordSpan = word.Trim();
        if (wordSpan.Length <= 2)
        {
            return word.ToString();
        }

        using var owner = SpanOwner<char>.Allocate(wordSpan.Length + 8);
        var writtenChars = wordSpan.ToLowerInvariant(owner.Span);
        var state = new State { Chars = owner.Span, Length = writtenChars };

        TrimStartingApostrophe(ref state);

        var alternateLookup = Exceptions.GetAlternateLookup<ReadOnlySpan<char>>();
        if (alternateLookup.TryGetValue(state.CurrentWord, out var exception))
        {
            return exception;
        }

        MarkYsAsConsonants(ref state);

        // Region 1 is the region after the first non-vowel following a vowel, or the end of the word if
        // there is no such non-vowel.
        var region1 = GetRegion1(state.CurrentWord);
        // R2 is the region after the first non-vowel following a vowel in R1, or the end of the word if
        // there is no such non-vowel.
        var region2 = GetRegion(state.CurrentWord, region1);

        Step0RemoveSPluralSuffix(ref state);
        Step1ARemoveOtherSPluralSuffixes(ref state);

        var region2AlternateLookup = ExceptionsRegion2.GetAlternateLookup<ReadOnlySpan<char>>();
        if (region2AlternateLookup.Contains(state.CurrentWord))
        {
            return ToString(state, word);
        }

        Step1BRemoveLySuffixes(ref state, region1);
        Step1CReplaceSuffixYWithIIfPreceededWithConsonant(ref state);
        Step2ReplaceSuffixes(ref state, region1);
        Step3ReplaceSuffixes(ref state, region1, region2);
        Step4RemoveSomeSuffixesInR2(ref state, region2);
        Step5RemoveEorLSuffixes(ref state, region1, region2);

        return ToString(state, word);
    }

    private static string ToString(in State state, in ReadOnlySpan<char> original)
    {
        if (state.Length <= 0)
        {
            return string.Empty;
        }

        var word = state.CurrentWord;
        using var owner = SpanOwner<char>.Allocate(word.Length);
        var writtenChars = word.ToLowerInvariant(owner.Span);

        word = owner.Span[..writtenChars];
        return word.SequenceEqual(original) ? original.ToString() : StringPool.Shared.GetOrAdd(word);
    }

    /// <summary>
    /// The English stemmer treats apostrophe as a letter, removing it from the beginning of a word, where it might
    /// have stood for an opening quote, from the end of the word, where it might have stood for a closing quote,
    /// or been an apostrophe following s.
    /// </summary>
    private static void TrimStartingApostrophe(ref State state)
    {
        if (state.Chars[0] != '\'')
        {
            return;
        }

        state.Chars = state.Chars[1..];
        state.Length -= 1;
    }

    /// <summary>
    /// Set initial y, or y after a vowel, to Y
    /// </summary>
    private static void MarkYsAsConsonants(ref State state)
    {
        ref var searchSpace = ref MemoryMarshal.GetReference(state.Chars);
        for (var i = 0; i < state.Length; i++)
        {
            ref char chr = ref Unsafe.Add(ref searchSpace, i);
            if (chr == 'y' &&
                (i == 0 || Vowels.Contains(Unsafe.Add(ref searchSpace, i - 1))))
            {
                state.Chars[i] = 'Y';
            }
        }
    }

    private static int GetRegion1(in ReadOnlySpan<char> word)
    {
        foreach (var exception in ExceptionsRegion1)
        {
            if (word.StartsWith(exception))
            {
                return exception.Length;
            }
        }

        return GetRegion(word, 0);
    }

    private static int GetRegion(in ReadOnlySpan<char> word, int begin)
    {
        var foundVowel = false;
        ref var searchSpace = ref MemoryMarshal.GetReference(word);
        for (var i = begin; i < word.Length; i++)
        {
            ref char chr = ref Unsafe.Add(ref searchSpace, i);
            if (Vowels.Contains(chr))
            {
                foundVowel = true;
                continue;
            }

            if (foundVowel && !Vowels.Contains(chr))
            {
                return i + 1;
            }
        }

        return word.Length;
    }

    private static bool IsSuffixInRegion(in State state, int region, string suffix) =>
        region <= state.Length - suffix.Length;

    private static void Step0RemoveSPluralSuffix(ref State state)
    {
        var word = state.CurrentWord;
        foreach (var oldSuffix in Step0Suffixes)
        {
            if (!word.EndsWith(oldSuffix))
            {
                continue;
            }

            state.ReplaceSuffix(oldSuffix);
            break;
        }
    }

    private static void Step1ARemoveOtherSPluralSuffixes(ref State state)
    {
        var word = state.CurrentWord;
        if (word.EndsWith("sses"))
        {
            state.ReplaceSuffix("sses", "ss");
            return;
        }

        if (word.EndsWith("ied") || word.EndsWith("ies"))
        {
            var restOfWord = word[..^3];
            var numOfCharsToKeep = state.Length > 4 ? 1 : 2;
            state.Length = restOfWord.Length + numOfCharsToKeep;
            return;
        }

        if (word.EndsWith("us") || word.EndsWith("ss") || word[^1] is not 's' || state.Length < 3)
        {
            return;
        }

        // Skip both the last letter ('s') and the letter before that
        ref var searchSpace = ref MemoryMarshal.GetReference(state.Chars);
        for (var i = 0; i < state.Length - 2; i++)
        {
            ref char chr = ref Unsafe.Add(ref searchSpace, i);
            if (!Vowels.Contains(chr))
            {
                continue;
            }

            state.Length--;
            break;
        }
    }

    private static void Step1BRemoveLySuffixes(ref State state, int region1)
    {
        var word = state.CurrentWord;
        foreach (var suffix in Step1BSuffixes1)
        {
            if (!word.EndsWith(suffix))
            {
                continue;
            }

            if (IsSuffixInRegion(state, region1, suffix))
            {
                state.ReplaceSuffix(suffix, "ee");
            }

            return;
        }

        foreach (var suffix in Step1BSuffixes2)
        {
            if (!word.EndsWith(suffix))
            {
                continue;
            }

            var trunc = word[..(state.Length - suffix.Length)];
            if (!trunc.ContainsAny(Vowels))
            {
                break;
            }

            state.ReplaceSuffix(suffix);
            if (EndsWithAny(trunc, Step1BSuffixes3))
            {
                state.Chars[state.Length++] = 'e';
            }
            else if (EndsWithAny(trunc, Step1BDoubles))
            {
                state.Length--;
            }
            else if (EndsInShortSyllable(trunc) && GetRegion1(trunc) == trunc.Length)
            {
                state.Chars[state.Length++] = 'e';
            }

            break;
        }
    }

    private static bool EndsWithAny(in ReadOnlySpan<char> s, in ReadOnlySpan<string> searches)
    {
        foreach (var search in searches)
        {
            if (s.EndsWith(search, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static void Step1CReplaceSuffixYWithIIfPreceededWithConsonant(ref State state)
    {
        var word = state.CurrentWord;
        if (word[^1] is 'y' or 'Y' && word.Length > 2 && !Vowels.Contains(word[^2]))
        {
            state.Chars[state.Length - 1] = 'i';
        }
    }

    private static void Step2ReplaceSuffixes(ref State state, int region1)
    {
        var word = state.CurrentWord;
        foreach (var suffix in Step2Suffixes)
        {
            if (!word.EndsWith(suffix.Key))
            {
                continue;
            }

            if (IsSuffixInRegion(state, region1, suffix.Key))
            {
                _ = state.TryReplaceSuffix(suffix.Key, suffix.Value);
            }

            return;
        }

        if (word.EndsWith("ogi") && IsSuffixInRegion(state, region1, "ogi") && word[^4] == 'l')
        {
            state.ReplaceSuffix("ogi", "og");
            return;
        }

        if ((word.EndsWith("li") & IsSuffixInRegion(state, region1, "li")) && LiEndings.Contains(word[^3]))
        {
            state.ReplaceSuffix("li");
        }
    }

    private static void Step3ReplaceSuffixes(ref State state, int region1, int region2)
    {
        var word = state.CurrentWord;
        foreach (var suffix in Step3Suffixes)
        {
            if (!word.EndsWith(suffix.Key))
            {
                continue;
            }

            if (IsSuffixInRegion(state, region1, suffix.Key))
            {
                _ = state.TryReplaceSuffix(suffix.Key, suffix.Value);
            }

            return;
        }

        if (word.EndsWith("ative") &&
            IsSuffixInRegion(state, region1, "ative") &&
            IsSuffixInRegion(state, region2, "ative"))
        {
            state.ReplaceSuffix("ative");
        }
    }

    private static void Step4RemoveSomeSuffixesInR2(ref State state, int region2)
    {
        var word = state.CurrentWord;
        foreach (var suffix in Step4Suffixes)
        {
            if (!word.EndsWith(suffix))
            {
                continue;
            }

            if (IsSuffixInRegion(state, region2, suffix))
            {
                state.ReplaceSuffix(suffix);
            }

            return;
        }

        if (word.EndsWith("ion") && IsSuffixInRegion(state, region2, "ion") && word[^4] is 's' or 't')
        {
            state.ReplaceSuffix("ion");
        }
    }

    private static void Step5RemoveEorLSuffixes(ref State state, int region1, int region2)
    {
        var word = state.CurrentWord;
        if (word[^1] is 'e' &&
            (
                IsSuffixInRegion(state, region2, "e") ||
                (IsSuffixInRegion(state, region1, "e") && !EndsInShortSyllable(state.CurrentWord[..^1]))
            )
           )
        {
            state.ReplaceSuffix("e");
            return;
        }

        if (word[^1] is 'l' && IsSuffixInRegion(state, region2, "l") && state.Length > 1 && word[^2] is 'l')
        {
            state.ReplaceSuffix("l");
        }
    }

    /// <summary>
    /// Define a short syllable in a word as either (a) a vowel followed by a non-vowel other than w, x or Y and
    /// preceded by a non-vowel, or * (b) a vowel at the beginning of the word followed by a non-vowel.
    /// </summary>
    private static bool EndsInShortSyllable(in ReadOnlySpan<char> word) =>
        word.Length switch
        {
            < 2 => false,
            2 => Vowels.Contains(word[0]) && !Vowels.Contains(word[1]),
            _ => Vowels.Contains(word[^2]) &&
                 !Vowels.Contains(word[^1]) &&
                 !NonShortConsonants.Contains(word[^1]) &&
                 !Vowels.Contains(word[^3])
        };

    private ref struct State
    {
        public Span<char> Chars;
        public int Length;

        public ReadOnlySpan<char> CurrentWord => Chars[..Length];

        public void ReplaceSuffix(string oldSuffix, string? newSuffix = null)
        {
            Length -= oldSuffix.Length;
            if (newSuffix is null)
            {
                return;
            }

            var newSuffixSpan = newSuffix.AsSpan();
            newSuffixSpan.CopyTo(Chars[Length..]);
            Length += newSuffixSpan.Length;
        }

        public bool TryReplaceSuffix(string oldSuffix, string? newSuffix)
        {
            if (!CurrentWord.Contains(oldSuffix.AsSpan(), StringComparison.Ordinal))
            {
                return false;
            }

            ReplaceSuffix(oldSuffix, newSuffix);
            return true;
        }
    }
}