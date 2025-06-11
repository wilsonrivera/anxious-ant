using System.Collections;

namespace AnxiousAnt.Collections;

/// <summary>
/// Represents a read-only wrapper for a collection of key-value pairs.
/// </summary>
/// <typeparam name="TValue">The type of values associated with the keys.</typeparam>
public sealed class ReadOnlyKeyValueCollection<TValue> : IReadOnlyKeyValueCollection<TValue>
{
    private readonly IReadOnlyKeyValueCollection<TValue> _innerCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyKeyValueCollection{TValue}"/> class that is a read-only
    /// wrapper around the specified collection.
    /// </summary>
    /// <param name="innerCollection">The collection to wrap.</param>
    public ReadOnlyKeyValueCollection(IReadOnlyKeyValueCollection<TValue> innerCollection)
    {
        ArgumentNullException.ThrowIfNull(innerCollection);

        _innerCollection = innerCollection;
    }

    /// <summary>
    /// Gets an empty instance of the <see cref="ReadOnlyKeyValueCollection{TValue}"/>.
    /// </summary>
    public static ReadOnlyKeyValueCollection<TValue> Empty { get; } = new(new KeyValueCollection<TValue>());

    /// <inheritdoc />
    public int Count => _innerCollection.Count;

    /// <inheritdoc />
    public KeyValuePair<string, TValue> this[int index] => _innerCollection[index];

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => _innerCollection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public TValue FirstOrDefault(string key) => _innerCollection.FirstOrDefault(key);

    /// <inheritdoc />
    public bool TryGetFirst(string key, [MaybeNullWhen(false)] out TValue value) =>
        _innerCollection.TryGetFirst(key, out value);

    /// <inheritdoc />
    public IEnumerable<TValue> GetAll(string key) => _innerCollection.GetAll(key);

    /// <inheritdoc />
    public bool Contains(string key) => _innerCollection.Contains(key);
}