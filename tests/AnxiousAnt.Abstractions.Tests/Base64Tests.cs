using System.Security.Cryptography;

namespace AnxiousAnt;

public class Base64Tests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("03knf09j923-i212_($)!(@%1-")]
    public void IsBase64_ShouldReturnFalseWhenGivenNullWhiteSpaceEmptyOrNonBase64Characters(string? input)
    {
        // Assert
        Base64.IsBase64(input).ShouldBeFalse();
    }

    [Theory]
    [InlineData("aGVsbG8gd29ybGQ=")]
    [InlineData("aGVsbG8gd/29+ybGQ")]
    [InlineData("aGVsbG8gd29ybGQ==========")]
    public void IsBase64_ShouldReturnTrueWhenGivenValidBase64String(string input)
    {
        // Assert
        Base64.IsBase64(input).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("aGVsbG8gd/29+ybGQ")]
    [InlineData("03knf09j923-i212_($)!(@%1-")]
    public void IsUrlSafeBase64_ShouldReturnFalseWhenGivenNullWhiteSpaceEmptyOrNonUrlSafeBase64Characters(string? input)
    {
        // Assert
        Base64.IsUrlSafeBase64(input).ShouldBeFalse();
    }

    [Theory]
    [InlineData("aGVsbG8gd29ybGQ=")]
    [InlineData("aGVsbG8gd29_3-ybGQ")]
    [InlineData("aGVsbG8gd29ybGQ")]
    public void IsUrlSafeBase64_ShouldReturnTrueWhenGivenValidBase64String(string input)
    {
        // Assert
        Base64.IsBase64(input).ShouldBeTrue();
    }

    [Fact]
    public void GetEncodeLength_ShouldThrowWehnGivenInvalidLength()
    {
        // Act
        Action act = () => Base64.GetEncodeLength(-1);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetEncodeLength_ShouldReturnZeroWhenGivenZero()
    {
        // Act
        var length = Base64.GetEncodeLength(0);

        // Assert
        length.ShouldBe(0);
    }

    [Fact]
    public void GetEncodeLength_ShouldReturnExpectedLength()
    {
        // Act
        var length = Base64.GetEncodeLength(12);

        // Assert
        length.ShouldBe(16);
    }

    [Fact]
    public void GetUrlSafeEncodeLength_ShouldThrowWehnGivenInvalidLength()
    {
        // Act
        Action act = () => Base64.GetUrlSafeEncodeLength(-1);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetUrlSafeEncodeLength_ShouldReturnZeroWhenGivenZero()
    {
        // Act
        var length = Base64.GetUrlSafeEncodeLength(0);

        // Assert
        length.ShouldBe(0);
    }

    [Fact]
    public void GetUrlSafeEncodeLength_ShouldReturnExpectedLength()
    {
        // Act
        var length = Base64.GetUrlSafeEncodeLength(12);

        // Assert
        length.ShouldBe(16);
    }

    [Fact]
    public void GetMaxDecodeLength_ShouldThrowWhenGivenInvalidLength()
    {
        // Arrange
        Action act = () => Base64.GetMaxDecodeLength(-1);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("aGVsbG8gd29ybGQ", 9)]
    [InlineData("aGVsbG8gd29ybGQ=", 12)]
    [InlineData("aGVsbG8gd29ybGQsIHRoaXMgaXMgYSBsb25nZXIgdGVzdA", 33)]
    [InlineData("aGVsbG8gd29ybGQsIHRoaXMgaXMgYSBsb25nZXIgdGVzdA==", 36)]
    [InlineData("aGVsbG8gd29ybGQ/sIHR+oaXMgaXMgYSBsb25nZXIgdGVzdA==", 36)]
    public void GetMaxDecodeLength_ShouldReturnCorrectLength(string input, int expectedLength)
    {
        // Act
        var length = Base64.GetMaxDecodeLength(input.Length);

        // Assert
        length.ShouldBe(expectedLength);
    }

    [Fact]
    public void GetMaxUrlSafeDecodeLength_ShouldThrowWhenGivenInvalidLength()
    {
        // Arrange
        Action act = () => Base64.GetMaxDecodeLength(-1);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("aGVsbG8gd29ybGQ", 12)]
    [InlineData("aGVsbG8gd29ybGQsIHRoaXMgaXMgYSBsb25nZXIgdGVzdA", 35)]
    [InlineData("aGVsbG8gd29ybGQ_sIHR-oaXMgaXMgYSBsb25nZXIgdGVzdA", 36)]
    public void GetMaxUrlSafeDecodeLength_ShouldReturnCorrectLength(string input, int expectedLength)
    {
        // Act
        var length = Base64.GetMaxUrlSafeDecodeLength(input.Length);

        // Assert
        length.ShouldBe(expectedLength);
    }

    [Fact]
    public void TryToBase64_ShouldReturnFalseWhenGivenNullBytes()
    {
        // Arrange
        var destination = new char[5];

        // Act
        var result = Base64.TryToBase64(null!, destination.AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeFalse();
        destination.ShouldBe(new char[5]);
        charsWritten.ShouldBe(0);
    }

    [Fact]
    public void TryToBase64_ShouldReturnTrueWhenGivenEmptyBytes()
    {
        // Act
        var result = Base64.TryToBase64(Array.Empty<byte>(), new char[5].AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeTrue();
        charsWritten.ShouldBe(0);
    }

    [Fact]
    public void TryToBase64_ShouldReturnFalseWhenNotEnoughSpaceInDestination()
    {
        // Arrange
        var bytes = RandomNumberGenerator.GetBytes(33);
        var destination = new char[5];

        // Act
        var result = Base64.TryToBase64(bytes, destination.AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeFalse();
        destination.ShouldBe(new char[5]);
        charsWritten.ShouldBe(0);
    }

    [Fact]
    public void TryToBase64_ShouldReturnTrueWhenGivenEnoughSpace()
    {
        // Arrange
        var bytes = RandomNumberGenerator.GetBytes(33);
        var destination = new char[50];

        var expected = new char[50];
        var expectedCharsWritten = Convert.ToBase64CharArray(bytes, 0, 33, expected, 0);

        // Act
        var result = Base64.TryToBase64(bytes, destination.AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeTrue();
        charsWritten.ShouldBe(expectedCharsWritten);
        destination[..charsWritten].ShouldBe(expected[..expectedCharsWritten]);
    }

    [Fact]
    public void ToBase64_ShouldThrowWhenGivenNullBytes()
    {
        // Arrange
        Action act = () => Base64.ToBase64(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ToBase64_ShouldReturnEmptyStringWhenGivenEmptyBytes()
    {
        // Act
        var result = Base64.ToBase64(Array.Empty<byte>());

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToBase64_ShouldReturnExpectedResult()
    {
        // Arrange
        var bytes = RandomNumberGenerator.GetBytes(Random.Shared.Next(0, 33));
        var expected = Convert.ToBase64String(bytes);

        // Act
        var result = Base64.ToBase64(bytes);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void TryToUrlSafeBase64_ShouldReturnFalseWhenGivenNullBytes()
    {
        // Arrange
        var destination = new char[5];

        // Act
        var result = Base64.TryToUrlSafeBase64(null!, destination.AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeFalse();
        destination.ShouldBe(new char[5]);
        charsWritten.ShouldBe(0);
    }

    [Fact]
    public void TryToUrlSafeBase64_ShouldReturnTrueWhenGivenEmptyBytes()
    {
        // Act
        var result = Base64.TryToUrlSafeBase64(Array.Empty<byte>(), new char[5].AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeTrue();
        charsWritten.ShouldBe(0);
    }

    [Fact]
    public void TryToUrlSafeBase64_ShouldReturnFalseWhenNotEnoughSpaceInDestination()
    {
        // Arrange
        var bytes = RandomNumberGenerator.GetBytes(33);
        var destination = new char[5];

        // Act
        var result = Base64.TryToUrlSafeBase64(bytes, destination.AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeFalse();
        destination.ShouldBe(new char[5]);
        charsWritten.ShouldBe(0);
    }

    [Fact]
    public void TryToUrlSafeBase64_ShouldReturnTrueWhenGivenEnoughSpace()
    {
        // Arrange
        byte[] bytes = [9, 214, 23, 122, 34, 1, 51, 42, 32, 96, 34, 87, 55, 255];
        var destination = new char[50];

        // Act
        var result = Base64.TryToUrlSafeBase64(bytes, destination.AsSpan(), out var charsWritten);

        // Assert
        result.ShouldBeTrue();
        charsWritten.ShouldBe(19);
        destination[..charsWritten].ShouldBe([
            'C', 'd', 'Y', 'X', 'e', 'i', 'I', 'B', 'M', 'y', 'o', 'g', 'Y', 'C', 'J', 'X', 'N', '_', '8'
        ]);
    }

    [Fact]
    public void ToUrlSafeBase64_ShouldThrowWhenGivenNullBytes()
    {
        // Arrange
        Action act = () => Base64.ToUrlSafeBase64(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ToUrlSafeBase64_ShouldReturnEmptyStringWhenGivenEmptyBytes()
    {
        // Act
        var result = Base64.ToUrlSafeBase64(Array.Empty<byte>());

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToUrlSafeBase64_ShouldReturnExpectedResult()
    {
        // Arrange
        var bytes = RandomNumberGenerator.GetBytes(Random.Shared.Next(0, 33));
        var expected = Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');

        // Act
        var result = Base64.ToUrlSafeBase64(bytes);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("                ")]
    public void TryFromString_ShouldReturnFalseWhenGivenNullEmptyOrWhiteSpaceString(string? input)
    {
        // Assert
        Base64.TryFromString(input, out _).ShouldBe(false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("                ")]
    [InlineData("oaf09920!@$9niq9mr1")]
    public void TryFromChars_ShouldReturnFalseWhenGivenInvalidInput(string? input)
    {
        // Assert
        Base64.TryFromChars(input.AsSpan(), out _).ShouldBe(false);
    }

    [Theory]
    [InlineData("aGVsbG8gd29ybGQ")]
    [InlineData("aGVsbG8gd29ybGQ===========")]
    [InlineData("aGVsbG8gd29_3-ybGQ")]
    public void TryFromChars_ShouldReturnTrueWhenGivenValidBase64(string input)
    {
        // Arrange
        var normalized = input.Replace('-', '+').Replace('_', '/').TrimEnd('=');
        if (normalized.Length % 4 != 0)
        {
            var neededPadding = 4 - (normalized.Length % 4);
            normalized += new string('=', neededPadding);
        }

        var expected = Convert.FromBase64String(GetWithPadding(input));

        // Act
        var result = Base64.TryFromChars(input.AsSpan(), out var bytes);

        // Assert
        result.ShouldBeTrue();
        bytes.Length.ShouldBe(expected.Length);
        bytes.ToArray().ShouldBe(expected);
    }

    private static string GetWithPadding(string input)
    {
        var normalized = input.Replace('-', '+').Replace('_', '/').TrimEnd('=');
        if (normalized.Length % 4 != 0)
        {
            var neededPadding = 4 - (normalized.Length % 4);
            normalized += new string('=', neededPadding);
        }

        return normalized;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("         ")]
    public void FromString_ShouldThrowWhenGivenNullEmptyOrWhiteSpaceString(string? input)
    {
        // Arrange
        Action act = () => Base64.FromString(input);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void FromString_ShouldThrowWhenGivenInvalidInput()
    {
        // Arrange
        Action act = () => Base64.FromString("oaf09920!@$9niq9mr1");

        // Assert
        act.ShouldThrow<FormatException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("         ")]
    public void FromChars_ShouldThrowWhenGivenEmptyOrWhiteSpaceString(string input)
    {
        // Arrange
        Action act = () => Base64.FromChars(input.AsSpan());

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void FromChars_ShouldThrowWhenGivenInvalidInput()
    {
        // Arrange
        Action act = () => Base64.FromChars("oaf09920!@$9niq9mr1");

        // Assert
        act.ShouldThrow<FormatException>();
    }

    [Fact]
    public void FromChars_ShouldReturnExpectedResult()
    {
        // Arrange
        const string input = "aGVsbG8gd29ybGQ=";
        var expected = Convert.FromBase64String(input);

        // Act
        using var result = Base64.FromString(input);

        // Assert
        result.Length.ShouldBe(expected.Length);
        result.ToArray().ShouldBe(expected);
    }
}