// Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Util/NameValueList.cs

namespace AnxiousAnt.Collections;

/// <summary>
/// Represents a collection of name-value pairs that supports list-based operations
/// as well as retrieval and manipulation of values by name.
/// </summary>
/// <typeparam name="TValue">The type of values associated with the names.</typeparam>
public interface IKeyValueCollection<TValue> : IList<KeyValuePair<string, TValue>>, IKeyValueCollectionBase<TValue>
{
    /// <summary>
    /// Adds a key-value pair to the collection.
    /// </summary>
    /// <param name="key">The key associated with the value.</param>
    /// <param name="value">The value to associate with the key.</param>
    void Add(string key, TValue value);

    /// <summary>
    /// Adds a key-value pair to the collection or replaces an existing value if the key already exists.
    /// </summary>
    /// <param name="key">The key associated with the value.</param>
    /// <param name="value">The value to associate with the key.</param>
    void AddOrReplace(string key, TValue value);

    /// <summary>
    /// Removes all values associated with the specified key.
    /// </summary>
    /// <param name="key">The key to remove from the collection.</param>
    /// <returns>
    /// <c>true</c> if an entry with the specified key was successfully removed; otherwise, <c>false</c>.
    /// </returns>
    bool Remove(string key);

    /// <summary>
    /// Returns a read-only representation of the current key-value collection.
    /// </summary>
    /// <returns>
    /// A read-only collection containing the same key-value pairs as the current collection.
    /// </returns>
    [Pure]
    ReadOnlyKeyValueCollection<TValue> AsReadOnly();
}