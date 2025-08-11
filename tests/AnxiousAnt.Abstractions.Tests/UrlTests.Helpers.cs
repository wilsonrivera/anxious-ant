namespace AnxiousAnt;

partial class UrlTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("       ")]
    public void IsValidHostName_ShouldReturnFalseWhenGivenNullEmptyOrWhitespace(string? input)
    {
        // Assert
        Url.IsValidHostName(input).ShouldBeFalse();
    }

    [Fact]
    public void IsValidHostName_ShouldReturnTrueWhenGivenValidHostName()
    {
        // Assert
        Url.IsValidHostName("example.com").ShouldBeTrue();
    }

    [Theory]
    [InlineData("a234567890123456789012345678901234567890123456789012345678901232.com")]
    [InlineData(
        "this.is.a.really.long.domain.name.that.exceeds.the.total.length.limit..............................................................example.com"
    )]
    [InlineData("exa_mple.com")]
    [InlineData("ex!ample.com")]
    [InlineData("ex@ample.com")]
    [InlineData("exa#mple.com")]
    [InlineData("exa$mple.com")]
    [InlineData("exa%20mple.com")]
    [InlineData("example .com")]
    [InlineData("-abc.com")]
    [InlineData("abc-.com")]
    [InlineData("example.-com")]
    [InlineData("ex--ample.com")]
    [InlineData("\u200Eexample.com")]
    public void IsValidHostName_ShouldReturnFalseWhenGivenInvalidHostName(string input)
    {
        // Assert
        Url.IsValidHostName(input).ShouldBeFalse();
    }
}