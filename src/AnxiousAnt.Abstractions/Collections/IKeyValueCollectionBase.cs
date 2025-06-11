// Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Util/NameValueList.cs

namespace AnxiousAnt.Collections;

/// <summary>
/// Represents a collection of key-value pairs that supports retrieval and manipulation of values by key.
/// </summary>
/// <typeparam name="TValue">The type of values associated with the keys.</typeparam>
public interface IKeyValueCollectionBase<TValue>
{
    /// <summary>
    /// Retrieves the first value associated with the specified key or a default value if the key is not found.
    /// </summary>
    /// <param name="key">The key for which to retrieve the value.</param>
    /// <returns>
    /// The first value associated with the specified key, or the default value of <typeparamref name="TValue"/>
    /// if the key does not exist.
    /// </returns>
    TValue FirstOrDefault(string key);

    /// <summary>
    /// Tries to retrieve the first value associated with the specified key.
    /// </summary>
    /// <param name="key">The key for which to retrieve the value.</param>
    /// <param name="value">
    /// When this method returns, contains the first value associated with the specified key if found;
    /// otherwise, the default value for the type of the <typeparamref name="TValue"/> parameter.
    /// </param>
    /// <returns>
    /// <c>true</c> if a value was found for the specified key; otherwise, <c>false</c>.
    /// </returns>
    bool TryGetFirst(string key, [MaybeNullWhen(false)] out TValue value);

    /// <summary>
    /// Retrieves all values associated with the specified key.
    /// </summary>
    /// <param name="key">The key for which to retrieve the values.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TValue}"/> containing all values associated with the specified key,
    /// or an empty sequence if the key does not exist.
    /// </returns>
    IEnumerable<TValue> GetAll(string key);

    /// <summary>
    /// Determines whether the collection contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the collection.</param>
    /// <returns>
    /// <c>true</c> if the collection contains an entry with the specified key; otherwise, <c>false</c>.
    /// </returns>
    bool Contains(string key);
}