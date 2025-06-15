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
    /// Calculates the maximum possible length of decoded data from a Base64-encoded string.
    /// </summary>
    /// <param name="s">The Base64-encoded input string to evaluate. Must not be <c>null</c>.</param>
    /// <returns>
    /// The maximum length of the decoded data in bytes.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the input string <paramref name="s"/> is <c>null</c>.</exception>
    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxDecodeLength(string? s)
    {
        ArgumentNullException.ThrowIfNull(s);
        return GetMaxDecodeLength(s.AsSpan());
    }

    /// <summary>
    /// Computes the maximum possible length of decoded bytes for a given Base64 or URL-safe Base64 encoded input.
    /// </summary>
    /// <param name="s">The read-only span of characters representing the encoded input to evaluate.</param>
    /// <returns>
    /// The maximum number of bytes that the decoded output might contain.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaxDecodeLength(in ReadOnlySpan<char> s)
    {
        if (s.IsWhiteSpace())
        {
            return 0;
        }

        var isUrlSafe = IsUrlSafeBase64(s);
        if (isUrlSafe)
        {
            return ThirdParty.LitJWT.Base64.GetMaxBase64UrlDecodeLength(s.Length);
        }

        var charsWithoutPadding = s.TrimEnd('=');
        var neededPadding = 4 - (charsWithoutPadding.Length % 4);

        return ThirdParty.LitJWT.Base64.GetMaxBase64DecodeLength(charsWithoutPadding.Length + neededPadding);
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
    /// Converts the specified string into its Base64-encoded string representation.
    /// </summary>
    /// <param name="str">The string to convert into a Base64-encoded string.</param>
    /// <returns>
    /// A Base64-encoded representation of the provided string. Returns an empty string if the input is null or empty.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static string ToBase64(string? str)
    {
        ArgumentNullException.ThrowIfNull(str);
        return ToBase64(str.AsSpan());
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
    /// Encodes the specified read-only span of characters to a Base64-encoded string.
    /// </summary>
    /// <param name="s">The read-only span of characters to encode.</param>
    /// <returns>
    /// The Base64-encoded string representation of the input characters.
    /// </returns>
    public static string ToBase64(in ReadOnlySpan<char> s)
    {
        if (s.IsEmpty)
        {
            return string.Empty;
        }

        // Convert the given string to its byte representation
        var inputByteCount = EncodingUtils.SecureUtf8Encoding.GetByteCount(s);
        using var bytesOwner = SpanOwner<byte>.Allocate(inputByteCount, AllocationMode.Clear);
        var bytesWritten = EncodingUtils.SecureUtf8Encoding.GetBytes(s, bytesOwner.Span);

        // Convert the byte representation to Base64
        return ToBase64(bytesOwner.Span[..bytesWritten]);
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
    /// Converts a string to a URL-safe Base64-encoded string.
    /// </summary>
    /// <param name="str">The input string to be encoded.</param>
    /// <returns>
    /// A URL-safe Base64-encoded string representation of the input string.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static string ToUrlSafeBase64(string? str)
    {
        ArgumentNullException.ThrowIfNull(str);
        return ToUrlSafeBase64(str.AsSpan());
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
    /// Converts the specified read-only span of characters to a URL-safe Base64-encoded string.
    /// </summary>
    /// <param name="s">The read-only span of characters to encode.</param>
    /// <returns>
    /// A URL-safe Base64-encoded string representing the provided input.
    /// </returns>
    public static string ToUrlSafeBase64(in ReadOnlySpan<char> s)
    {
        if (s.IsEmpty)
        {
            return string.Empty;
        }

        // Convert the given string to its byte representation
        var inputByteCount = EncodingUtils.SecureUtf8Encoding.GetByteCount(s);
        using var bytesOwner = SpanOwner<byte>.Allocate(inputByteCount, AllocationMode.Clear);
        var bytesWritten = EncodingUtils.SecureUtf8Encoding.GetBytes(s, bytesOwner.Span);

        // Convert the byte representation to Base64
        return ToUrlSafeBase64(bytesOwner.Span[..bytesWritten]);
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
    /// Attempts to decode a Base64-encoded string into a sequence of bytes and writes the result to the provided destination buffer.
    /// </summary>
    /// <param name="str">The Base64-encoded string to decode. Can be null or empty.</param>
    /// <param name="destination">A span of bytes where the decoded data will be written.</param>
    /// <param name="bytesWritten">The number of bytes written to the destination span.</param>
    /// <returns>
    /// <c>true</c> if the string was successfully decoded; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool TryFromString(string? str, Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;
        return !string.IsNullOrWhiteSpace(str) && TryFromChars(str.AsSpan(), destination, out bytesWritten);
    }

    /// <summary>
    /// Attempts to decode a Base64 or URL-safe Base64 encoded string represented as a read-only span of characters
    /// into a byte span.
    /// </summary>
    /// <param name="s">The input read-only span of characters to decode.</param>
    /// <param name="destination">The span where the decoded bytes will be written.</param>
    /// <param name="bytesWritten">When this method returns, contains the number of bytes written into the
    /// destination span.</param>
    /// <returns>
    /// <c>true</c> if the decoding was successful; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryFromChars(in ReadOnlySpan<char> s, Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;
        if (s.IsWhiteSpace())
        {
            return false;
        }

        // Ensure that the input is valid Base64 or URL-safe Base64
        var isUrlSafe = IsUrlSafeBase64(s);
        switch (isUrlSafe)
        {
            case false when !IsBase64(s):
                return false;
            case false:
                // Ensure that the input is padded correctly when it's not URL-safe
                {
                    var charsWithoutPadding = s.TrimEnd('=');
                    var neededPadding = 4 - (charsWithoutPadding.Length % 4);
                    if (neededPadding > 0 && s.Length != charsWithoutPadding.Length + neededPadding)
                    {
                        // Add padding to the end of the string
                        var charsLength = charsWithoutPadding.Length + neededPadding;
                        using var charsOwner = SpanOwner<char>.Allocate(charsLength);
                        charsWithoutPadding.CopyTo(charsOwner.Span);
                        charsOwner.Span[^neededPadding..].Fill('=');

                        return TryFromChars(
                            charsOwner.Span[..charsLength],
                            destination,
                            out bytesWritten
                        );
                    }

                    break;
                }
        }

        // Ensure that the destination buffer is big enough to hold the decoded data
        var expectedMinLength = isUrlSafe
            ? ThirdParty.LitJWT.Base64.GetMaxBase64UrlDecodeLength(s.Length)
            : ThirdParty.LitJWT.Base64.GetMaxBase64DecodeLength(s.Length);

        if (destination.Length < expectedMinLength)
        {
            return false;
        }

        // Try to decode the input
        return (!isUrlSafe && ThirdParty.LitJWT.Base64.TryFromBase64Chars(s, destination, out bytesWritten)) ||
               (isUrlSafe && ThirdParty.LitJWT.Base64.TryFromBase64UrlChars(s, destination, out bytesWritten));
    }

    /// <summary>
    /// Converts a Base64-encoded string into its corresponding byte array representation.
    /// </summary>
    /// <param name="str">The Base64-encoded string to convert. This can be null or empty.</param>
    /// <param name="result">When the method returns, contains the resulting byte array if the conversion was
    /// successful; otherwise, contains a default value.</param>
    /// <returns>
    /// <c>true</c> if the conversion was successful; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool TryFromString(string? str, out RentedArray<byte> result)
    {
        result = default;
        return !string.IsNullOrWhiteSpace(str) && TryFromChars(str.AsSpan(), out result);
    }

    /// <summary>
    /// Attempts to decode the provided span of characters as a Base64-encoded string.
    /// </summary>
    /// <param name="s">The read-only span of characters to decode.</param>
    /// <param name="result">When this method returns, contains the decoded data as a rented array, if the
    /// decode operation succeeded.</param>
    /// <returns>
    /// <c>true</c> if the span of characters was successfully decoded; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryFromChars(in ReadOnlySpan<char> s, out RentedArray<byte> result)
    {
        result = default;
        if (s.IsWhiteSpace())
        {
            return false;
        }

        var expectedMinLength = GetMaxDecodeLength(s);
        using var owner = SpanOwner<byte>.Allocate(expectedMinLength);
        if (!TryFromChars(s, owner.Span, out var bytesWritten))
        {
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
    /// An array containing the decoded <see cref="byte"/>s.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static byte[] FromString(string? s)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s);
        return FromChars(s.AsSpan());
    }

    /// <summary>
    /// Decodes a Base64-encoded read-only span of characters into a rented array of bytes.
    /// </summary>
    /// <param name="s">The read-only span of Base64-encoded or URL-safe Base64 characters to decode.</param>
    /// <returns>
    /// An array containing the decoded <see cref="byte"/>s.
    /// </returns>
    public static byte[] FromChars(in ReadOnlySpan<char> s)
    {
        if (s.IsWhiteSpace())
        {
            throw new ArgumentException(
                "The value cannot be an empty string or composed entirely of whitespace.",
                nameof(s)
            );
        }

        using var owner = SpanOwner<byte>.Allocate(GetMaxDecodeLength(s));
        if (!TryFromChars(s, owner.Span, out var bytesWritten))
        {
            throw new FormatException(
                "The input is not a valid Base-64 string as it contains a non-base 64 character, more than two " +
                "padding characters, or an illegal character among the padding characters."
            );
        }

        var result = new byte[bytesWritten];
        owner.Span[..bytesWritten].CopyTo(result.AsSpan());

        return result;
    }
}