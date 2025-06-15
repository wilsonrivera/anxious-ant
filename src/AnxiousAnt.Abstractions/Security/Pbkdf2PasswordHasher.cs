using System.Security.Cryptography;

using AnxiousAnt.Collections;

namespace AnxiousAnt.Security;

/// <summary>
/// Provides methods to hash and verify passwords using the PBKDF2 algorithm.
/// </summary>
public static class Pbkdf2PasswordHasher
{
    internal const int V1FormatMarker = 0x01;
    private const int SaltLength = 128 / 8;
    private const int NumberOfBytesRequested = 384 / 8;
    private const int IterationsCount = 1_000_000;

    private static readonly Lazy<RandomNumberGenerator> LazyRng = new(RandomNumberGenerator.Create);

    /// <summary>
    /// Hashes the provided password using the PBKDF2 algorithm.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>
    /// A base64-encoded hash of the password.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static string Hash(string? password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return Hash(password.AsSpan());
    }

    /// <summary>
    /// Hashes the provided password using the PBKDF2 algorithm.
    /// </summary>
    /// <param name="password">The password to hash as an array of bytes.</param>
    /// <returns>
    /// A base64-encoded hash of the password.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static string Hash(byte[] password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return Hash(password.AsSpan());
    }

    /// <summary>
    /// Hashes the provided password using the PBKDF2 algorithm.
    /// </summary>
    /// <param name="password">The password to hash as a read-only character span.</param>
    /// <returns>
    /// A base64-encoded string representation of the hashed password.
    /// </returns>
    public static string Hash(in ReadOnlySpan<char> password)
    {
        var passwordByteCount = EncodingUtils.SecureUtf8Encoding.GetByteCount(password);
        using var passwordBytesOwner = SpanOwner<byte>.Allocate(passwordByteCount);
        var passwordLength = Encoding.UTF8.GetBytes(password, passwordBytesOwner.Span);

        return Hash(passwordBytesOwner.Span[..passwordLength]);
    }

    /// <summary>
    /// Hashes the provided password using the PBKDF2 algorithm with a byte span input.
    /// </summary>
    /// <param name="password">The password represented as a read-only span of bytes.</param>
    /// <returns>
    /// A base64-encoded string representing the hashed password.
    /// </returns>
    public static string Hash(in ReadOnlySpan<byte> password)
    {
        using var saltOwner = SpanOwner<byte>.Allocate(SaltLength);
        LazyRng.Value.GetBytes(saltOwner.Span);

        using var hash = DerivePasswordBytes(
            password,
            saltOwner.Span[..SaltLength],
            V1FormatMarker,
            KeyDerivationPrf.HmacSha512,
            NumberOfBytesRequested,
            IterationsCount
        );

        return Base64.ToBase64(hash.Span);
    }

    /// <summary>
    /// Verifies whether the provided password matches the hashed password using the PBKDF2 algorithm.
    /// </summary>
    /// <param name="hashedPassword">The previously hashed password to verify against.</param>
    /// <param name="providedPassword">The plain text password to verify.</param>
    /// <returns>
    /// A boolean value indicating whether the provided password matches the hashed password.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool Verify(string hashedPassword, string providedPassword)
    {
        ArgumentNullException.ThrowIfNull(hashedPassword);
        ArgumentNullException.ThrowIfNull(providedPassword);

        return Verify(hashedPassword.AsSpan(), providedPassword.AsSpan());
    }

    /// <summary>
    /// Verifies whether the provided password matches the given hashed password.
    /// </summary>
    /// <param name="hashedPassword">The hashed password to verify against.</param>
    /// <param name="providedPassword">The provided password to verify.</param>
    /// <returns>
    /// A boolean indicating whether the provided password matches the hashed password.
    /// </returns>
    public static bool Verify(in ReadOnlySpan<char> hashedPassword, in ReadOnlySpan<char> providedPassword)
    {
        if (hashedPassword.IsWhiteSpace())
        {
            // The hashed password seems empty or comprised of only whitespaces
            return false;
        }

        // First, we need to base64 decode the hashed password
        var hashDecodeLength = Base64.GetMaxDecodeLength(hashedPassword);
        using var hashOwner = SpanOwner<byte>.Allocate(hashDecodeLength);
        if (!Base64.TryFromChars(hashedPassword, hashOwner.Span, out var hashLength) || hashLength == 0)
        {
            // Either the hashed password is not a valid base64 representation, or the length of the decode
            // base64 bytes is invalid
            return false;
        }

        // Verify whether the given password hash and plain text password match
        var passwordHash = hashOwner.Span[..hashLength];
        return passwordHash[0] is V1FormatMarker &&
               VerifyPasswordBytes(passwordHash, providedPassword, V1FormatMarker, out _, out _);
    }

    private static uint ReadNetworkByteOrder(in ReadOnlySpan<byte> buffer, int offset)
    {
        return ((uint)(buffer[offset + 0]) << 24)
               | ((uint)(buffer[offset + 1]) << 16)
               | ((uint)(buffer[offset + 2]) << 8)
               | ((buffer[offset + 3]));
    }

    internal static void WriteNetworkByteOrder(Span<byte> buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }

    private static RentedArray<byte> DerivePasswordBytes(
        in ReadOnlySpan<byte> password,
        in ReadOnlySpan<byte> salt,
        byte formatMarker,
        KeyDerivationPrf prf,
        int numberOfBytesRequested,
        int iterationsCount)
    {
        var saltLength = salt.Length;

        using var subkeyOwner = SpanOwner<byte>.Allocate(numberOfBytesRequested);
        Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            subkeyOwner.Span[..numberOfBytesRequested],
            iterationsCount,
            prf switch
            {
                KeyDerivationPrf.HmacSha1 => HashAlgorithmName.SHA1,
                KeyDerivationPrf.HmacSha256 => HashAlgorithmName.SHA256,
                KeyDerivationPrf.HmacSha512 => HashAlgorithmName.SHA512,
                _ => throw new ArgumentOutOfRangeException(nameof(prf))
            }
        );

        using var outputOwner = SpanOwner<byte>.Allocate(13 + saltLength + numberOfBytesRequested);
        outputOwner.Span[0] = formatMarker;
        WriteNetworkByteOrder(outputOwner.Span, 1, (uint)prf);
        WriteNetworkByteOrder(outputOwner.Span, 5, (uint)iterationsCount);
        WriteNetworkByteOrder(outputOwner.Span, 9, (uint)saltLength);

        salt.CopyTo(outputOwner.Span[13..]);
        subkeyOwner.Span[..numberOfBytesRequested].CopyTo(outputOwner.Span[(13 + saltLength)..]);

        var finalOutputLength = 13 + saltLength + numberOfBytesRequested;
        return RentedArray<byte>.FromSpan(outputOwner.Span[..finalOutputLength]);
    }

    internal static bool VerifyPasswordBytes(
        in ReadOnlySpan<byte> hashedPassword,
        in ReadOnlySpan<char> providedPassword,
        byte formatMarker,
        out KeyDerivationPrf prf,
        out int iterationsCount)
    {
        prf = default;
        iterationsCount = 0;
        if (hashedPassword.Length <= 13)
        {
            // We need a minimum of 13 bytes for the hash header
            return false;
        }

        try
        {
            // Read header information
            prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
            iterationsCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
            var saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

            // Ensure that prf and the iterations count are valid
            if (prf > KeyDerivationPrf.HmacSha512 || iterationsCount < 1)
            {
                return false;
            }

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8 || saltLength > hashedPassword.Length - 13)
            {
                return false;
            }

            using var saltOwner = SpanOwner<byte>.Allocate(saltLength);
            hashedPassword.Slice(13, saltLength).CopyTo(saltOwner.Span);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            int subkeyLength = hashedPassword.Length - 13 - saltOwner.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }

            using var expectedSubkeyOwner = SpanOwner<byte>.Allocate(subkeyLength);
            hashedPassword[(13 + saltLength)..].CopyTo(expectedSubkeyOwner.Span);

            // Convert the provided password to bytes
            var passwordByteCount = EncodingUtils.SecureUtf8Encoding.GetByteCount(providedPassword);
            using var passwordBytesOwner = SpanOwner<byte>.Allocate(passwordByteCount);
            var passwordLength = Encoding.UTF8.GetBytes(providedPassword, passwordBytesOwner.Span);

            // Calculate the derived key based on the extracted parameters
            using var actualSubkey = DerivePasswordBytes(
                passwordBytesOwner.Span[..passwordLength],
                saltOwner.Span,
                formatMarker,
                prf,
                subkeyLength,
                iterationsCount
            );

            return CryptographicOperations.FixedTimeEquals(
                actualSubkey.Span[(13 + saltLength)..],
                expectedSubkeyOwner.Span
            );
        }
        catch
        {
            // This should never occur except in the case of a malformed payload, where
            // we might go off the end of the array. Regardless, a malformed payload
            // implies verification failed.
            return false;
        }
    }

    internal enum KeyDerivationPrf : byte
    {
        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-1 hash function (FIPS 180-4).
        /// </summary>
        HmacSha1,

        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-256 hash function (FIPS 180-4).
        /// </summary>
        HmacSha256,

        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-512 hash function (FIPS 180-4).
        /// </summary>
        HmacSha512,
    }
}