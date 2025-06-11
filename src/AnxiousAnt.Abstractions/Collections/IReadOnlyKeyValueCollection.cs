// Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Util/NameValueList.cs

namespace AnxiousAnt.Collections;

/// <summary>
/// Represents a read-only collection of key-value pairs that supports retrieval operations
/// and provides list-like access to items.
/// </summary>
/// <typeparam name="TValue">The type of values associated with the keys.</typeparam>
public interface IReadOnlyKeyValueCollection<TValue>
    : IReadOnlyList<KeyValuePair<string, TValue>>, IKeyValueCollectionBase<TValue>;