using System.Net;
using System.Net.Sockets;

using AnxiousAnt.Text;

namespace AnxiousAnt.Net;

/// <summary>
/// Provides extension methods for the <see cref="IPAddress"/> class.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IPAddressExtensions
{
    private const int BytesToAllocate = 32;

    /// <summary>
    /// Converts an <see cref="IPAddress"/> to a string representation with expanded format for IPv6 addresses.
    /// </summary>
    /// <param name="address">
    /// The <see cref="IPAddress"/> to convert. This must not be null.
    /// </param>
    /// <returns>
    /// A new <see cref="IPAddress"/> instance if the input is an IPv6 address with expanded string representation,
    /// or the original <see cref="IPAddress"/> if it is not an IPv6 address or if it cannot be processed.
    /// </returns>
    public static string ToExtendedString(this IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        if (address.AddressFamily is not AddressFamily.InterNetworkV6)
        {
            return address.ToString();
        }

        using var owner = SpanOwner<byte>.Allocate(BytesToAllocate);
        _ = address.TryWriteBytes(owner.Span, out _);

        var sb = StringBuilderPool.Rent();
        for (var i = 0; i < 8; i++)
        {
            var value = (owner.Span[i * 2] << 8) | owner.Span[i * 2 + 1];
            sb.Append(value.ToString("x4")).Append(':');
        }

        sb.Length--;
        return StringBuilderPool.ToStringAndReturn(sb);
    }

    /// <summary>
    /// Computes a hash value for the given <see cref="IPAddress"/> using the FNV-1a hashing algorithm.
    /// </summary>
    /// <param name="address">
    /// The <see cref="IPAddress"/> to compute the hash for. This must not be null.
    /// </param>
    /// <returns>
    /// A 64-bit unsigned integer representing the computed hash value of the IP address.
    /// </returns>
    public static ulong Fold(this IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        using var owner = SpanOwner<byte>.Allocate(BytesToAllocate);
        _ = address.TryWriteBytes(owner.Span, out var bytesWritten);

        ulong result = 2166136261ul;
        for (var i = 0; i < bytesWritten; i++)
        {
            result ^= owner.Span[i];
            result *= 16777619ul;
        }

        return result;
    }
}