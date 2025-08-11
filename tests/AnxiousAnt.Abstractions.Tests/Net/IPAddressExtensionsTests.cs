using System.Net;

namespace AnxiousAnt.Net;

// ReSharper disable once InconsistentNaming
public class IPAddressExtensionsTests
{
    [Fact]
    public void ToExtendedString_ShouldThrowWhenGivenNullIp()
    {
        // Arrange
        Action act = () => IPAddressExtensions.ToExtendedString(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData("127.0.0.1", "127.0.0.1")]
    [InlineData("::1", "0000:0000:0000:0000:0000:0000:0000:0001")]
    [InlineData("2001:db8::1", "2001:0db8:0000:0000:0000:0000:0000:0001")]
    public void ToExtendedString_ShouldReturnExtendedString(string input, string expected)
    {
        // Arrange
        var ip = IPAddress.Parse(input);

        // Act
        var result = ip.ToExtendedString();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Fold_ShouldThrowWhenGivenNullIp()
    {
        // Arrange
        Action act = () => IPAddressExtensions.Fold(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData("127.0.0.1", 543007363077980397uL)]
    [InlineData("::1", 3031955301628188530uL)]
    [InlineData("::11", 3031955301896630434uL)]
    [InlineData("2001:db8::1", 17615211542034973940uL)]
    public void Fold_ShouldReturnExpectedValue(string input, ulong expected)
    {
        // Arrange
        var ip = IPAddress.Parse(input);

        // Act
        var hash = ip.Fold();

        // Assert
        hash.ShouldBe(expected);
    }
}