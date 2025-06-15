using CommunityToolkit.HighPerformance.Buffers;

namespace AnxiousAnt.Security;

public class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldThrowWhenGivenNullString()
    {
        // Arrange
        Action act = () => Pbkdf2PasswordHasher.Hash((string)null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Hash_ShouldThrowWhenGivenNullBytes()
    {
        // Arrange
        Action act = () => Pbkdf2PasswordHasher.Hash((byte[])null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Hash_ShouldDeriveBytesForPassword()
    {
        // Act
        var password = Pbkdf2PasswordHasher.Hash("password1");

        // Assert
        password.ShouldNotBeEmpty();
        Base64.IsBase64(password).ShouldBeTrue();
    }

    [Fact]
    public void Verify_ShouldThrowWhenGivenNullHashedPassword()
    {
        // Arrange
        Action act = () => Pbkdf2PasswordHasher.Verify(null!, "password1");

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Verify_ShouldThrowWhenGivenNullPassword()
    {
        // Arrange
        Action act = () => Pbkdf2PasswordHasher.Verify("password1", null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    public void Verify_ShouldReturnFalseWhenGivenEmptyOrWhiteSpaceHash(string hashedPassword)
    {
        // Act
        var result = Pbkdf2PasswordHasher.Verify(hashedPassword, "password1");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Verify_ShouldReturnFalseWhenGivenInvalidHashedPassword()
    {
        // Act
        var result = Pbkdf2PasswordHasher.Verify("password1", "password1");

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData(
        "AQAAAAIAD0JAAAAAEKZBSLYVPRFBWtdFCyk+OZcmrlk6zsK6TjcuDMuU+CUi6F6sKpI8FqVuy4o6eHqEP1vnhe0hLfMdHK2T+McMqKE",
        "password1"
    )]
    [InlineData(
        "AQAAAAIAD0JAAAAAEKZBSLYVPRFBWtdFCyk+OZcmrlk6zsK6TjcuDMuU+CUi6F6sKpI8FqVuy4o6eHqEP1vnhe0hLfMdHK2T+McMqKE=",
        "password1"
    )]
    [InlineData(
        "AQAAAAIAD0JAAAAAEKZBSLYVPRFBWtdFCyk+OZcmrlk6zsK6TjcuDMuU+CUi6F6sKpI8FqVuy4o6eHqEP1vnhe0hLfMdHK2T+McMqKE======",
        "password1"
    )]
    public void Verify_ShouldReturnTrueWhenGivenPasswordMatchesGivenHash(string hashedPassword, string password)
    {
        // Act
        var result = Pbkdf2PasswordHasher.Verify(hashedPassword, password);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void VerifyPasswordBytes_ShouldReturnFalseWhenHeaderCannotBeRead()
    {
        // Act
        var result = Pbkdf2PasswordHasher.VerifyPasswordBytes(
            "\0\0\0\0\0\0\0\0\0\0\0\0\0"u8,
            "password",
            Pbkdf2PasswordHasher.V1FormatMarker,
            out _,
            out _
        );

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void VerifyPasswordBytes_ShouldReturnFalseWhenHashHasInvalidPrf()
    {
        // Arrange
        using var owner = SpanOwner<byte>.Allocate(77);
        owner.Span[0] = Pbkdf2PasswordHasher.V1FormatMarker;
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 1, 40);
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 5, 400_000);
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 9, 16);

        // Act
        var result = Pbkdf2PasswordHasher.VerifyPasswordBytes(
            owner.Span,
            "password1",
            Pbkdf2PasswordHasher.V1FormatMarker,
            out _,
            out _
        );

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void VerifyPasswordBytes_ShouldReturnFalseWhenHashIterationsCountIsInvalid()
    {
        // Arrange
        using var owner = SpanOwner<byte>.Allocate(77);
        owner.Span[0] = Pbkdf2PasswordHasher.V1FormatMarker;
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 1, 1);
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 5, 0);
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 9, 16);

        // Act
        var result = Pbkdf2PasswordHasher.VerifyPasswordBytes(
            owner.Span,
            "password1",
            Pbkdf2PasswordHasher.V1FormatMarker,
            out _,
            out _
        );

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void VerifyPasswordBytes_ShouldReturnFalseWhenInvalidHash()
    {
        // Arrange
        using var owner = SpanOwner<byte>.Allocate(13 + 17);
        owner.Span[0] = Pbkdf2PasswordHasher.V1FormatMarker;
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 1, 2);
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 5, 500_000);
        Pbkdf2PasswordHasher.WriteNetworkByteOrder(owner.Span, 9, 16);

        // Act
        var result = Pbkdf2PasswordHasher.VerifyPasswordBytes(
            owner.Span,
            "password1",
            Pbkdf2PasswordHasher.V1FormatMarker,
            out var prf,
            out var iterationsCount
        );

        // Assert
        result.ShouldBeFalse();
        prf.ShouldBe(Pbkdf2PasswordHasher.KeyDerivationPrf.HmacSha512);
        iterationsCount.ShouldBe(500_000);
    }

    [Fact]
    public void VerifyPasswordBytes_ShouldReturnExpectedValues()
    {
        // Arrange
        const string hashedPassword =
            "AQAAAAIAD0JAAAAAEKZBSLYVPRFBWtdFCyk+OZcmrlk6zsK6TjcuDMuU+CUi6F6sKpI8FqVuy4o6eHqEP1vnhe0hLfMdHK2T+McMqKE";

        // Act
        var result = Pbkdf2PasswordHasher.VerifyPasswordBytes(
            Base64.FromString(hashedPassword),
            "password1",
            Pbkdf2PasswordHasher.V1FormatMarker,
            out var prf,
            out var iterationsCount
        );

        // Assert
        result.ShouldBeTrue();
        prf.ShouldBe(Pbkdf2PasswordHasher.KeyDerivationPrf.HmacSha512);
        iterationsCount.ShouldBe(1_000_000);
    }
}