using System.Collections;
using System.Diagnostics;

namespace AnxiousAnt.Collections;

/// <summary>
/// Represents a structure that provides a pooled array for efficient memory management,
/// offering a span, memory, and segment interface for accessing elements.
/// </summary>
/// <typeparam name="T">The type of elements in the pooled array.</typeparam>
[DebuggerDisplay("Length = {Length}")]
public struct RentedArray<T> : IEnumerable<T>, IDisposable
{
    private T[]? _arrayFromPool;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly bool _isEmpty;
    private readonly ArrayPool<T> _pool;
    private readonly int _length;

    private RentedArray(bool empty)
    {
        _isEmpty = empty;
        _arrayFromPool = [];
        _pool = ArrayPool<T>.Shared;
        _length = 0;
    }

    private RentedArray(ArrayPool<T> pool, T[] array, int length)
    {
        _arrayFromPool = array;
        _pool = pool;
        _length = length;
    }

    /// <summary>
    /// Gets an empty instance of <see cref="RentedArray{T}"/>.
    /// </summary>
    public static RentedArray<T> Empty { get; } = new(true);

    /// <summary>
    /// Gets the number of elements stored in the <see cref="RentedArray{T}"/>.
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _length;
    }

    /// <summary>
    /// Provides access to an element of the <see cref="PooledList{T}"/> by its zero-based index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to access.</param>
    /// <returns>
    /// A reference to the element at the specified <paramref name="index"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative or greater
    /// than or equal to the number of allocated elements in the backing storage.</exception>
    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var span = _arrayFromPool.AsSpan();
            ArgumentOutOfRangeException.ThrowIfNegative(index);

            ref var local = ref span[index];
            return ref local!;
        }
    }

    /// <summary>
    /// Gets the underlying array allocated from the pool, which backs the <see cref="RentedArray{T}"/>.
    /// </summary>
    public T[] Array => _arrayFromPool!;

    /// <summary>
    /// Gets a span representing the contiguous region of memory currently in use
    /// within the <see cref="RentedArray{T}"/>.
    /// </summary>
    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _arrayFromPool.AsSpan(0, _length);
    }

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> representing the current elements of the <see cref="RentedArray{T}"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="Memory{T}"/> will span only the used portion of the underlying array.
    /// If the array is uninitialized or contains no elements, the returned memory will be empty.
    /// </remarks>
    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _arrayFromPool.AsMemory(0, _length);
    }

    /// <summary>
    /// Gets an <see cref="ArraySegment{T}"/> representing the current elements of the <see cref="RentedArray{T}"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="ArraySegment{T}"/> provides access to the underlying memory for the elements
    /// currently stored in the array, starting at index 0 and with a count equal to the number of active elements.
    /// </remarks>
    public ArraySegment<T> ArraySegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(_arrayFromPool!, 0, _length);
    }

    /// <summary>
    /// Creates a new instance of <see cref="RentedArray{T}"/> from the given array,
    /// acquiring the array's content for internal use.
    /// </summary>
    /// <param name="array">The source array to create the <see cref="RentedArray{T}"/> instance from.</param>
    /// <returns>
    /// A <see cref="RentedArray{T}"/> instance containing the elements of the provided array.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static RentedArray<T> FromArray(T[] array) =>
        FromArray(ArrayPool<T>.Shared, array);

    /// <summary>
    /// Creates a new instance of <see cref="RentedArray{T}"/> from the given span,
    /// acquiring the span's content for internal use.
    /// </summary>
    /// <param name="span">The source span to create the <see cref="RentedArray{T}"/> instance from.</param>
    /// <returns>
    /// A <see cref="RentedArray{T}"/> instance containing the elements of the provided span.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static RentedArray<T> FromSpan(Span<T> span) =>
        FromSpan(ArrayPool<T>.Shared, span);

    internal static RentedArray<T> FromArray(ArrayPool<T> pool, T[] array)
    {
        ArgumentNullException.ThrowIfNull(pool);
        ArgumentNullException.ThrowIfNull(array);

        return FromSpan(pool, array.AsSpan());
    }

    internal static RentedArray<T> FromSpan(ArrayPool<T> pool, Span<T> source)
    {
        ArgumentNullException.ThrowIfNull(pool);
        if (source.Length == 0)
        {
            // No need to rent an empty array
            return Empty;
        }

        var myArray = pool.Rent(source.Length);
        source.CopyTo(myArray.AsSpan());

        return new RentedArray<T>(pool, myArray, source.Length);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => ArraySegment.GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _arrayFromPool;
        if (toReturn is null || _isEmpty)
        {
            return;
        }

        _arrayFromPool = null;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            toReturn.AsSpan(0, _length).Clear();
        }

        var pool = _pool ?? ArrayPool<T>.Shared;
        pool.Return(toReturn);
    }

    /// <summary>
    /// Returns a reference to the 0th element of the array.
    /// </summary>
    /// <returns>
    /// A reference to the 0th element of the array.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetPinnableReference()
    {
        // Ensure that the native code has just one forward branch that is predicted-not-taken.
        ref T ret = ref Unsafe.NullRef<T>();
        ref var array = ref _arrayFromPool;

        if (_length > 0 && array is not null)
        {
            ret = ref array[0];
        }

        return ref ret;
    }
}