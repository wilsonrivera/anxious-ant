using System.Security.Claims;

namespace AnxiousAnt;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void TryFindClaim_ShouldReturnsFalseWhenPrincipalIsNull()
    {
        // Arrange
        ClaimsPrincipal? principal = null;

        // Assert
        principal.TryFindClaim("type", out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void TryFindClaim_ShouldReturnFalseWhenClaimTypeIsNullOrEmpty(string? type)
    {
        // Arrange
        var principal = new ClaimsPrincipal();

        // Assert
        principal.TryFindClaim(type, out _).ShouldBeFalse();
    }

    [Fact]
    public void TryFindClaim_ShouldReturnFalseWhenClaimDoesNotExist()
    {
        // Arrange
        var principal = new ClaimsPrincipal();

        // Assert
        principal.TryFindClaim("type", out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    public void TryFindClaim_ShouldReturnFalseWhenClaimExistsButValueIsEmpty(string value)
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity([
            new Claim("type", value)
        ]));

        // Assert
        principal.TryFindClaim("type", out _).ShouldBeFalse();
    }

    [Fact]
    public void TryFindClaim_ShouldReturnTrueWhenClaimExistsAndValueIsNotEmpty()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity([
            new Claim("type", "value")
        ]));

        // Assert
        principal.TryFindClaim("type", out var value).ShouldBeTrue();
        value.ShouldBe("value");
    }

    [Fact]
    public void TryFindClaim_ShouldReturnFalseWhenValueCannotBeParsed()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity([
            new Claim("type", "value")
        ]));

        // Assert
        principal.TryFindClaim<int>("type", out _).ShouldBeFalse();
    }

    [Fact]
    public void TryFindClaim_ShouldReturnTrueWhenValueCanBeParsed()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity([
            new Claim("type", "42")
        ]));

        // Assert
        principal.TryFindClaim<int>("type", out var value).ShouldBeTrue();
        value.ShouldBe(42);
    }
}