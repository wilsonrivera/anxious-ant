namespace AnxiousAnt.Text;

/// <summary>
/// Provides extensions for the <see cref="IPorter2Stemmer"/> interface.
/// </summary>
public static class Porter2StemmerExtensions
{
    /// <summary>
    /// Reduces a word to its root or base form (stem) by applying the Porter2 stemming algorithm.
    /// </summary>
    /// <param name="stemmer">The <see cref="IPorter2Stemmer"/>.</param>
    /// <param name="word">The word to be stemmed.</param>
    /// <returns>
    /// The stemmed version of the input word.
    /// </returns>
    public static string Stem(this IPorter2Stemmer stemmer, string? word)
    {
        ArgumentNullException.ThrowIfNull(stemmer);
        return string.IsNullOrEmpty(word) ? string.Empty : stemmer.Stem(word.AsSpan());
    }
}