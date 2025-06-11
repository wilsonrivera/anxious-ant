using System.Diagnostics;

using AnxiousAnt.Collections;

namespace AnxiousAnt;

/// <summary>
/// Utility class providing methods for Base64 encoding and decoding operations.
/// </summary>
public static class Base64
{
    private static readonly SearchValues<char> AllowedBase64Characters = SearchValues.Create(
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
        'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
        'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        '/', '+', '=',
        '_', '-' // URL Safe
    );

    private static readonly SearchValues<char> AllowedUrlSafeBase64Characters = SearchValues.Create(
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
        'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
        'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        '_', '-' // URL Safe
    );

    /// <summary>
    /// Determines whether the specified string is a valid Base64-encoded string.
    /// </summary>
    /// <param name="value">The read-only span of characters to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the specified string is a valid Base64-encoded string; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBase64(ReadOnlySpan<char> value) =>
        !value.IsWhiteSpace() && !value.ContainsAnyExcept(AllowedBase64Characters);

    /// <summary>
    /// Determines whether the specified string is a valid URL-safe Base64-encoded string.
    /// </summary>
    /// <param name="value">The read-only span of characters to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the specified string is a valid URL-safe Base64-encoded string; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsUrlSafeBase64(ReadOnlySpan<char> value) =>
        !value.IsWhiteSpace() && !value.ContainsAnyExcept(AllowedUrlSafeBase64Characters);

