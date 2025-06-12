namespace AnxiousAnt.Collections;

public class PooledListTests
{
    [Fact]
    public void Uninitialized_ShouldHaveCapacityAndLengthOfZero()
    {
        // Arrange
        PooledList<int> list = default;

        // Assert
        list.Capacity.ShouldBe(0);
        list.Length.ShouldBe(0);
    }

    [Fact]
    public void DefaultCtor_ShouldHaveCapacityAndLengthOfZero()
    {
        // Arrange
        PooledList<int> list = new();

        // Assert
        list.Capacity.ShouldBe(0);
        list.Length.ShouldBe(0);
    }

    [Fact]
    public void Ctor_ShouldHaveCapacityOfAtLeastGivenValue()
    {
        // Arrange
        const int capacity = 10;
        PooledList<int> list = new(capacity);

        // Assert
        list.Capacity.ShouldBeGreaterThanOrEqualTo(capacity);
    }

    [Fact]
    public void Length_ShouldReturnZeroWhenListIsEmpty()
    {
        // Arrange
        PooledList<int> list = new();

        // Assert
        list.Length.ShouldBe(0);
    }

    [Fact]
    public void Length_ShouldThrowWhenSettingToNegativeValue()
    {
        // Arrange
        Action act = () => new PooledList<int> { Length = -1 };

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Length_ShouldThrowWhenSettingToValueGreaterThanCapacity()
    {
        // Arrange
        Action act = () => new PooledList<int>(1) { Length = 100 };

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Length_ShouldUpdateTheLengthOfTheList()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Append(4);
        list.Length = 0;

        // Assert
        list.Length.ShouldBe(0);
    }

    [Fact]
    public void Memory_ShouldReturnEmptyForUninitializedList()
    {
        // Arrange
        PooledList<int> list = default;

        // Act
        var memory = list.Memory;

        // Assert
        memory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Memory_ShouldReturnEmptyForListWithNoElements()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        var memory = list.Memory;

        // Assert
        memory.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Memory_ShouldReturnMemoryForListElements()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Append(1);
        var memory = list.Memory;

        // Assert
        memory.Length.ShouldBe(1);
        memory.Span[0].ShouldBe(1);
    }

    [Fact]
    public void ArraySegment_ShouldReturnEmptyForUninitializedList()
    {
        // Arrange
        PooledList<int> list = default;

        // Act
        var segment = list.ArraySegment;

        // Assert
        segment.Count.ShouldBe(0);
    }

    [Fact]
    public void ArraySegment_ShouldReturnEmptyForListWithNoElements()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        var segment = list.ArraySegment;

        // Assert
        segment.Count.ShouldBe(0);
    }

    [Fact]
    public void ArraySegment_ShouldReturnMemoryForListElements()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Append(1);
        var segment = list.ArraySegment;

