namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for the <see cref="Url"/> class.
/// </summary>
public static class UrlExtensions
{
    private const string DataScheme = "data";

    /// <summary>
    /// Determines whether the URL uses a secure scheme.
    /// </summary>
    /// <param name="url">The <see cref="Url"/> object to be checked.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Url"/>'s scheme is a secure protocol such as HTTPS, WSS, FTPS, or SFTP;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool HasSecureScheme(this Url url)
    {
        ArgumentNullException.ThrowIfNull(url);

        var scheme = url.Scheme;
        return !string.IsNullOrWhiteSpace(scheme) &&
               (
                   scheme.OrdinalEquals(Uri.UriSchemeHttps, true) ||
                   scheme.OrdinalEquals(Uri.UriSchemeWss, true) ||
                   scheme.OrdinalEquals(Uri.UriSchemeFtps, true) ||
                   scheme.OrdinalEquals(Uri.UriSchemeSftp, true)
               );
    }

    /// <summary>
    /// Determines whether the URL uses the HTTP or HTTPS scheme.
    /// </summary>
    /// <param name="url">The <see cref="Url"/> object to be checked.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Url"/>'s scheme is either HTTP or HTTPS; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasHttpOrHttpsScheme(this Url url)
    {
        ArgumentNullException.ThrowIfNull(url);

        var scheme = url.Scheme;
        return !string.IsNullOrWhiteSpace(scheme) &&
               (
                   scheme.OrdinalEquals(Uri.UriSchemeHttp, true) ||
                   scheme.OrdinalEquals(Uri.UriSchemeHttps, true)
               );
    }

    /// <summary>
    /// Determines whether the <see cref="Url"/> uses the "data" scheme.
    /// </summary>
    /// <param name="url">The <see cref="Url"/> object to be checked.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Url"/>'s scheme is "data"; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDataUrl(this Url url)
    {
        ArgumentNullException.ThrowIfNull(url);

        var scheme = url.Scheme;
        return !string.IsNullOrWhiteSpace(scheme) &&
               scheme.OrdinalEquals(DataScheme, true);
    }

    /// <summary>
    /// Determines whether the <see cref="Url"/> uses the "file" scheme.
    /// </summary>
    /// <param name="url">The <see cref="Url"/> object to be checked.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Url"/> object's scheme is <see cref="Uri.UriSchemeFile"/>; otherwise,
    /// <c>false</c>.
    /// </returns>
    public static bool IsFileUrl(this Url url)
    {
        ArgumentNullException.ThrowIfNull(url);

        var scheme = url.Scheme;
        return !string.IsNullOrWhiteSpace(scheme) &&
               scheme.OrdinalEquals(Uri.UriSchemeFile, true);
    }
}