namespace AnxiousAnt.Collections;

internal sealed class TrackingArrayPool<T> : ArrayPool<T>
{
    private readonly List<T[]> _rentedArrays = [];

    public static TrackingArrayPool<T> Instance { get; } = new();

    public int Count => _rentedArrays.Count;

    /// <inheritdoc />
    public override T[] Rent(int minimumLength)
    {
        var arr = Shared.Rent(minimumLength);
        _rentedArrays.Add(arr);

        return arr;
    }

    /// <inheritdoc />
    public override void Return(T[] array, bool clearArray = false)
    {
        Shared.Return(array, clearArray);
        _rentedArrays.Remove(array);
    }
}