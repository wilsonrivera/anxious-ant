using System.Collections.ObjectModel;

namespace AnxiousAnt;

/// <summary>
/// Provides a set of extension methods for collection manipulation and handling.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Determines whether the specified collection is empty.
    /// </summary>
    /// <param name="source">The collection to be checked for emptiness.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>
    /// <c>true</c> if the collection contains no elements; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.TryGetNonEnumeratedCount(out var count))
        {
            return count == 0;
        }

        switch (source)
        {
            case ICollection<T> collection:
                return collection.Count == 0;
            case IReadOnlyCollection<T> readOnlyCollection:
                return readOnlyCollection.Count == 0;
        }

        return !source.Any();
    }

    /// <summary>
    /// Returns the specified collection if it is not null and contains items; otherwise, returns an empty collection.
    /// </summary>
    /// <param name="collection">The collection to check and return if not null and not empty.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>
    /// A non-null collection, either the provided collection or an empty collection.
    /// </returns>
    [Pure]
    public static IReadOnlyCollection<T> OrEmpty<T>(this IReadOnlyCollection<T>? collection) =>
        collection is not null && !IsEmpty(collection) ? collection : ReadOnlyCollection<T>.Empty;

    /// <summary>
    /// Converts the specified collection to an array.
    /// </summary>
    /// <param name="source">The collection to be converted to an array.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>
    /// An array containing the elements of the specified collection.
    /// </returns>
    [Pure]
    public static T[] AsArray<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source switch
        {
            T[] array => array,
            _ => source.ToArray()
        };
    }

    /// <summary>
    /// Adds a range of items to the specified collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list and collection.</typeparam>
    /// <param name="collection">The collection to which the items will be added.</param>
    /// <param name="items">The items to add to the collection.</param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(items);

        if (collection is List<T> listImpl)
        {
            listImpl.AddRange(items);
        }
        else
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}