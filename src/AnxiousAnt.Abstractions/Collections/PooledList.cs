using System.Collections;
using System.Diagnostics;

namespace AnxiousAnt.Collections;

/// <summary>
/// <para>
/// A stack-allocated list-like structure intended for scenarios where performance and memory efficiency
/// are critical.
/// </para>
/// <para>
/// This structure is primarily designed to minimize memory allocations by leveraging array pooling
/// and stack allocation, making it well-suited for high-performance applications.
/// </para>
/// </summary>
/// <typeparam name="T">The type of the elements stored in the <see cref="PooledList{T}"/>.</typeparam>
/// <remarks>
/// Adapted from
/// https://github.com/dotnet/runtime/blob/27604b57bd2e9c9aa46d005db7c4e387d461b5b6/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ValueListBuilder.cs
/// </remarks>
[DebuggerDisplay("Count = {Length}")]
public ref struct PooledList<T> : IEnumerable<T>
{
    private readonly ArrayPool<T>? _pool;
    private T[]? _arrayFromPool;
    private Span<T> _span;
    private int _pos;

    /// <summary>
    /// Creates a new instance of the <see cref="PooledList{T}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity of the list.</param>
    public PooledList(int capacity) : this(ArrayPool<T>.Shared, capacity)
    {
    }

    internal PooledList(ArrayPool<T> pool, int capacity) : this(pool)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        Grow(capacity);
    }

    internal PooledList(ArrayPool<T> pool)
    {
        ArgumentNullException.ThrowIfNull(pool);

        _pool = pool;
    }

    /// <summary>
    /// Gets or sets the current number of elements stored in the <see cref="PooledList{T}"/>.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the value is negative, zero, or
    /// exceeds the allocated capacity of the <see cref="PooledList{T}"/>.</exception>
    public int Length
    {
        get => _pos;
        set
        {
            var span = _span;
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, span.Length);

            _pos = value;
        }
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
            var span = _span;
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, span.Length);

            ref var local = ref span[index];
            return ref local!;
        }
    }

    /// <summary>
    /// Gets a span representing the contiguous region of memory currently in use
    /// within the <see cref="PooledList{T}"/>.
    /// </summary>
    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _span[.._pos];
    }

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> representing the current elements of the <see cref="PooledList{T}"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="Memory{T}"/> will span only the used portion of the underlying array.
    /// If the list is uninitialized or contains no elements, the returned memory will be empty.
    /// </remarks>
    public Memory<T> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _arrayFromPool.AsMemory(0, _pos);
    }

    /// <summary>
    /// Gets an <see cref="ArraySegment{T}"/> representing the current elements of the <see cref="PooledList{T}"/>.
    /// </summary>
    /// <remarks>
    /// The returned <see cref="ArraySegment{T}"/> provides access to the underlying memory for the elements
    /// currently stored in the list, starting at index 0 and with a count equal to the number of active elements.
    /// </remarks>
    public ArraySegment<T> ArraySegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(_arrayFromPool ?? [], 0, _pos);
    }

    internal int Capacity => _span.Length;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => ArraySegment.GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Appends an item to the end of the <see cref="PooledList{T}"/>.
    /// If there is insufficient space in the underlying storage, the list will resize automatically.
    /// </summary>
    /// <param name="item">The item to append to the list.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T item)
    {
        int pos = _pos;

        // Workaround for https://github.com/dotnet/runtime/issues/72004
        Span<T> span = _span;
        if ((uint)pos < (uint)span.Length)
        {
            span[pos] = item;
            _pos = pos + 1;
        }
        else
        {
            AddWithResize(item);
        }
    }

    /// <summary>
    /// Appends the elements from the specified readonly span to the <see cref="PooledList{T}"/>.
    /// </summary>
    /// <param name="source">The readonly span containing elements to append.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<T> source)
    {
        int pos = _pos;
        Span<T> span = _span;
        if (source.Length == 1 && (uint)pos < (uint)span.Length)
        {
            span[pos] = source[0];
            _pos = pos + 1;
        }
        else
        {
            AppendMultiChar(source);
        }
    }

    /// <summary>
    /// Removes and returns the last element of the list.
    /// </summary>
    /// <returns>The last element of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the list is empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop()
    {
        var pos = _pos;
        if (pos == 0)
        {
            throw new InvalidOperationException("Tried to pop an empty list");
        }

        _pos--;

        var span = _span;
        return span[pos - 1];
    }

    /// <summary>
    /// Converts the current contents of the <see cref="PooledList{T}"/> to a new array.
    /// </summary>
    /// <returns>
    /// An array containing all the elements of the <see cref="PooledList{T}"/> up to its current length.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray()
    {
        var array = _arrayFromPool;
        if (array is not { Length: > 0 } || _pos == 0)
        {
            return [];
        }

        var target = new T[_pos];
        Span.CopyTo(target.AsSpan());

        return target;
    }

    /// <summary>
    /// Converts the current <see cref="PooledList{T}"/> to a <see cref="RentedArray{T}"/> using its underlying span.
    /// </summary>
    /// <returns>
    /// A <see cref="RentedArray{T}"/> containing the elements of the current list.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RentedArray<T> ToRentedArray() => RentedArray<T>.FromSpan(Span);

    /// <summary>
    /// Returns a reference to the 0th element of the list.
    /// </summary>
    /// <returns>
    /// A reference to the 0th element of the list.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetPinnableReference()
    {
        // Ensure that the native code has just one forward branch that is predicted-not-taken.
        ref T ret = ref Unsafe.NullRef<T>();
        ref var array = ref _arrayFromPool;

        if (_pos > 0 && array is not null)
        {
            ret = ref array[0];
        }

        return ref ret;
    }

    /// <summary>
    /// Releases the resources used by the <see cref="PooledList{T}"/> instance, returning any pooled array to the
    /// shared array pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _arrayFromPool;
        if (toReturn is null)
        {
            return;
        }

        _arrayFromPool = null;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            toReturn.AsSpan(0, _pos).Clear();
        }

        var pool = _pool ?? ArrayPool<T>.Shared;
        pool.Return(toReturn);
    }

    // Hide uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        Debug.Assert(_pos == _span.Length);
        int pos = _pos;

        Grow();
        _span[pos] = item;
        _pos = pos + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AppendMultiChar(scoped ReadOnlySpan<T> source)
    {
        if ((uint)(_pos + source.Length) > (uint)_span.Length)
        {
            Grow(_span.Length - _pos + source.Length);
        }

        source.CopyTo(_span.Slice(_pos));
        _pos += source.Length;
    }

    // Note that consuming implementations depend on the list only growing if it's absolutely
    // required.  If the list is already large enough to hold the additional items be added,
    // it must not grow. The list is used in a number of places where the reference is checked
    // and it's expected to match the initial reference provided to the constructor if that
    // span was sufficiently large.
    private void Grow(int additionalCapacityRequired = 1)
    {
        const int ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Double the size of the span.  If it's currently empty, default to size 4,
        // although it'll be increased in Rent to the pool's minimum bucket size.
        int nextCapacity = Math.Max(
            _span.Length != 0 ? _span.Length * 2 : 4,
            _span.Length + additionalCapacityRequired
        );

        // If the computed doubled capacity exceeds the possible length of an array, then we
        // want to downgrade to either the maximum array length if that's large enough to hold
        // an additional item, or the current length + 1 if it's larger than the max length, in
        // which case it'll result in an OOM when calling Rent below.  In the exceedingly rare
        // case where _span.Length is already int.MaxValue (in which case it couldn't be a managed
        // array), just use that same value again and let it OOM in Rent as well.
        if ((uint)nextCapacity > ArrayMaxLength)
        {
            nextCapacity = Math.Max(Math.Max(_span.Length + 1, ArrayMaxLength), _span.Length);
        }

        var pool = _pool ?? ArrayPool<T>.Shared;
        T[] array = pool.Rent(nextCapacity);
        _span.CopyTo(array);

        T[]? toReturn = _arrayFromPool;
        _span = _arrayFromPool = array;
        if (toReturn is null)
        {
            return;
        }

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            toReturn.AsSpan(0, _pos).Clear();
        }

        pool.Return(toReturn);
    }
}