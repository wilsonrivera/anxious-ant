namespace AnxiousAnt;

partial class StringExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Truncate_ShouldReturnEmptyStringWhenProvidedStringIsNullOrEmpty(string? input)
    {
        // Act
        var truncated = input.Truncate(50);
        var truncatedSpan = input.AsSpan().Truncate(50);

        // Assert
        truncated.ShouldBeEmpty();
        truncatedSpan.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Truncate_ShouldThrowWhenProvidedMaximumLengthIsNegativeOrZero(int maxLength)
    {
        // Arrange
        const string input = "Hello World";
        Func<string> act = () => input.Truncate(maxLength);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Truncate_ShouldThrowWhenProvideMaximumLengthIsTooShort()
    {
        // Arrange
        const string input = "Hello World";
        Func<string> act = () => input.Truncate(1, "...");

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Truncate_ShouldUseDefaultOmissionIndicatorWhenItIsNull()
    {
        // Arrange
        const string input = "Hello World";

        // Act
        var result = input.Truncate(6, null);

        // Assert
        result.ShouldBe("Hello…");
    }

    [Fact]
    public void Truncate_ShouldNotAddOmissionIndicatorWhenItIsEmpty()
    {
        // Arrange
        const string input = "Hello World";

        // Act
        var result = input.Truncate(5, string.Empty);

        // Assert
        result.ShouldBe("Hello");
    }

    [Theory]
    [InlineData("Hello world", 150, null, "Hello world")]
    [InlineData("Hello world", 6, null, "Hello…")]
    [InlineData("Hello world", 5, "...", "He...")]
    [InlineData("Hello world", 5, "", "Hello")]
    public void Truncate(string input, int maxLength, string? omissionIndicator, string expected)
    {
        // Act
        var result = input.Truncate(maxLength, omissionIndicator);

        // Assert
        result.ShouldBe(expected);
    }
}