        // Assert
        segment.Count.ShouldBe(1);
        segment[0].ShouldBe(1);
    }

    [Fact]
    public void GetEnumerator_ShouldReturnEmptyEnumeratorForUninitializedList()
    {
        // Arrange
        PooledList<int> list = default;

        // Act
        using var enumerator = list.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void GetEnumerator_ShouldReturnEmptyEnumeratorForListWithNoElements()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        using var enumerator = list.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void Append_Single_ShouldGrowListWhenListIsUninitialized()
    {
        // Arrange
        PooledList<int> list = default;

        // Act
        list.Append(1);

        // Assert
        list.Capacity.ShouldBeGreaterThanOrEqualTo(1);
        list.Length.ShouldBe(1);
    }

    [Fact]
    public void Append_Single_ShouldGrowListWhenCreatedWithDefaultCtor()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Append(1);

        // Assert
        list.Capacity.ShouldBeGreaterThanOrEqualTo(1);
        list.Length.ShouldBe(1);
    }

    [Fact]
    public void Append_Single_ShouldGrowListWhenCapacityIsReached()
    {
        // Arrange
        PooledList<int> list = new(1);

        // Act
        var capacity = list.Capacity;
        for (var i = 0; i < capacity; i++)
        {
            list.Append(i);
        }

        list.Append(42);

        // Assert
        list.Capacity.ShouldBeGreaterThanOrEqualTo(capacity);
        list.Length.ShouldBe(capacity + 1);
    }

    [Fact]
    public void Append_Multiple_ShouldGrowListWhenListIsUninitialized()
    {
        // Arrange
        PooledList<int> list = default;

        // Act
        list.Append([1]);

        // Assert
        list.Capacity.ShouldBeGreaterThanOrEqualTo(1);
        list.Length.ShouldBe(1);
    }

    [Fact]
    public void Append_Multiple_ShouldGrowListWhenCreatedWithDefaultCtor()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Append([1]);

        // Assert
        list.Capacity.ShouldBeGreaterThanOrEqualTo(1);
        list.Length.ShouldBe(1);
    }

    [Fact]
    public void Append_Multiple_ShouldGrowListWhenCapacityIsReached()
    {
        // Arrange
        PooledList<int> list = new(1);

        // Act
        var capacity = list.Capacity;
        list.Append(Enumerable.Range(0, capacity).ToArray());

        list.Append([42]);
        list.Append([42]);

        // Assert
        list.Capacity.ShouldBeGreaterThanOrEqualTo(capacity);
        list.Length.ShouldBe(capacity + 2);
    }

    [Fact]
    public void Append_ShouldCleanupPreviousBucketOnGrow()
    {
        // Arrange
        PooledList<string> list = new();

        // Act
        list.Append("hello");
        var capacity = list.Capacity;
        var spanBeforeGrow = list.Span;

        list.Append(Enumerable.Range(0, capacity).Select(i => i.ToString()).ToArray());
        list.Append("world");

        // Assert
        list.Length.ShouldBe(capacity + 2);
        spanBeforeGrow[0].ShouldBeNull();
        list.ToArray().ShouldBe(["hello", ..Enumerable.Range(0, capacity).Select(i => i.ToString()), "world"]);
    }

    [Fact]
    public void Pop_ShouldThrowWhenListIsEmpty()
    {
        // Arrange
        Action act = () =>
        {
            PooledList<int> list = new();
            list.Pop();
        };

        // Assert
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Pop_ShouldReturnLastElementAndShrinkList()
    {
        // Arrange
        PooledList<int> list = new();

        // Arrange
        list.Append(1);
        var item = list.Pop();

        // Assert
        item.ShouldBe(1);
        list.Length.ShouldBe(0);
        list.Capacity.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ToArray_ShouldReturnEmptyArrayWhenListIsEmpty()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        var array = list.ToArray();

        // Assert
        array.ShouldBeEmpty();
        array.ShouldBeSameAs(Array.Empty<int>());
    }

    [Fact]
    public void ToArray_ShouldReturnArrayOfListElements()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Append(1);
        var array = list.ToArray();

        // Assert
        array.ShouldBe([1]);
    }

    [Fact]
    public void GetPinnableReference_ShouldReturnPinnedReferenceForListElements()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Append(1);
        list.Append(2);
        ref var reference = ref list.GetPinnableReference();

        // Assert
        reference.ShouldBe(list[0]);
        Unsafe.Add(ref reference, 0).ShouldBe(1);
        Unsafe.Add(ref reference, 1).ShouldBe(2);
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenListIsUninitialized()
    {
        // Arrange
        PooledList<int> list = default;

        // Assert
        list.Dispose();
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenUsingDefaultCtor()
    {
        // Arrange
        PooledList<int> list = new();

        // Assert
        list.Dispose();
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenCalledMultipleTimes()
    {
        // Arrange
        PooledList<int> list = new();

        // Act
        list.Dispose();
        list.Dispose();
    }

    [Fact]
    public void Dispose_ReturnArrayWhenDisposed()
    {
        // Arrange
        PooledList<string> list = new();

        // Act
        list.Append("hello");
        list.Dispose();

        // Assert
        list[0].ShouldBeNull();
    }

    [Fact]
    public void ShouldNeverOwnMoreThanOneRentedArray()
    {
        // Arrange
        var pool = new TrackingArrayPool<int>();
        PooledList<int> list = new(pool);

        // Act
        for (var i = 0; i < 100; i++)
        {
            list.Append(i);
        }

        // Assert
        pool.Count.ShouldBe(1);
        list.Dispose();
        pool.Count.ShouldBe(0);
    }
}