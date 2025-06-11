// Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Util/NameValueList.cs
namespace AnxiousAnt.Collections;

/// <summary>
/// A collection of key-value pairs that supports case-sensitive or case-insensitive key lookups and provides
/// additional functionality for managing and querying the pairs.
/// </summary>
/// <typeparam name="TValue">The type of the values stored in the collection.</typeparam>
public sealed class KeyValueCollection<TValue>
    : List<KeyValuePair<string, TValue>>, IKeyValueCollection<TValue>, IReadOnlyKeyValueCollection<TValue>
{
    private readonly bool _ignoreCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyValueCollection{TValue}"/> class with default settings.
    /// </summary>
    public KeyValueCollection() : this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyValueCollection{TValue}"/> class with the specified case
    /// sensitivity and a capacity of zero.
    /// </summary>
    /// <param name="ignoreCase">A value indicating whether name matching in the collection should be case-insensitive.</param>
    public KeyValueCollection(bool ignoreCase) : this(0, ignoreCase)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyValueCollection{TValue}"/> class with the specified
    /// capacity and case sensitivity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new collection can initially store.</param>
    /// <param name="ignoreCase">A value indicating whether name matching in the collection should be case-insensitive.</param>
    public KeyValueCollection(int capacity, bool ignoreCase = false) : base(capacity)
    {
        _ignoreCase = ignoreCase;
    }

    /// <inheritdoc />
    public TValue FirstOrDefault(string key) => GetAll(key).FirstOrDefault()!;

    /// <inheritdoc />
    public bool TryGetFirst(string key, [MaybeNullWhen(false)] out TValue value)
    {
        foreach (var item in GetAll(key))
        {
            value = item;
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public IEnumerable<TValue> GetAll(string key) => this
        .Where(x => x.Key.OrdinalEquals(key, _ignoreCase))
        .Select(x => x.Value);

    /// <inheritdoc />
    public bool Contains(string key) => this.Any(x => x.Key.OrdinalEquals(key, _ignoreCase));

    /// <inheritdoc />
    public void Add(string key, TValue value) => Add(new KeyValuePair<string, TValue>(key, value));

    /// <inheritdoc />
    public void AddOrReplace(string key, TValue value)
    {
        var i = 0;
        var replaced = false;

        while (i < Count)
        {
            if (!this[i].Key.OrdinalEquals(key, _ignoreCase))
            {
                i++;
            }
            else if (replaced)
            {
                RemoveAt(i);
            }
            else
            {
                this[i] = new KeyValuePair<string, TValue>(key, value);
                replaced = true;
                i++;
            }
        }

        if (!replaced)
        {
            Add(key, value);
        }
    }

    /// <inheritdoc />
    public bool Remove(string key) =>
        RemoveAll(x => x.Key.OrdinalEquals(key, _ignoreCase)) > 0;

    /// <inheritdoc />
    [Pure]
    public new ReadOnlyKeyValueCollection<TValue> AsReadOnly() =>
        Count == 0
            ? ReadOnlyKeyValueCollection<TValue>.Empty
            : new ReadOnlyKeyValueCollection<TValue>(this);
}