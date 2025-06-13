namespace AnxiousAnt;

public partial class StringExtensionsTests
{
    [Fact]
    public void IfNullOrWhiteSpace_ShouldReturnOriginalString_WhenStringIsNotNullOrWhiteSpace()
    {
        // Arrange
        const string original = "test";
        const string defaultValue = "default";

        // Act
        var result = original.IfNullOrWhiteSpace(defaultValue);

        // Assert
        result.ShouldBe(original);
    }

    [Fact]
    public void IfNullOrWhiteSpace_ShouldReturnDefaultValue_WhenStringIsNullWhiteSpace()
    {
        // Arrange
        const string original = " ";
        const string defaultValue = "default";

        // Act
        var result = original.IfNullOrWhiteSpace(defaultValue);

        // Assert
        result.ShouldBe(defaultValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void JsonEscape_ShouldReturnEmptyWhenGivenNullOrEmpty(string? input)
    {
        // Act
        var result = input.JsonEscape();

        // Assert
        result.ShouldBeSameAs(string.Empty);
    }

    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData("Hello \"World\"", "Hello \\\"World\\\"")]
    [InlineData("Hello\\World", @"Hello\\World")]
    [InlineData("Hello\tWorld", "Hello\\tWorld")]
    [InlineData("Hello\nWorld", "Hello\\nWorld")]
    [InlineData("Hello\rWorld", "Hello\\rWorld")]
    [InlineData("\b\f\t\n\r", @"\b\f\t\n\r")]
    public void JsonEscape_ShouldEscapeProperly(string? input, string expected)
    {
        // Act
        var result = input.JsonEscape();

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void JsonUnescape_ShouldReturnInputWhenGivenNullEmptyOrWhiteSpace(string? input)
    {
        // Act
        var result = input.JsonUnescape();

        // Assert
        result.ShouldBeSameAs(input);
    }

    [Fact]
    public void JsonUnescape_NullStringShouldReturnNull()
    {
        // Act
        var result = "null".JsonUnescape();

        // Assert
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData("Hello \\\"World\\\"", "Hello \"World\"")]
    [InlineData(@"Hello\\World", "Hello\\World")]
    [InlineData("Hello\\tWorld", "Hello\tWorld")]
    [InlineData("Hello\\nWorld", "Hello\nWorld")]
    [InlineData("Hello\\rWorld", "Hello\rWorld")]
    [InlineData(@"\b\f\t\n\r", "\b\f\t\n\r")]
    public void JsonUnescape_ShouldUnescapeProperly(string input, string expected)
    {
        // Act
        var result = input.JsonUnescape();

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AsLowerInvariant_ShouldReturnEmptyWhenGivenNullOrEmpty(string? input)
    {
        // Act
        var result = input.AsLowerInvariant();

        // Assert
        result.ShouldBeSameAs(string.Empty);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("")]
    [InlineData("         ")]
    public void AsLowerInvariant_ShouldReturnInputWhenEmptyOrLowerCase(string input)
    {
        // Act
        var result = input.AsLowerInvariant();

        // Assert
        result.ShouldBeSameAs(input);
    }

    [Theory]
    [InlineData("TEST", "test")]
    [InlineData("Test", "test")]
    [InlineData("test", "test")]
    [InlineData("123", "123")]
    [InlineData("123TEST", "123test")]
    [InlineData("TEST!", "test!")]
    [InlineData("!@#$%^", "!@#$%^")]
    [InlineData("tESt teST", "test test")]
    public void AsLowerInvariant_ShouldReturnExpectedResult(string? input, string expected)
    {
        // Act
        var result = input.AsLowerInvariant();

        // Assert
        result.ShouldBe(expected);
    }
}