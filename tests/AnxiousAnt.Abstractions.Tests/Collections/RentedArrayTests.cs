namespace AnxiousAnt.Collections;

public class RentedArrayTests
{
    [Fact]
    public void Uninitialized_ShouldNotHaveAnyElement()
    {
        // Arrange
        RentedArray<int> rentedArray = default;

        // Assert
        rentedArray.Length.ShouldBe(0);
        rentedArray.GetArray().ShouldBeNull();
        rentedArray.Span.Length.ShouldBe(0);
        rentedArray.Memory.Length.ShouldBe(0);
    }

    [Fact]
    public void DefaultCtor_ShouldNotHaveAnyElement()
    {
        // Arrange
        RentedArray<int> rentedArray = new();

        // Assert
        rentedArray.Length.ShouldBe(0);
        rentedArray.GetArray().ShouldBeNull();
        rentedArray.Span.Length.ShouldBe(0);
        rentedArray.Memory.Length.ShouldBe(0);
    }

    [Fact]
    public void This_ShouldThrowWhenGivenNegativeIndex()
    {
        // Arrange
        RentedArray<int> rentedArray = new();
        Action act = () => { _ = rentedArray[-1]; };

        // Assert
        act.ShouldThrow<IndexOutOfRangeException>();
    }

    [Fact]
    public void This_ShouldThrowWhenGivenIndexGreaterThanLength()
    {
        // Arrange
        RentedArray<int> rentedArray = RentedArray<int>.FromArray([1]);
        Action act = () => { _ = rentedArray[1]; };

        // Assert
        act.ShouldThrow<IndexOutOfRangeException>();
    }

    [Fact]
    public void This_ShouldReturnElementAtIndex()
    {
        // Arrange
        RentedArray<int> rentedArray = RentedArray<int>.FromArray([3]);

        // Assert
        rentedArray[0].ShouldBe(3);
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
        var rentedArray = RentedArray<int>.FromArray([]);

        // Assert
        rentedArray.GetArray().ShouldBeSameAs(RentedArray<int>.Empty.GetArray());
    }

    [Fact]
    public void FromSpan_ShouldReturnEmptyWhenGivenEmptyArray()
    {
        // Act
        var rentedArray = RentedArray<int>.FromSpan([]);

        // Assert
        rentedArray.GetArray().ShouldBeSameAs(RentedArray<int>.Empty.GetArray());
    }

    [Fact]
    public void FromSpan_ShouldNotRentWhenGivenEmptyArray()
    {
        // Act
        var pool = new TrackingArrayPool<int>();
        var rentedArray = RentedArray<int>.FromSpan(pool, []);

        // Assert
        pool.Count.ShouldBe(0);
        rentedArray.GetArray().ShouldBeSameAs(RentedArray<int>.Empty.GetArray());
    }

    [Fact]
    public void FromSpan_ShouldCopyGivenArray()
    {
        // Arrange
        var arr = new[] { 1, 2, 3 };

        // Act
        var rentedArray = RentedArray<int>.FromSpan(arr);
        arr[0] = 4;

        // Assert
        rentedArray.Length.ShouldBe(3);
        rentedArray.GetArray()![..3].ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void FromSpan_ShouldReturnRentedArray()
    {
        // Arrange
        var arr = new[] { 1, 2, 3 };
        var pool = new TrackingArrayPool<int>();

        // Act
        var rentedArray = RentedArray<int>.FromSpan(pool, arr);
        arr[0] = 4;

        // Assert
        pool.Count.ShouldBe(1);
        rentedArray.Length.ShouldBe(3);
        rentedArray.ToArray().ShouldBe([1, 2, 3]);
        rentedArray.Dispose();

        pool.Count.ShouldBe(0);
    }

    [Fact]
    public void Dispose_Uninitialized_ShouldNotThrow()
    {
        // Arrange
        RentedArray<int> rentedArray = default;

        // Act
        rentedArray.Dispose();
    }

    [Fact]
    public void Dispose_DefaultCtor_ShouldNotThrow()
    {
        // Arrange
        RentedArray<int> rentedArray = new();

        // Act
        rentedArray.Dispose();
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenRentedArrayIsDisposed()
    {
        // Arrange
        var rentedArray = RentedArray<int>.FromArray([1, 2, 3]);

        // Act
        rentedArray.Dispose();
        rentedArray.Dispose();
    }

    [Fact]
    public void Dispose_ShouldClearArrayWhenTypeIsReferenceType()
    {
        // Arrange
        var rentedArray = RentedArray<string>.FromArray(["1", "2", "3"]);
        var array = rentedArray.GetArray()!;

        // Act
        rentedArray.Dispose();

        // Assert
        rentedArray.GetArray().ShouldBeNull();
        array[0].ShouldBeNull();
        array[1].ShouldBeNull();
        array[2].ShouldBeNull();
    }

    [Fact]
    public void GetEnumerator_Uninitialized_ShouldReturnEmptyEnumerator()
    {
        // Arrange
        RentedArray<int> rentedArray = default;

        // Act
        var enumerator = rentedArray.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void GetEnumerator_DefaultCtor_ShouldReturnEmptyEnumerator()
    {
        // Arrange
        RentedArray<int> rentedArray = new();

        // Act
        var enumerator = rentedArray.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void GetEnumerator_ShouldReturnEmptyEnumeratorForListWithNoElements()
    {
        // Arrange
        RentedArray<int> rentedArray = RentedArray<int>.Empty;

        // Act
        var enumerator = rentedArray.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void GetArray_Uninitialized_ShouldReturnNull()
    {
        // Arrange
        RentedArray<int> rentedArray = default;

        // Assert
        rentedArray.GetArray().ShouldBeNull();
    }

    [Fact]
    public void GetArray_DefaultCtor_ShouldReturnNull()
    {
        // Arrange
        RentedArray<int> rentedArray = new();

        // Assert
        rentedArray.GetArray().ShouldBeNull();
    }

    [Fact]
    public void GetArray_ShouldReturnUnderlyingArray()
    {
        // Arrange
        var rentedArray = RentedArray<int>.FromArray([1, 2, 3]);

        // Act
        var array = rentedArray.GetArray()!;

        // Assert
        array[..3].ShouldBe([1, 2, 3]);
        array.ShouldBeSameAs(rentedArray.GetArray());
    }

    [Fact]
    public void ToArray_Uninitialized_ShouldReturnEmptyArray()
    {
        // Arrange
        RentedArray<int> rentedArray = default;

        // Act
        var array = rentedArray.ToArray();

        // Assert
        array.ShouldBeEmpty();
        array.ShouldBeSameAs(RentedArray<int>.Empty.GetArray());
    }

    [Fact]
    public void ToArray_DefaultCtor_ShouldReturnEmptyArray()
    {
        // Arrange
        RentedArray<int> rentedArray = default;

        // Act
        var array = rentedArray.ToArray();

        // Assert
        array.ShouldBeEmpty();
        array.ShouldBeSameAs(RentedArray<int>.Empty.GetArray());
    }

    [Fact]
    public void ToArray_ShouldReturnArrayWithSameElements()
    {
        // Arrange
        var rentedArray = RentedArray<int>.FromArray([1, 2, 3]);

        // Act
        var array = rentedArray.ToArray();

        // Assert
        array.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void Enumerator_SmokeTest()
    {
        // Arrange
        var rentedArray = RentedArray<int>.FromArray([1, 2, 3]);
        var result = new List<int>();

        // Act
        foreach (var item in rentedArray)
        {
            result.Add(item);
        }

        // Assert
        result.ShouldBe([1, 2, 3]);
    }
}