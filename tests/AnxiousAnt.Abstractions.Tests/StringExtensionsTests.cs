namespace AnxiousAnt;

public class StringExtensionsTests
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
    [InlineData(null, "test")]
    [InlineData("test", null)]
    public void OrdinalEqual_ShouldReturnFalse_WhenOneStringIsNull(string? s1, string? s2)
    {
        // Act
        var result = s1.OrdinalEquals(s2);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void OrdinalEqual_ShouldReturnTrue_WhenStringsAreEqual_ConsideringCase()
    {
        // Arrange
        const string s1 = "Test";
        const string s2 = "Test";

        // Act
        var result = s1.OrdinalEquals(s2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void OrdinalEqual_ShouldReturnTrue_WhenStringsAreEqual_IgnoringCase()
    {
        // Arrange
        const string s1 = "Test";
        const string s2 = "test";

        // Act
        var result = s1.OrdinalEquals(s2, true);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void OrdinalEqual_ShouldReturnFalse_WhenStringsAreDifferent()
    {
        // Arrange
        const string s1 = "Test";
        const string s2 = "test";

        // Act
        var result = s1.OrdinalEquals(s2);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void OrdinalContains_ShouldReturnFalse_WhenStringIsNull()
    {
        // Arrange
        const string? s = null;

        // Act
        var result = s.OrdinalContains('e');

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null, "test")]
    [InlineData("test", null)]
    public void OrdinalContains_ShouldReturnFalse_WhenOneStringIsNull(string? s1, string? s2)
    {
        // Act
        var result = s1.OrdinalContains(s2);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData('t', false, true)]
    [InlineData('T', false, false)]
    [InlineData('t', true, true)]
    [InlineData('T', true, true)]
    [InlineData('a', true, false)]
    public void OrdinalContains_Char(char c, bool ignoreCase, bool expected)
    {
        // Arrange
        const string s = "testing";

        // Act
        var result = s.OrdinalContains(c, ignoreCase);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("test", false, true)]
    [InlineData("Test", false, false)]
    [InlineData("test", true, true)]
    [InlineData("TEST", true, true)]
    [InlineData("samp", true, false)]
    public void OrdinalContains_String(string s2, bool ignoreCase, bool expected)
    {
        // Arrange
        const string s = "testing";

        // Act
        var result = s.OrdinalContains(s2, ignoreCase);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, "test")]
    [InlineData("test", null)]
    public void OrdinalStartsWith_ShouldReturnFalse_WhenOneStringIsNull(string? s1, string? s2)
    {
        // Act
        var result = s1.OrdinalStartsWith(s2);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("test", false, true)]
    [InlineData("Test", false, false)]
    [InlineData("test", true, true)]
    [InlineData("TEST", true, true)]
    [InlineData("samp", true, false)]
    public void OrdinalStartsWith(string prefix, bool ignoreCase, bool expected)
    {
        // Arrange
        const string s = "testing";

        // Act
        var result = s.OrdinalStartsWith(prefix, ignoreCase);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, "test")]
    [InlineData("test", null)]
    public void OrdinalEndsWith_ShouldReturnFalse_WhenOneStringIsNull(string? s1, string? s2)
    {
        // Act
        var result = s1.OrdinalEndsWith(s2);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("ing", false, true)]
    [InlineData("inG", false, false)]
    [InlineData("ing", true, true)]
    [InlineData("ING", true, true)]
    [InlineData("ple", true, false)]
    public void OrdinalEndsWith(string suffix, bool ignoreCase, bool expected)
    {
        // Arrange
        const string s = "testing";

        // Act
        var result = s.OrdinalEndsWith(suffix, ignoreCase);

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