using Microsoft.Extensions.ObjectPool;

namespace AnxiousAnt.ObjectPool;

/// <summary>
/// Provides a pool of <see cref="StringBuilder"/> instances for efficient reuse and reduced
/// memory allocations. The <see cref="StringBuilderPool"/> uses an object pool pattern to manage
/// <see cref="StringBuilder"/> objects, which is especially beneficial in scenarios involving
/// frequent string manipulations.
/// </summary>
public static class StringBuilderPool
{
    // internal for testing
    private static readonly Lazy<ObjectPool<StringBuilder>> LazyPool = new(
        static () => new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy())
    );

    /// <summary>
    /// Rents a <see cref="StringBuilder"/> from the pool.
    /// <see cref="StringBuilder"/> instances.
    /// </summary>
    /// <returns>
    /// A <see cref="StringBuilder"/> instance from the pool, ready for use in building strings.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Rent() => LazyPool.Value.Get();

    /// <summary>
    /// Returns a <see cref="StringBuilder"/> to the pool for reuse after it has been used.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> instance to be returned to the pool.</param>
    [ExcludeFromCodeCoverage(
        Justification = "Because of the nature of Lazy, we can't test anything other than it throws when given null."
    )]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return(StringBuilder sb)
    {
        ArgumentNullException.ThrowIfNull(sb);
        if (!LazyPool.IsValueCreated)
        {
            return;
        }

        LazyPool.Value.Return(sb);
    }

    /// <summary>
    /// Converts the contents of the provided <see cref="StringBuilder"/> to a string, then returns the instance
    /// back to the pool for reuse.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> instance whose contents are to be converted.</param>
    /// <returns>
    /// The <see cref="string"/> representation of the contents of the specified <see cref="StringBuilder"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToStringAndReturn(StringBuilder sb)
    {
        ArgumentNullException.ThrowIfNull(sb);

        var length = sb.Length;
        var result = length == 0 ? string.Empty : sb.ToString();
        if (LazyPool.IsValueCreated)
        {
            LazyPool.Value.Return(sb);
        }

        return result;
    }
}