    /// <summary>
    /// Calculates the length of the base64-encoded string representation for a given input length.
    /// </summary>
    /// <param name="length">The length of the input data in bytes.</param>
    /// <returns>
    /// The length of the base64-encoded string for the specified input length.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetEncodeLength(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        return ThirdParty.LitJWT.Base64.GetBase64EncodeLength(length);
    }

    /// <summary>
    /// Calculates the length of a URL-safe Base64-encoded string based on the specified input length.
    /// </summary>
    /// <param name="length">The length of the input data to be encoded.</param>
    /// <returns>
    /// The length of the resulting URL-safe Base64-encoded string.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetUrlSafeEncodeLength(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        return ThirdParty.LitJWT.Base64.GetBase64UrlEncodeLength(length);
    }

    /// <summary>
    /// Calculates the maximum length of the byte array that can be obtained by decoding a base64
    /// string of the specified length.
    /// </summary>
    /// <param name="length">The length of the base64 string.</param>
    /// <returns>
    /// The maximum length of the byte array that can be obtained by decoding the input base64 string.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxDecodeLength(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        return ThirdParty.LitJWT.Base64.GetMaxBase64DecodeLength(length);
    }

    /// <summary>
    /// Calculates the maximum number of bytes required to decode a URL-safe Base64-encoded string of
    /// the specified length.
    /// </summary>
    /// <param name="length">The length of the URL-safe Base64-encoded string.</param>
    /// <returns>
    /// The maximum number of bytes required to decode the URL-safe Base64-encoded string.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxUrlSafeDecodeLength(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        return ThirdParty.LitJWT.Base64.GetMaxBase64UrlDecodeLength(length);
    }

    /// <summary>
    /// Attempts to encode the specified byte array to a Base64 string representation and writes the result
    /// to the provided destination buffer.
    /// </summary>
    /// <param name="bytes">The byte array to encode. If null, the operation returns <c>false</c>.</param>
    /// <param name="destination">The span of characters where the encoded Base64 string is written.</param>
    /// <param name="charsWritten">The number of characters written to the destination span.</param>
    /// <returns>
    /// <c>true</c> if the encoding operation succeeds and the encoded string fits in the destination buffer;
    /// otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool TryToBase64(byte[]? bytes, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;
        return bytes is not null && TryToBase64(bytes.AsSpan(), destination, out charsWritten);
    }

    /// <summary>
    /// Attempts to convert the specified read-only span of bytes to a Base64-encoded string and writes the result
    /// into the provided span of characters.
    /// </summary>
    /// <param name="bytes">The read-only span of bytes to encode into Base64.</param>
    /// <param name="destination">The span of characters to hold the encoded Base64 string.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters written
    /// to the destination span.</param>
    /// <returns>
    /// <c>true</c> if the conversion was successful and the result fits within the destination span;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool TryToBase64(in ReadOnlySpan<byte> bytes, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;
        if (bytes.IsEmpty)
        {
            return true;
        }

        var expectedMinLength = GetEncodeLength(bytes.Length);
        if (destination.Length < expectedMinLength)
        {
            return false;
        }

        return bytes.IsEmpty || ThirdParty.LitJWT.Base64.TryToBase64Chars(bytes, destination, out charsWritten);
    }

    /// <summary>
    /// Converts the specified byte array to a Base64-encoded string.
    /// </summary>
    /// <param name="bytes">The byte array to be encoded.</param>
    /// <returns>
    /// A Base64-encoded string representation of the specified byte array.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static string ToBase64(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        return ToBase64(bytes.AsSpan());
    }

    /// <summary>
    /// Converts the specified read-only span of bytes to a Base64-encoded string.
    /// </summary>
    /// <param name="bytes">The read-only span of bytes to be encoded.</param>
    /// <returns>
    /// A Base64-encoded string representation of the specified byte span.
    /// </returns>
    public static string ToBase64(in ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty)
        {
            return string.Empty;
        }

        var bufferLength = GetEncodeLength(bytes.Length);
        using var charOwner = SpanOwner<char>.Allocate(bufferLength);
        if (!TryToBase64(bytes, charOwner.Span, out var charsWritten))
        {
            // We should always have a buffer big enough to encode the given bytes
            throw new UnreachableException();
        }

        return new string(charOwner.Span[..charsWritten]);
    }

    /// <summary>
    /// Attempts to encode the specified byte array into a URL-safe Base64-encoded string and writes it to
    /// the destination buffer.
    /// </summary>
    /// <param name="bytes">The byte array to encode into a URL-safe Base64 format.</param>
    /// <param name="destination">The span of characters to receive the encoded URL-safe Base64 string.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters written to the
    /// destination buffer.</param>
    /// <returns>
    /// <c>true</c> if the encoding operation was successful; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool TryToUrlSafeBase64(byte[]? bytes, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;
        return bytes is not null && TryToUrlSafeBase64(bytes.AsSpan(), destination, out charsWritten);
    }

    /// <summary>
    /// Attempts to encode the specified byte span as a URL-safe Base64 string into the destination span.
    /// </summary>
    /// <param name="bytes">The span of bytes to encode.</param>
    /// <param name="destination">The span of characters to write the encoded URL-safe Base64 string to.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters written to the
    /// destination span.</param>
    /// <returns>
    /// <c>true</c> if the encoding operation was successful; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryToUrlSafeBase64(in ReadOnlySpan<byte> bytes, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;
        if (bytes.IsEmpty)
        {
            return true;
        }

        var expectedMinLength = GetUrlSafeEncodeLength(bytes.Length);
        if (destination.Length < expectedMinLength)
        {
            return false;
        }

        return ThirdParty.LitJWT.Base64.TryToBase64UrlChars(bytes, destination, out charsWritten);
    }

    /// <summary>
    /// Converts an array of bytes to a URL-safe Base64-encoded string.
    /// </summary>
    /// <param name="bytes">The array of bytes to be encoded.</param>
    /// <returns>
    /// A URL-safe Base64-encoded string representation of the input bytes.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static string ToUrlSafeBase64(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        return ToUrlSafeBase64(bytes.AsSpan());
    }

    /// <summary>
    /// Converts a span of bytes to a URL-safe Base64-encoded string.
    /// </summary>
    /// <param name="bytes">The span of bytes to be encoded.</param>
    /// <returns>
    /// A URL-safe Base64-encoded string representation of the input bytes.
    /// </returns>
    public static string ToUrlSafeBase64(in ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty)
        {
            return string.Empty;
        }

        var bufferLength = GetUrlSafeEncodeLength(bytes.Length);
        using var charOwner = SpanOwner<char>.Allocate(bufferLength);
        if (!TryToUrlSafeBase64(bytes, charOwner.Span, out var charsWritten))
        {
            // We should always have a buffer big enough to encode the given bytes
            throw new UnreachableException();
        }

        return new string(charOwner.Span[..charsWritten]);
    }

    /// <summary>
    /// Attempts to decode a Base64-encoded or URL-safe Base64-encoded string into a binary array.
    /// </summary>
    /// <param name="str">The Base64-encoded or URL-safe Base64-encoded string to decode.</param>
    /// <param name="result">
    /// When this method returns, contains the decoded byte array if the operation succeeded;
    /// otherwise, contains the default value of <see cref="RentedArray{T}"/>. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <c>true</c> if the string was successfully decoded into a byte array; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool TryFromString(string? str, out RentedArray<byte> result)
    {
        result = default;
        return !string.IsNullOrWhiteSpace(str) && TryFromChars(str.AsSpan(), out result);
    }

    /// <summary>
    /// Attempts to decode a Base64-encoded or URL-safe Base64-encoded string into a binary array.
    /// </summary>
    /// <param name="s">The read-only span of Base64-encoded or URL-safe Base64 characters to decode.</param>
    /// <param name="result">
    /// When this method returns, contains a <see cref="RentedArray{T}"/> containing the decoded binary data if the
    /// decoding is successful, or an uninitialized <see cref="RentedArray{T}"/> if the decoding fails.
    /// </param>
    /// <returns>
    /// <c>true</c> if the string is successfully decoded; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryFromChars(in ReadOnlySpan<char> s, out RentedArray<byte> result)
    {
        result = default;
        if (s.IsEmpty)
        {
            return false;
        }

        // Ensure that the input is valid Base64 or URL-safe Base64
        var isUrlSafe = IsUrlSafeBase64(s);
        switch (isUrlSafe)
        {
            case false when !IsBase64(s):
                return false;
            case false when s.Length % 4 != 0:
                // Ensure that the input is padded correctly when it's not URL-safe
                {
                    var charsWithoutPadding = s.TrimEnd('=');
                    var neededPadding = 4 - (charsWithoutPadding.Length % 4);
                    if (neededPadding > 0)
                    {
                        // Add padding to the end of the string
                        var charsLength = charsWithoutPadding.Length + neededPadding;
                        using var charsOwner = SpanOwner<char>.Allocate(charsLength);
                        charsWithoutPadding.CopyTo(charsOwner.Span);
                        charsOwner.Span[^neededPadding..].Fill('=');

                        return TryFromChars(charsOwner.Span[..charsLength], out result);
                    }

                    break;
                }
        }

        // Calculate the maximum length of the decoded bytes
        var length = s.Length + 4; // Add extra space in case padding is missing
        var decodeLength = isUrlSafe ? GetMaxUrlSafeDecodeLength(length) : GetMaxDecodeLength(length);

        // Try to decode the input
        using var owner = SpanOwner<byte>.Allocate(decodeLength);
        if (!(
                (!isUrlSafe && ThirdParty.LitJWT.Base64.TryFromBase64Chars(s, owner.Span, out var bytesWritten)) ||
                (isUrlSafe && ThirdParty.LitJWT.Base64.TryFromBase64UrlChars(s, owner.Span, out bytesWritten))
            ))
        {
            // The input is not valid Base64 or URL-safe Base64 string
            result = default;
            return false;
        }

        result = RentedArray<byte>.FromSpan(owner.Span[..bytesWritten]);
        return true;
    }

    /// <summary>
    /// Decodes a Base64-encoded string into a rented array of bytes.
    /// </summary>
    /// <param name="s">The Base64-encoded or URL-safe Base64 string to decode.</param>
    /// <returns>
    /// A <see cref="RentedArray{T}"/> containing the decoded bytes.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static RentedArray<byte> FromString(string? s)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s);
        return FromChars(s.AsSpan());
    }

    /// <summary>
    /// Decodes a Base64-encoded read-only span of characters into a rented array of bytes.
    /// </summary>
    /// <param name="s">The read-only span of Base64-encoded or URL-safe Base64 characters to decode.</param>
    /// <returns>
    /// A <see cref="RentedArray{T}"/> containing the decoded bytes.
    /// </returns>
    public static RentedArray<byte> FromChars(in ReadOnlySpan<char> s)
    {
        if (s.IsWhiteSpace())
        {
            throw new ArgumentException(
                "The value cannot be an empty string or composed entirely of whitespace.",
                nameof(s)
            );
        }

        if (!TryFromChars(s, out var result))
        {
            throw new FormatException(
                "The input is not a valid Base-64 string as it contains a non-base 64 character, more than two " +
                "padding characters, or an illegal character among the padding characters."
            );
        }

        return result;
    }
}