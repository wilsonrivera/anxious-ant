namespace AnxiousAnt;

partial class StringExtensionsTests
{
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
}