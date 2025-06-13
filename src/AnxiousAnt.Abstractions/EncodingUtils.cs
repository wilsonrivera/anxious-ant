namespace AnxiousAnt;

/// <summary>
/// Provides utility methods and properties related to encoding functionality.
/// </summary>
[ExcludeFromCodeCoverage]
public static class EncodingUtils
{
    private static readonly Lazy<Encoding> LazySecureUtf8Encoding =
        new(static () => new UTF8Encoding(false, true));

    /// <summary>
    /// Gets a thread-safe, secure UTF-8 encoding instance that does not emit a BOM (Byte Order Mark)
    /// and throws exceptions for invalid byte sequences.
    /// </summary>
    public static Encoding SecureUtf8Encoding => LazySecureUtf8Encoding.Value;
}