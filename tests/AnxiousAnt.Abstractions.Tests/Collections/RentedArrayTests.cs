namespace AnxiousAnt.Collections;

public class RentedArrayTests
{
    [Fact]
    public void Uninitialized_ShouldNotHaveAnyElement()
    {
        // Arrange
        RentedArray<int> array = default;
        Action act = () => { _ = array.ArraySegment; };

        // Assert
        array.Length.ShouldBe(0);
        array.Array.ShouldBeNull();
        array.Span.Length.ShouldBe(0);
        array.Memory.Length.ShouldBe(0);
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void DefaultCtor_ShouldNotHaveAnyElement()
    {
        // Arrange
        RentedArray<int> array = new();
        Action act = () => { _ = array.ArraySegment; };

        // Assert
        array.Length.ShouldBe(0);
        array.Array.ShouldBeNull();
        array.Span.Length.ShouldBe(0);
        array.Memory.Length.ShouldBe(0);
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void This_ShouldThrowWhenGivenNegativeIndex()
    {
        // Arrange
        RentedArray<int> array = new();
        Action act = () => { _ = array[-1]; };

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void This_ShouldThrowWhenGivenIndexGreaterThanLength()
    {
        // Arrange
        RentedArray<int> array = RentedArray<int>.FromArray([3]);

        // Assert
        array[0].ShouldBe(3);
    }

    [Fact]
    public void FromArray_ShouldThrowWhenGivenNullArray()
    {
        // Arrange
        Action act = () => RentedArray<int>.FromArray(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void FromArray_ShouldReturnEmptyWhenGivenEmptyArray()
    {
        // Act
        var array = RentedArray<int>.FromArray([]);

        // Assert
        array.Array.ShouldBeSameAs(RentedArray<int>.Empty.Array);
    }

    [Fact]
    public void FromArray_ShouldNotRentWhenGivenEmptyArray()
    {
        // Act
        var countBefore = TrackingArrayPool<int>.Instance.Count;
        var array = RentedArray<int>.FromArray(TrackingArrayPool<int>.Instance, []);

        // Assert
        TrackingArrayPool<int>.Instance.Count.ShouldBe(countBefore);
        array.Array.ShouldBeSameAs(RentedArray<int>.Empty.Array);
    }

    [Fact]
    public void FromArray_ShouldCopyGivenArray()
    {
        // Arrange
        var arr = new[] { 1, 2, 3 };

        // Act
        var rented = RentedArray<int>.FromArray(arr);
        arr[0] = 4;

        // Assert
        rented.Length.ShouldBe(3);
        rented.Array[..3].ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void FromArray_ShouldReturnRentedArray()
    {
        // Arrange
        var arr = new[] { 1, 2, 3 };
        var countBefore = TrackingArrayPool<int>.Instance.Count;

        // Act
        var rented = RentedArray<int>.FromArray(TrackingArrayPool<int>.Instance, arr);
        arr[0] = 4;

        // Assert
        TrackingArrayPool<int>.Instance.Count.ShouldBe(countBefore + 1);
        rented.Length.ShouldBe(3);
        rented.Array[..3].ShouldBe([1, 2, 3]);
        rented.Dispose();

        TrackingArrayPool<int>.Instance.Count.ShouldBe(countBefore);
    }

    [Fact]
    public void FromSpan_ShouldReturnEmptyWhenGivenEmptyArray()
    {
        // Act
        var array = RentedArray<int>.FromSpan([]);

        // Assert
        array.Array.ShouldBeSameAs(RentedArray<int>.Empty.Array);
    }

    [Fact]
    public void FromSpan_ShouldNotRentWhenGivenEmptyArray()
    {
        // Act
        var countBefore = TrackingArrayPool<int>.Instance.Count;
        var array = RentedArray<int>.FromSpan(TrackingArrayPool<int>.Instance, []);

        // Assert
        TrackingArrayPool<int>.Instance.Count.ShouldBe(countBefore);
        array.Array.ShouldBeSameAs(RentedArray<int>.Empty.Array);
    }

    [Fact]
    public void FromSpan_ShouldCopyGivenArray()
    {
        // Arrange
        var arr = new[] { 1, 2, 3 };

        // Act
        var rented = RentedArray<int>.FromSpan(arr);
        arr[0] = 4;

        // Assert
        rented.Length.ShouldBe(3);
        rented.Array[..3].ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void FromSpan_ShouldReturnRentedArray()
    {
        // Arrange
        var arr = new[] { 1, 2, 3 };
        var countBefore = TrackingArrayPool<int>.Instance.Count;

        // Act
        var rented = RentedArray<int>.FromSpan(TrackingArrayPool<int>.Instance, arr);
        arr[0] = 4;

        // Assert
        TrackingArrayPool<int>.Instance.Count.ShouldBe(countBefore + 1);
        rented.Length.ShouldBe(3);
        rented.Array[..3].ShouldBe([1, 2, 3]);
        rented.Dispose();

        TrackingArrayPool<int>.Instance.Count.ShouldBe(countBefore);
    }

    [Fact]
    public void GetEnumerator_ShouldReturnEmptyEnumeratorForUninitializedList()
    {
        // Arrange
        RentedArray<int> list = RentedArray<int>.Empty;

        // Act
        using var enumerator = list.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void GetEnumerator_ShouldReturnEmptyEnumeratorForListWithNoElements()
    {
        // Arrange
        RentedArray<int> list = RentedArray<int>.Empty;

        // Act
        using var enumerator = list.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenRentedArrayIsUninitialized()
    {
        // Arrange
        RentedArray<int> array = default;

        // Act
        array.Dispose();
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenRentedArrayIsEmpty()
    {
        // Arrange
        RentedArray<int> array = new();

        // Act
        array.Dispose();
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenRentedArrayIsDisposed()
    {
        // Arrange
        var array = RentedArray<int>.FromArray([1, 2, 3]);

        // Act
        array.Dispose();
        array.Dispose();
    }

    [Fact]
    public void Dispose_ShouldClearArrayWhenTypeIsReferenceType()
    {
        // Arrange
        var rented = RentedArray<string>.FromArray(["1", "2", "3"]);
        var array = rented.Array;

        // Act
        rented.Dispose();

        // Assert
        rented.Array.ShouldBeNull();
        array[0].ShouldBeNull();
        array[1].ShouldBeNull();
        array[2].ShouldBeNull();
    }
}