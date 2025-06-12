using System.Diagnostics;

namespace AnxiousAnt.Collections;

/// <summary>
/// Represents a structure that provides a pooled array for efficient memory management,
/// offering a span, memory, and segment interface for accessing elements.
/// </summary>
/// <typeparam name="T">The type of elements in the pooled array.</typeparam>
[DebuggerDisplay("Length = {Length}")]
public struct RentedArray<T> : IDisposable
{
    private T[]? _arrayFromPool;
    private readonly ArrayPool<T> _pool;
    private readonly int _length;

    private RentedArray(ArrayPool<T> pool, T[] array, int length)
    {
        _arrayFromPool = array;
        _pool = pool;
        _length = length;
    }

    /// <summary>
    /// Gets an empty instance of <see cref="RentedArray{T}"/>.
    /// </summary>
    public static RentedArray<T> Empty { get; } = new(ArrayPool<T>.Shared, [], 0);

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
            var span = _arrayFromPool.AsSpan(0, _length);
            ref var local = ref span[index];
            return ref local!;
        }
    }

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
    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _arrayFromPool.AsMemory(0, _length);
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
    public static RentedArray<T> FromArray(T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        return FromSpan(ArrayPool<T>.Shared, array.AsSpan());
    }

    /// <summary>
    /// Creates a new instance of <see cref="RentedArray{T}"/> from the given span,
    /// acquiring the span's content for internal use.
    /// </summary>
    /// <param name="source">The source span to create the <see cref="RentedArray{T}"/> instance from.</param>
    /// <returns>
    /// A <see cref="RentedArray{T}"/> instance containing the elements of the provided span.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static RentedArray<T> FromSpan(in Span<T> source) =>
        FromSpan(ArrayPool<T>.Shared, source);

    /// <summary>
    /// Creates a new instance of <see cref="RentedArray{T}"/> from the given span,
    /// acquiring the span's content for internal use.
    /// </summary>
    /// <param name="source">The source span to create the <see cref="RentedArray{T}"/> instance from.</param>
    /// <returns>
    /// A <see cref="RentedArray{T}"/> instance containing the elements of the provided span.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static RentedArray<T> FromSpan(in ReadOnlySpan<T> source) =>
        FromSpan(ArrayPool<T>.Shared, source);

    internal static RentedArray<T> FromSpan(ArrayPool<T> pool, in ReadOnlySpan<T> source)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _arrayFromPool;
        if (toReturn is null || toReturn.Length == 0)
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
    /// Returns an enumerator that iterates through the elements of the <see cref="RentedArray{T}"/>.
    /// </summary>
    /// <returns>
    /// An enumerator for the <see cref="RentedArray{T}"/>, allowing iteration over its elements.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Retrieves the underlying array used by the <see cref="RentedArray{T}"/> instance.
    /// </summary>
    /// <returns>
    /// The array backing this <see cref="RentedArray{T}"/> instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[]? GetArray() => _arrayFromPool;

    /// <summary>
    /// Converts the rented array into a new regular array, copying the elements to it.
    /// </summary>
    /// <returns>
    /// A new array of type <typeparamref name="T"/> containing all the elements from the current
    /// <see cref="RentedArray{T}"/>.
    /// </returns>
    public T[] ToArray()
    {
        var length = _length;
        var arrayFromPool = _arrayFromPool;
        if (length == 0)
        {
            return [];
        }

        var arr = new T[length];
        Array.Copy(arrayFromPool!, 0, arr, 0, length);
        return arr;
    }

    /// <summary>
    /// Provides an enumerator structure for iterating over the elements of a <see cref="RentedArray{T}"/>.
    /// </summary>
    public ref struct Enumerator(RentedArray<T> rentedArray)
    {
        private int _index;
        private readonly int _length = rentedArray._length;

        /// <summary>
        /// Gets the current element during iteration.
        /// </summary>
        public T Current { get; private set; } = default!;

        /// <summary>
        /// Advances the enumerator to the next element of the <see cref="RentedArray{T}"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the enumerator was successfully advanced to the next element; otherwise, <c>false</c>.
        /// </returns>
        public bool MoveNext()
        {
            var length = _length;
            if (length == 0 || _index >= length)
            {
                return false;
            }

            Current = rentedArray[_index++];
            return true;
        }
    }
}