namespace AnxiousAnt;

public class UrlExtensionsTests
{
    [Fact]
    public void HasSecureSchema_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        Action act = () => UrlExtensions.HasSecureScheme(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData("http://example.com", false)]
    [InlineData("https://example.com", true)]
    [InlineData("ws://example.com", false)]
    [InlineData("wss://example.com", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("sftp://example.com", true)]
    public void HasSecureScheme_ShouldReturnExpectedValue(string input, bool expected)
    {
        // Arrange
        var url = Url.Parse(input);

        // Act
        var result = url.HasSecureScheme();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void HasHttpOrHttpsScheme_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        Action act = () => UrlExtensions.HasHttpOrHttpsScheme(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData("http://example.com")]
    [InlineData("https://example.com")]
    public void HasHttpOrHttpsScheme_ShouldReturnTrueForHttpOrHttpsUrl(string input)
    {
        // Arrange
        var url = Url.Parse(input);

        // Assert
        url.HasHttpOrHttpsScheme().ShouldBeTrue();
    }

    [Fact]
    public void HasHttpOrHttpsScheme_ShouldReturnFalseForNonHttpOrHttpsUrl()
    {
        // Arrange
        var url = Url.Parse("ftp://example.com");

        // Assert
        url.HasHttpOrHttpsScheme().ShouldBeFalse();
    }

    [Fact]
    public void IsDataUrl_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        Action act = () => UrlExtensions.IsDataUrl(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void IsDataUrl_ShouldReturnTrueForDataUrl()
    {
        // Arrange
        var url = Url.Parse("data:text/plain;base64,SGVsbG8gV29ybGQh");

        // Assert
        url.IsDataUrl().ShouldBeTrue();
    }

    [Fact]
    public void IsDataUrl_ShouldReturnFalseForNonDataUrl()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Assert
        url.IsDataUrl().ShouldBeFalse();
    }

    [Fact]
    public void IsFileUrl_ShouldThrowWhenGivenNullUrl()
    {
        // Arrange
        Action act = () => UrlExtensions.IsFileUrl(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void IsFileUrl_ShouldReturnTrueForFileUrl()
    {
        // Arrange
        var url = Url.Parse("file:///etc/hosts");

        // Assert
        url.IsFileUrl().ShouldBeTrue();
    }

    [Fact]
    public void IsFileUrl_ShouldReturnFalseForNonDataUrl()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Assert
        url.IsFileUrl().ShouldBeFalse();
    }
}