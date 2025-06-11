using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics;

namespace AnxiousAnt;

public class CollectionExtensionsTests
{
    public static TheoryData<IEnumerable<string>> EmptyCollections =>
    [
        Enumerable.Empty<string>(),
        new List<string>(),
        Array.Empty<string>(),
        new HashSet<string>(),
        new List<string>().AsReadOnly(),
        new List<string>().ToFrozenSet(),
        new TestEnumerable<string>(),
        new TestCollection(),
        new TestReadOnlyCollection()
    ];

    public static TheoryData<IEnumerable<KeyValuePair<string, string>>> EmptyDictionaries =>
    [
        new Dictionary<string, string>(),
        [],
        new Dictionary<string, string>().AsReadOnly(),
        new Dictionary<string, string>().ToFrozenDictionary(),
        new TestEnumerable<KeyValuePair<string, string>>(),
        new TestDictionary(),
        new TestReadOnlyDictionary()
    ];

    [Fact]
    public void IsEmpty_ShouldThrowWhenGivenNullSource()
    {
        // Act
        Action act = () => CollectionExtensions.IsEmpty<int>(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(EmptyCollections))]
    public void IsEmpty_ShouldReturnTrueWhenGivenEmptyCollection(IEnumerable<string> collection)
    {
        // Act
        var result = collection.IsEmpty();

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(EmptyDictionaries))]
    public void IsEmpty_ShouldReturnTrueWhenGivenEmptyDictionary(IEnumerable<KeyValuePair<string, string>> dictionary)
    {
        // Act
        var result = dictionary.IsEmpty();

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEmpty_ShouldReturnFalseWhenGivenNonEmptyCollection()
    {
        // Arrange
        var list = new List<int> { 1, 2 };
        var dict = new Dictionary<int, int> { [0] = 1, [2] = 3 };

        // Assert
        list.IsEmpty().ShouldBeFalse();
        dict.IsEmpty().ShouldBeFalse();
    }

    [Fact]
    public void OrEmpty_ShouldReturnEmptyWhenGivenNull()
    {
        // Arrange
        List<string>? list = null;

        // Act
        var result = list.OrEmpty();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void OrEmpty_ShouldReturnSameWhenNotGivenNull()
    {
        // Arrange
        int[] array = [1, 2, 3];

        // Act
        var result = array.OrEmpty();

        // Assert
        result.ShouldBeSameAs(array);
    }

    [Fact]
    public void AsArray_ShouldThrowWhenGivenNullSource()
    {
        // Act
        Action act = () => CollectionExtensions.AsArray<int>(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void AsArray_ShouldReturnSameWhenGivenArray()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act
        var result = array.AsArray();

        // Assert
        result.ShouldBeSameAs(array);
    }

    [Fact]
    public void AsArray_ShouldReturnNewArrayWhenGivenEnumerable()
    {
        // Arrange
        IEnumerable<int> enumerable = [1, 2, 3];

        // Act
        var result = enumerable.AsArray();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeSameAs(enumerable);
    }

    [Fact]
    public void AddRange_ShouldThrowWhenGivenNullSource()
    {
        // Arrange
        Action act = () => CollectionExtensions.AddRange<int>(null!, []);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void AddRange_ShouldThrowWhenGivenNullItems()
    {
        // Arrange
        Action act = () => CollectionExtensions.AddRange(new List<int>(), null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void AddRange_ShouldAddItemsToSourceList()
    {
        // Arrange
        ICollection<int> list = new List<int>();

        // Act
        list.AddRange([1, 2, 3]);

        // Assert
        list.Count.ShouldBe(3);
    }

    [Fact]
    public void AddRange_ShouldAddItemsToSourceHash()
    {
        // Arrange
        ICollection<int> hash = new HashSet<int>();

        // Act
        hash.AddRange([1, 2, 3, 3]);

        // Assert
        hash.Count.ShouldBe(3);
    }

    private sealed class TestEnumerable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class TestCollection : ICollection<string>
    {
        public int Count => 0;
        public bool IsReadOnly => false;

        public void Add(string item)
        {
            throw new UnreachableException();
        }

        public void Clear()
        {
            throw new UnreachableException();
        }

        public bool Contains(string item)
        {
            throw new UnreachableException();
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new UnreachableException();
        }

        public bool Remove(string item)
        {
            throw new UnreachableException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new UnreachableException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private sealed class TestReadOnlyCollection : IReadOnlyCollection<string>
    {
        public int Count => 0;

        public IEnumerator<string> GetEnumerator()
        {
            throw new UnreachableException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private sealed class TestDictionary : IDictionary<string, string>
    {
        public int Count => 0;
        public bool IsReadOnly => false;
        public ICollection<string> Keys => [];
        public ICollection<string> Values => [];

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new UnreachableException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            throw new UnreachableException();
        }

        public void Clear()
        {
            throw new UnreachableException();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new UnreachableException();
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new UnreachableException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new UnreachableException();
        }

        public void Add(string key, string value)
        {
            throw new UnreachableException();
        }

        public bool ContainsKey(string key)
        {
            throw new UnreachableException();
        }

        public bool Remove(string key)
        {
            throw new UnreachableException();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            throw new UnreachableException();
        }

        public string this[string key]
        {
            get => throw new UnreachableException();
            set => throw new UnreachableException();
        }
    }

    private sealed class TestReadOnlyDictionary : IReadOnlyDictionary<string, string>
    {
        public int Count => 0;
        public IEnumerable<string> Keys => [];
        public IEnumerable<string> Values => [];

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new UnreachableException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            throw new UnreachableException();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            throw new UnreachableException();
        }

        public string this[string key] => throw new UnreachableException();
    }
}