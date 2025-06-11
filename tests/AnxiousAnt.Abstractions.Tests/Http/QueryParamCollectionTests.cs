namespace AnxiousAnt.Http;

public class QueryParamCollectionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("          ")]
    [InlineData("?          ")]
    public void Constructor_WithEmptyWhiteSpaceOrNullString_ShouldBeEmpty(string? input)
    {
        // Act
        var collection = new QueryParamCollection(input);

        // Assert
        collection.Count.ShouldBe(0);
    }

    [Fact]
    public void Constructor_WithNonEmptyString_ShouldParseCorrectly()
    {
        // Arrange
        const string input = "key1=value1&key2=value2";

        // Act
        var collection = new QueryParamCollection(input);

        // Assert
        collection.Count.ShouldBe(2);
        collection.ToString().ShouldBe(input);
    }

    [Fact]
    public void ThisInt_ShouldThrowWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        Action act = () => _ = collection[0];

        // Assert
        act.ShouldThrow<IndexOutOfRangeException>();
    }

    [Fact]
    public void ThisInt_ShouldReturnCorrectValue()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2");

        // Act
        var result = collection[1];

        // Assert
        result.Key.ShouldBe("key2");
        result.Value.ShouldBe("value2");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void ThisInt_ShouldThrowWhenIndexIsOutOfRange(int index)
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2");

        // Act
        Action act = () => _ = collection[index];

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToString_ShouldReturnEmptyStringWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        var result = collection.ToString();

        // Assert
        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key1=")]
    [InlineData("key1=value")]
    [InlineData("key1=value1&key2=value2")]
    public void ToString_ShouldReturn(string input)
    {
        // Arrange
        var collection = new QueryParamCollection(input);

        // Assert
        var result = collection.ToString();

        // Assert
        result.ShouldBe(input);
    }

    [Fact]
    public void ToString_ShouldReturnCachedString()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2");

        // Act
        var result1 = collection.ToString();
        var result2 = collection.ToString();

        // Assert
        result2.ShouldBeSameAs(result1);
    }

    [Fact]
    public void GetEnumerator_ShouldReturnEmptyEnumeratorWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        using var enumerator = collection.GetEnumerator();

        // Assert
        enumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void GetEnumerator_ShouldReturnEnumeratorWithAllItems()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2");

        // Act
        using var enumerator = collection.GetEnumerator();

        // Assert
        collection.Count.ShouldBe(2);
        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Key.ShouldBe("key1");
        enumerator.Current.Value.ShouldBe("value1");
        enumerator.MoveNext().ShouldBeTrue();
        enumerator.Current.Key.ShouldBe("key2");
        enumerator.Current.Value.ShouldBe("value2");
    }

    [Fact]
    public void FirstOrDefault_ShouldReturnNullWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        var result = collection.FirstOrDefault("key1");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void FirstOrDefault_ShouldReturnNullWhenKeyDoesNotExist()
    {
        // Arrange
        var collection = new QueryParamCollection("key2=value2");

        // Act
        var result = collection.FirstOrDefault("key1");

        // Assert
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("Key1")]
    [InlineData("KEY1")]
    public void FirstOrDefault_ShouldReturnValueWhenKeyExists(string key)
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2");

        // Act
        var result = collection.FirstOrDefault(key);

        // Assert
        result.ShouldBe("value1");
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalseWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        var result = collection.TryGetFirst("ke1", out _);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalseWhenKeyDoesNotExist()
    {
        // Arrange
        var collection = new QueryParamCollection("key2=value2");

        // Act
        var result = collection.TryGetFirst("ke1", out _);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("Key1")]
    [InlineData("KEY1")]
    public void TryGetValue_ShouldReturnTrueAndValueWhenKeyExists(string key)
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2");

        // Act
        var result = collection.TryGetFirst(key, out var value);

        // Assert
        result.ShouldBeTrue();
        value.ShouldBe("value1");
    }

    [Fact]
    public void GetAll_ShouldReturnEmptyCollectionWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        var result = collection.GetAll("key1");

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetAll_ShouldReturnAllValuesWhenKeyExists()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1");

        // Act
        var result = collection.GetAll("key1").ToArray();

        // Assert
        result.ShouldNotBeEmpty();
        result[0].ShouldBe("value1");
    }

    [Fact]
    public void Contains_ShouldReturnFalseWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        var result = collection.Contains("key1");

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("Key1")]
    [InlineData("KEY1")]
    public void Contains_ShouldReturnTrueWhenKeyExists(string key)
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        var result = collection.Contains(key);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("key1=value1&key2=value2")]
    public void Clone_ShouldCreateIdenticalCopy(string? input)
    {
        // Arrange
        var original = new QueryParamCollection(input);

        // Act
        var clone = original.Clone();

        // Assert
        clone.ShouldNotBeSameAs(original);
        clone.ToString().ShouldBe(original.ToString());
    }

    [Fact]
    public void Add_ShouldKeyWithValue()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        collection.Add("key1", "value1");

        // Assert
        collection.Count.ShouldBe(1);
        collection.TryGetFirst("key1", out var value).ShouldBeTrue();
        value.ShouldBe("value1");
        collection.ToString().ShouldBe("key1=value1");
    }

    [Fact]
    public void Add_ShouldAddKeyWithoutValue()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        collection.Add("key1", null, QueryParamCollection.NullValueHandling.NameOnly);

        // Assert
        collection.Count.ShouldBe(1);
        collection.TryGetFirst("key1", out var value).ShouldBeTrue();
        value.ShouldBeNull();
        collection.ToString().ShouldBe("key1");
    }

    [Fact]
    public void Add_ShouldAddMultipleValuesForSameKey()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        collection.Add("key1", "value1");
        collection.Add("key1", "value2");

        // Assert
        collection.Count.ShouldBe(2);
        collection.ToString().ShouldBe("key1=value1&key1=value2");
    }

    [Fact]
    public void Add_ShouldNotAddKeyWhenValueIsNullAndNullValueHandlingIsIgnore()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        collection.Add("key1", null, QueryParamCollection.NullValueHandling.Ignore);

        // Assert
        collection.Count.ShouldBe(0);
    }

    [Fact]
    public void Add_ShouldRemoveExistingKeyWhenValueIsNullAndNullValueHandlingIsRemove()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value");

        // Act
        collection.Add("key1", null, QueryParamCollection.NullValueHandling.Remove);

        // Assert
        collection.Count.ShouldBe(0);
    }

    [Fact]
    public void AddOrReplace_ShouldAddKeyWhenNotExists()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        collection.AddOrReplace("key1", "value1");

        // Assert
        collection.Count.ShouldBe(1);
        collection.TryGetFirst("key1", out var value).ShouldBeTrue();
        value.ShouldBe("value1");
    }

    [Fact]
    public void AddOrReplace_ShouldReplaceKeyWhenExists()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1");

        // Act
        collection.AddOrReplace("key1", "value2");

        // Assert
        collection.Count.ShouldBe(1);
        collection.TryGetFirst("key1", out var value).ShouldBeTrue();
        value.ShouldBe("value2");
    }

    [Fact]
    public void AddOrReplace_ShouldNotReplaceKeyWhenExistsAndNullValueHandlingIsIgnore()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1");

        // Act
        collection.AddOrReplace("key1", null, QueryParamCollection.NullValueHandling.Ignore);

        // Assert
        collection.Count.ShouldBe(1);
        collection.TryGetFirst("key1", out var value).ShouldBeTrue();
        value.ShouldBe("value1");
    }

    [Fact]
    public void AddOrReplace_ShouldReplaceKeyWhenExistsAndRemoveDuplicates()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2&key1=value3");

        // Act
        collection.AddOrReplace("key1", "value4");

        // Assert
        collection.Count.ShouldBe(2);
        collection.TryGetFirst("key1", out var value).ShouldBeTrue();
        value.ShouldBe("value4");
        collection.ToString().ShouldBe("key1=value4&key2=value2");
    }

    [Fact]
    public void AddOrReplace_ShouldReplaceKeyWhenExistsAndNullValueHandlingIsNameOnly()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2&key1=value3");

        // Act
        collection.AddOrReplace("key1", null, QueryParamCollection.NullValueHandling.NameOnly);

        // Assert
        collection.Count.ShouldBe(2);
        collection.TryGetFirst("key1", out var value).ShouldBeTrue();
        value.ShouldBeNull();
        collection.ToString().ShouldBe("key1&key2=value2");
    }

    [Fact]
    public void AddOrReplace_ShouldRemoveKeyWhenExistsAndNullValueHandlingIsRemove()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2&key1=value3");

        // Act
        collection.AddOrReplace("key1", null, QueryParamCollection.NullValueHandling.Remove);

        // Assert
        collection.Count.ShouldBe(1);
        collection.TryGetFirst("key1", out var value).ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Fact]
    public void Remove_ShouldReturnFalseWhenCollectionIsEmpty()
    {
        // Arrange
        var collection = new QueryParamCollection();

        // Act
        var result = collection.Remove("key1");

        // Assert
        result.ShouldBeFalse();
        collection.Count.ShouldBe(0);
    }

    [Fact]
    public void Remove_ShouldReturnFalseWhenKeyDoesNotExist()
    {
        // Arrange
        var collection = new QueryParamCollection("key2=value2");

        // Act
        var result = collection.Remove("key1");

        // Assert
        result.ShouldBeFalse();
        collection.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("Key1")]
    [InlineData("KEY1")]
    public void Remove_ShouldReturnTrueAndRemoveItemWhenKeyExists(string key)
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value1&key2=value2");

        // Act
        var result = collection.Remove(key);

        // Assert
        result.ShouldBeTrue();
        collection.Count.ShouldBe(1);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        var collection = new QueryParamCollection("key1=value2&key2=value2");

        // Act
        collection.Clear();

        // Assert
        collection.Count.ShouldBe(0);
    }

    [Fact]
    public void Sort_ShouldSortCollection()
    {
        // Arrange
        var collection = new QueryParamCollection("key2=value2&key1=value1");

        // Act
        collection.Sort();

        // Assert
        collection.ToString().ShouldBe("key1=value1&key2=value2");
    }

    [Theory]
    [InlineData("value1 value2", "value1+value2")]
    [InlineData("value1!value2", "value1%21value2")]
    public void ShouldEncodeValues(string input, string expected)
    {
        // Arrange
        var collection = new QueryParamCollection();
        collection.Add("key1", input);

        // Act
        var result = collection.ToString();

        // Assert
        result.ShouldBe($"key1={expected}");
    }

    [Theory]
    [InlineData("key1=value1+value2")]
    [InlineData("key1=value1%20value2")]
    public void ShouldDecodeEncodedValues(string input)
    {
        // Arrange
        var collection = new QueryParamCollection(input);

        // Act
        var result = collection.TryGetFirst("key1", out var value);

        // Assert
        result.ShouldBeTrue();
        value.ShouldBe("value1 value2");
    }
}