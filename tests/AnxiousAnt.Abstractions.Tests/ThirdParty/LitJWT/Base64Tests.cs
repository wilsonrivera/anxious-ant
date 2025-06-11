// Adapted from https://github.com/Cysharp/LitJWT/blob/ae03441aff175679aa7a6184b028e3f1759fc9af/tests/LitJWT.Tests/Base64Test.cs

using System.Security.Cryptography;

namespace AnxiousAnt.ThirdParty.LitJWT;

public class Base64Tests
{
#pragma warning disable CA2014
    [Fact]
    public void Base64Encode()
    {
        Span<char> writeTo = new char[1024];
        for (var i = 0; i < 1000; i++)
        {
            var item = RandomNumberGenerator.GetBytes(Random.Shared.Next(0, 33));
            var reference = Convert.ToBase64String(item);

            Base64.TryToBase64Chars(item, writeTo, out var written);
            var implResult = new string(writeTo[..written]);
            implResult.ShouldBe(reference);

            Base64.EncodeToBase64String(item).ShouldBe(reference);

            // ReSharper disable once StackAllocInsideLoop
            Span<byte> writeToUtf8 = stackalloc byte[Base64.GetBase64EncodeLength(item.Length)];
            Base64.TryToBase64Utf8(item, writeToUtf8, out var bytesWritten);
            Encoding.UTF8.GetString(writeToUtf8[..bytesWritten]).ShouldBe(reference);
        }
    }
#pragma warning restore CA2014

    [Fact]
    public void Base64Decode()
    {
        Span<byte> writeTo = new byte[1024];
        for (var i = 0; i < 1000; i++)
        {
            var item = RandomNumberGenerator.GetBytes(Random.Shared.Next(0, 33));
            var referenceString = Convert.ToBase64String(item);

            Base64.TryFromBase64Chars(referenceString, writeTo, out var written);
            var implResult = writeTo[..written];

            implResult.SequenceEqual(item).ShouldBeTrue(
                $"Str:{referenceString} Reference:{string.Join(",", item)} Actual:{string.Join(",", implResult.ToArray())}"
            );

            Base64.TryFromBase64Utf8(Encoding.UTF8.GetBytes(referenceString), writeTo, out written);
            implResult = writeTo[..written];
            implResult.SequenceEqual(item).ShouldBeTrue(
                $"Str:{referenceString} Reference:{string.Join(",", item)} Actual:{string.Join(",", implResult.ToArray())}"
            );
        }
    }

    [Fact]
    public void EdgeCaseEncode()
    {
        Span<char> writeTo = new char[1024];
        foreach (var item in new byte[][] { [] })
        {
            var reference = Convert.ToBase64String(item);

            Base64.TryToBase64Chars(item, writeTo, out var written);
            var implResult = new string(writeTo.Slice(0, written));

            implResult.ShouldBe(reference);
        }
    }

    [Fact]
    public void InvalidDecodeCases()
    {
        Span<byte> writeTo = new byte[1024];
        Base64.TryFromBase64Chars("AAB!DEFG", writeTo, out var written).ShouldBeFalse();
        Base64.TryFromBase64Chars("AAB=DEFG", writeTo, out written).ShouldBeFalse();
        Base64.TryFromBase64Chars("AABCDEF!", writeTo, out written).ShouldBeFalse();
        Base64.TryFromBase64Chars("AABCDEF", writeTo, out written).ShouldBeFalse();
        Base64.TryFromBase64Chars("=", writeTo, out written).ShouldBeFalse();
        Base64.TryFromBase64Chars("A", writeTo, out written).ShouldBeFalse();
        Base64.TryFromBase64Chars("A===", writeTo, out written).ShouldBeFalse();
        Base64.TryFromBase64Chars("A!!!", writeTo, out written).ShouldBeFalse();
    }

    [Fact]
    public void EncodeLength()
    {
        for (int i = 0; i < 99; i++)
        {
            Span<char> chars = new char[1000];
            Base64.TryToBase64Chars(new byte[i], chars, out var actual);

            actual.ShouldBe(Base64.GetBase64EncodeLength(i));
        }
    }

    [Fact]
    public void DecodeLength()
    {
        for (int i = 0; i < 99; i++)
        {
            Span<char> chars = new char[1000];
            Base64.TryToBase64Chars(new byte[i], chars, out var actual);
            i.ShouldBeLessThanOrEqualTo(Base64.GetMaxBase64DecodeLength(actual));
        }
    }
}