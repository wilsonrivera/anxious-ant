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
    public void GetMaxDecodeLength_ShouldThrowWhenGivenNullInput()
    {
        // Arrange
        Action act = () => Base64.GetMaxDecodeLength(null);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    public void GetMaxDecodeLength_ShouldReturnZeroWhenGivenEmptyOrWhiteSpaceInput(string? input)
    {
        // Act
        var length = Base64.GetMaxDecodeLength(input);

        // Assert
        length.ShouldBe(0);
    }

    [Theory]
    [InlineData("aGVsbG8gd29ybGQ", 12)]
    [InlineData("aGVsbG8gd29ybGQ=", 12)]
    [InlineData("aGVsbG8gd29ybGQsIHRoaXMgaXMgYSBsb25nZXIgdGVzdA", 35)]
    [InlineData("aGVsbG8gd29ybGQsIHRoaXMgaXMgYSBsb25nZXIgdGVzdA==", 36)]
    [InlineData("aGVsbG8gd29ybGQ/sIHR+oaXMgaXMgYSBsb25nZXIgdGVzdA==", 36)]
    [InlineData("aGVsbG8gd29ybGQ_sIHR-oaXMgaXMgYSBsb25nZXIgdGVzdA", 36)]
    public void GetMaxDecodeLength_ShouldReturnCorrectLength(string input, int expectedLength)
    {
        // Act
        var length = Base64.GetMaxDecodeLength(input);

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
    public void ToBase64_ShouldThrowWhenGivenNullString()
    {
        // Arrange
        Action act = () => Base64.ToBase64((string)null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ToBase64_ShouldThrowWhenGivenNullBytes()
    {
        // Arrange
        Action act = () => Base64.ToBase64((byte[])null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ToBase64_ShouldReturnEmptyStringWhenGivenEmptyString()
    {
        // Act
        var result = Base64.ToBase64(string.Empty);

        // Assert
        result.ShouldBeEmpty();
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
    public void ToBase64_String_ShouldReturnExpectedResult()
    {
        // Arrange
        const string input = "hello world";
        var expected = Convert.ToBase64String(EncodingUtils.SecureUtf8Encoding.GetBytes(input));

        // Act
        var result = Base64.ToBase64(input);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ToBase64_Bytes_ShouldReturnExpectedResult()
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
    public void ToUrlSafeBase64_ShouldThrowWhenGivenNullString()
    {
        // Arrange
        Action act = () => Base64.ToUrlSafeBase64((string)null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ToUrlSafeBase64_ShouldThrowWhenGivenNullBytes()
    {
        // Arrange
        Action act = () => Base64.ToUrlSafeBase64((byte[])null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ToUrlSafeBase64_ShouldReturnEmptyStringWhenGivenEmptyString()
    {
        // Act
        var result = Base64.ToUrlSafeBase64(string.Empty);

        // Assert
        result.ShouldBeEmpty();
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
    public void ToUrlSafeBase64_String_ShouldReturnExpectedResult()
    {
        // Arrange
        const string input = "hello world";
        var expected = Convert.ToBase64String(EncodingUtils.SecureUtf8Encoding.GetBytes(input))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        // Act
        var result = Base64.ToUrlSafeBase64(input);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ToUrlSafeBase64_Bytes_ShouldReturnExpectedResult()
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
    [InlineData("       ")]
    public void TryFromString_ShouldReturnFalseWhenGivenNullEmptyOrWhiteSpaceString(string? input)
    {
        // Arrange
        var bytes = new byte[4];

        // Assert
        Base64.TryFromString(input, bytes.AsSpan(), out var bytesWritten).ShouldBeFalse();
        bytes.ShouldBe("\0\0\0\0"u8.ToArray());
        bytesWritten.ShouldBe(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("       ")]
    public void TryFromChars_ShouldReturnFalseWhenGivenNullEmptyOrWhiteSpaceString(string? input)
    {
        // Arrange
        var bytes = new byte[4];

        // Assert
        Base64.TryFromChars(input.AsSpan(), bytes.AsSpan(), out var bytesWritten).ShouldBeFalse();
        bytes.ShouldBe("\0\0\0\0"u8.ToArray());
        bytesWritten.ShouldBe(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("                ")]
    [InlineData("oaf09920!@$9niq9mr1")]
    public void TryFromChars_ShouldReturnFalseWhenGivenInvalidInput(string? input)
    {
        // Assert
        Base64.TryFromChars(input.AsSpan(), [], out _).ShouldBe(false);
    }

    [Fact]
    public void TryFromChars_ShouldAddPaddingWhenInputIsNotUrlSafeAndIsMissingPadding()
    {
        // Arrange
        const string input = "aGVsbG8gd29ybGQhIQ";
        var bytes = new byte[100];

        // Act
        var result = Base64.TryFromChars(input.AsSpan(), bytes.AsSpan(), out var bytesWritten);

        // Assert
        result.ShouldBeTrue();
        bytesWritten.ShouldBe(13);
        bytes[..bytesWritten].ShouldBe("hello world!!"u8.ToArray());
    }

    [Fact]
    public void TryFromChars_ShouldReturnFalseWhenDestinationIsTooSmall()
    {
        // Arrange
        const string input = "aGVsbG8gd29ybGQ=";
        var destination = new byte[1];

        // Act
        var result = Base64.TryFromChars(input.AsSpan(), destination.AsSpan(), out var bytesWritten);

        // Assert
        result.ShouldBeFalse();
        bytesWritten.ShouldBe(0);
        destination.ShouldBe([0]);
    }

    [Theory]
    [InlineData("aGVsbG8gd29ybGQ")]
    [InlineData("aGVsbG8gd29ybGQ=")]
    [InlineData("aGVsbG8gd29ybGQ=================")]
    public void TryFromChars_ShouldReturnTrueForValidInput(string input)
    {
        // Arrange
        var bytes = new byte[12];

        // Act
        var result = Base64.TryFromChars(input.AsSpan(), bytes.AsSpan(), out var bytesWritten);

        // Assert
        result.ShouldBeTrue();
        bytesWritten.ShouldBe(11);
        bytes[..bytesWritten].ShouldBe("hello world"u8.ToArray());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("                ")]
    [InlineData("oaf09920!@$9niq9mr1")]
    public void TryFromChars_ShouldReturnEmptyRentedArrayWhenGivenEmptyOrInvalidInput(string? input)
    {
        // Act
        var result = Base64.TryFromChars(input.AsSpan(), out var bytes);

        // Assert
        result.ShouldBeFalse();
        bytes.Length.ShouldBe(0);
    }

    [Fact]
    public void TryFromChars_ShouldReturnExpectedRentedArray()
    {
        // Arrange
        const string input = "aGVsbG8gd29ybGQ=";

        // Act
        var result = Base64.TryFromChars(input.AsSpan(), out var bytes);

        // Assert
        result.ShouldBeTrue();
        bytes.Length.ShouldBe(11);
        bytes.ToArray().ShouldBe("hello world"u8.ToArray());
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
        var result = Base64.FromString(input);

        // Assert
        result.Length.ShouldBe(expected.Length);
        result.ToArray().ShouldBe(expected);
    }
}