namespace AnxiousAnt.Text;

/// <summary>
/// Defines the contract for a Porter2 stemming algorithm implementation, which is used to reduce words to their
/// base or root form.
/// </summary>
public interface IPorter2Stemmer
{
    /// <summary>
    /// Applies the Porter2 stemming algorithm to reduce a word to its base or root form.
    /// </summary>
    /// <param name="word">The input word as a read-only span of characters to be stemmed.</param>
    /// <returns>
    /// The stemmed base form of the input word. If the input is empty or has a length of two or less,
    /// the original string is returned.
    /// </returns>
    string Stem(in ReadOnlySpan<char> word);
}