namespace AnxiousAnt.Collections;

public class KeyValueCollectionTests
{
    [Fact]
    public void FirstOrDefault_ShouldReturnValueIfNameExists()
    {
        // Arrange
        var list = new KeyValueCollection<string> { { "Name1", "Value1" } };

        // Act
        var result = list.FirstOrDefault("Name1");

        // Assert
        result.ShouldBe("Value1");
    }

    [Fact]
    public void FirstOrDefault_ShouldReturnDefaultIfNameDoesNotExist()
    {
        // Arrange
        var list = new KeyValueCollection<string>();

        // Act
        var result = list.FirstOrDefault("Name1");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void FirstOrDefault_ShouldRespectCaseSensitivity()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: false) { { "Name1", "Value1" } };

        // Act
        var result = list.FirstOrDefault("name1");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void FirstOrDefault_ShouldIgnoreCaseWhenCaseSensitivityIsDisabled()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: true) { { "Name1", "Value1" } };

        // Act
        var result = list.FirstOrDefault("name1");

        // Assert
        result.ShouldBe("Value1");
    }

    [Fact]
    public void TryGetFirst_ShouldReturnTrueAndOutValueIfNameExists()
    {
        // Arrange
        var list = new KeyValueCollection<string> { { "Name1", "Value1" } };

        // Act
        var result = list.TryGetFirst("Name1", out var value);

        // Assert
        result.ShouldBeTrue();
        value.ShouldBe("Value1");
    }

    [Fact]
    public void TryGetFirst_ShouldReturnFalseAndDefaultValueIfNameDoesNotExist()
    {
        // Arrange
        var list = new KeyValueCollection<string>();

        // Act
        var result = list.TryGetFirst("Name1", out var value);

        // Assert
        result.ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Fact]
    public void TryGetFirst_ShouldRespectCaseSensitivity()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: false) { { "Name1", "Value1" } };

        // Act
        var result = list.TryGetFirst("name1", out var value);

        // Assert
        result.ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Fact]
    public void TryGetFirst_ShouldIgnoreCaseWhenCaseSensitivityIsDisabled()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: true) { { "Name1", "Value1" } };

        // Act
        var result = list.TryGetFirst("name1", out var value);

        // Assert
        result.ShouldBeTrue();
        value.ShouldBe("Value1");
    }

    [Fact]
    public void GetAll_ShouldReturnAllValuesForName()
    {
        // Arrange
        var list = new KeyValueCollection<string> { { "Name1", "Value1" }, { "Name1", "Value2" } };

        // Act
        var result = list.GetAll("Name1");

        // Assert
        result.ShouldBe(["Value1", "Value2"]);
    }

    [Fact]
    public void GetAll_ShouldRespectCaseSensitivity()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: false) { { "Name1", "Value1" }, { "Name1", "Value2" } };

        // Act
        var result = list.GetAll("name1");

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetAll_ShouldIgnoreCaseWhenCaseSensitivityIsDisabled()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: true) { { "Name1", "Value1" }, { "Name1", "Value2" } };

        // Act
        var result = list.GetAll("name1");

        // Assert
        result.ShouldBe(["Value1", "Value2"]);
    }

    [Fact]
    public void Contains_ShouldReturnTrueIfNameExists()
    {
        // Arrange
        var list = new KeyValueCollection<string> { { "Name1", "Value1" } };

        // Act
        var result = list.Contains("Name1");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Contains_ShouldReturnFalseIfNameDoesNotExist()
    {
        // Arrange
        var list = new KeyValueCollection<string>();

        // Act
        var result = list.Contains("Name1");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Contains_ShouldRespectCaseSensitivity()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: false) { { "Name1", "Value1" } };

        // Act
        var result = list.Contains("name1");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Contains_ShouldIgnoreCaseWhenCaseSensitivityIsDisabled()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: true) { { "Name1", "Value1" } };

        // Act
        var result = list.Contains("name1");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Add_ShouldAddNameValuePair()
    {
        // Arrange
        var list = new KeyValueCollection<string>();

        // Act
        list.Add("Name1", "Value1");

        // Assert
        Should.NotThrow(() => list.Single(item => item is { Key: "Name1", Value: "Value1" }));
    }

    [Fact]
    public void Add_ShouldRespectCaseSensitivity()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: false);

        // Act
        list.Add("Name1", "Value1");
        list.Add("name1", "Value2");

        // Assert
        list.Count.ShouldBe(2);
        list.ShouldContain(item => item.Key == "Name1" && item.Value == "Value1");
        list.ShouldContain(item => item.Key == "name1" && item.Value == "Value2");
    }

    [Fact]
    public void AddOrReplace_ShouldAddValueIfNameDoesNotExist()
    {
        // Arrange
        var list = new KeyValueCollection<string>();

        // Act
        list.AddOrReplace("Name1", "Value1");

        // Assert
        list.Count.ShouldBe(1);
        Should.NotThrow(() => list.Single(item => item is { Key: "Name1", Value: "Value1" }));
    }

    [Fact]
    public void AddOrReplace_ShouldReplaceValueIfNameExists()
    {
        // Arrange
        var list = new KeyValueCollection<string> { { "Name1", "Value1" } };

        // Act
        list.AddOrReplace("Name1", "Value2");

        // Assert
        list.Count.ShouldBe(1);
        Should.NotThrow(() => list.Single(item => item is { Key: "Name1", Value: "Value2" }));
    }

    [Fact]
    public void AddOrReplace_ShouldRespectCaseSensitivityWhenReplacing()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: false) { { "Name1", "Value1" } };

        // Act
        list.AddOrReplace("name1", "Value2");

        // Assert
        list.Count.ShouldBe(2);
        list.ShouldContain(item => item.Key == "Name1" && item.Value == "Value1");
        list.ShouldContain(item => item.Key == "name1" && item.Value == "Value2");
    }

    [Fact]
    public void AddOrReplace_ShouldIgnoreCaseWhenCaseSensitivityIsDisabled()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: true) { { "Name1", "Value1" } };

        // Act
        list.AddOrReplace("name1", "Value2");

        // Assert
        list.Count.ShouldBe(1);
        Should.NotThrow(() => list.Single(item => item is { Key: "name1", Value: "Value2" }));
    }

    [Fact]
    public void AddOrReplace_ShouldReplaceFirstMatchAndRemoveAnyAdditional()
    {
        // Arrange
        var list = new KeyValueCollection<string> { { "Name1", "Value1" }, { "Name1", "Value2" } };

        // Act
        list.AddOrReplace("Name1", "Value3");

        // Assert
        list.Count.ShouldBe(1);
        Should.NotThrow(() => list.Single(item => item is { Key: "Name1", Value: "Value3" }));
    }

    [Fact]
    public void AddOrReplace_ShouldReplaceFirstMatchAndRemoveAnyAdditionalIgnoringCaseSensitivity()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: true) { { "Name1", "Value1" }, { "Name1", "Value2" } };

        // Act
        list.AddOrReplace("name1", "Value3");

        // Assert
        list.Count.ShouldBe(1);
        Should.NotThrow(() => list.Single(item => item is { Key: "name1", Value: "Value3" }));
    }

    [Fact]
    public void Remove_ShouldRemoveNameValuePairByName()
    {
        // Arrange
        var list = new KeyValueCollection<string> { { "Name1", "Value1" } };

        // Act
        var result = list.Remove("Name1");

        // Assert
        result.ShouldBeTrue();
        list.ShouldBeEmpty();
    }

    [Fact]
    public void Remove_ShouldReturnFalseIfNameDoesNotExist()
    {
        // Arrange
        var list = new KeyValueCollection<string>();

        // Act
        var result = list.Remove("Name1");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Remove_ShouldRespectCaseSensitivity()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: false) { { "Name1", "Value1" } };

        // Act
        var result = list.Remove("name1");

        // Assert
        result.ShouldBeFalse();
        list.Count.ShouldBe(1);
    }

    [Fact]
    public void Remove_ShouldIgnoreCaseWhenCaseSensitivityIsDisabled()
    {
        // Arrange
        var list = new KeyValueCollection<string>(ignoreCase: true) { { "Name1", "Value1" } };

        // Act
        var result = list.Remove("name1");

        // Assert
        result.ShouldBeTrue();
        list.ShouldBeEmpty();
    }

    [Fact]
    public void AsReadOnly_ShouldReturnEmptyReadOnlyCollectionWhenEmpty()
    {
        // Arrange
        var collection = new KeyValueCollection<string>();

        // Act
        var instance = collection.AsReadOnly();

        // Assert
        instance.ShouldBeEmpty();
        instance.ShouldBeSameAs(ReadOnlyKeyValueCollection<string>.Empty);
    }

    [Fact]
    public void AsReadOnly_ShouldReturnReadOnlyCollectionWithSameValues()
    {
        // Arrange
        var collection = new KeyValueCollection<string> { { "Name1", "Value1" } };

        // Act
        var instance = collection.AsReadOnly();

        // Assert
        instance.ShouldNotBeEmpty();
        instance.ShouldNotBeSameAs(ReadOnlyKeyValueCollection<string>.Empty);
        instance.ShouldContain(item => item.Key == "Name1" && item.Value == "Value1");

        instance[0].ShouldBe(collection[0]);
        instance.GetAll("Name1").ShouldBe(collection.GetAll("Name1"));
        instance.Contains("Name1").ShouldBeTrue();
        instance.FirstOrDefault("Name1").ShouldBeSameAs(collection.FirstOrDefault("Name1"));
        instance.TryGetFirst("Name1", out var value).ShouldBeTrue();
        value.ShouldBe("Value1");
    }
}