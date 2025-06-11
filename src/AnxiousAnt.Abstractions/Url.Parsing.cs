namespace AnxiousAnt;

partial class Url : IParsable<Url>
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    static Url IParsable<Url>.Parse([NotNullWhen(true)] string? s, IFormatProvider? provider) => Parse(s);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    static bool IParsable<Url>.TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Url result) =>
        TryParse(s, out result);

    /// <summary>
    /// Parses a string into a <see cref="Url"/> instance.
    /// </summary>
    /// <param name="urlString">The URL string to parse.</param>
    /// <returns>
    /// The parsed <see cref="Url"/>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="urlString"/> is null or empty.</exception>
    /// <exception cref="FormatException">Thrown if the string cannot be parsed into a valid URL.</exception>
    public static Url Parse(string? urlString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(urlString);
        return TryParse(urlString, out var result) ? result : throw new FormatException();
    }

    /// <summary>
    /// Attempts to parse a string into a <see cref="Url"/> instance.
    /// </summary>
    /// <param name="urlString">The URL string to parse.</param>
    /// <param name="result">The parsed <see cref="Url"/> instance, or <c>null</c> if parsing failed.</param>
    /// <returns>
    /// <c>true</c> if parsing succeeded; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse([NotNullWhen(true)] string? urlString, [MaybeNullWhen(false)] out Url result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(urlString) || !Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out var uri))
        {
            return false;
        }

        var isRelative = false;
        if (uri.OriginalString.OrdinalStartsWith("//"))
        {
            if (!Uri.TryCreate($"http:{uri.OriginalString}", UriKind.Absolute, out uri))
            {
                return false;
            }
        }
        else if (uri.OriginalString.OrdinalStartsWith("/"))
        {
            uri = new Uri($"http://example.com{uri.OriginalString}", UriKind.Absolute);
        }
        else if (!uri.IsAbsoluteUri)
        {
            uri = new Uri($"http://example.com/{uri.OriginalString}", UriKind.Absolute);
            isRelative = true;
        }

        result = new Url(uri);
        result.ApplyQuirkFixes(uri, urlString, isRelative);
        return true;
    }

    private void ParseUserInfo(string userInfo)
    {
        if (string.IsNullOrWhiteSpace(userInfo))
        {
            return;
        }

        _userInfo = userInfo;

        int index = userInfo.IndexOf(':');
        if (index >= 0)
        {
            Username = userInfo[..index];
            Password = userInfo[(index + 1)..];
        }
        else
        {
            Username = userInfo;
        }
    }

    private void ApplyQuirkFixes(Uri uri, string originalString, bool isRelative)
    {
        if (isRelative)
        {
            Scheme = string.Empty;
            Host = string.Empty;
        }
        else
        {
            _hasLeadingSlash = uri.OriginalString.OrdinalStartsWith($"{Root}/", true);
            _hasTrailingSlash = _pathSegments?.Count is > 0 && uri.AbsolutePath.OrdinalEndsWith("/");

            var hasAuthority = uri.OriginalString.OrdinalStartsWith($"{Scheme}://", true);
            if (hasAuthority && string.IsNullOrWhiteSpace(Authority) && _pathSegments?.Count is > 0)
            {
                Host = _pathSegments[0];
                _pathSegments.RemoveAt(0);
            }
            else if (!hasAuthority && !string.IsNullOrWhiteSpace(Authority))
            {
                _pathSegments ??= [];
                _pathSegments.Insert(0, Authority);

                _changed = true;

                Username = string.Empty;
                Password = string.Empty;
                Host = string.Empty;
                Port = null;
            }
        }

        if (originalString.OrdinalStartsWith("//"))
        {
            Scheme = string.Empty;
        }
        else if (originalString.OrdinalStartsWith("/"))
        {
            _hasLeadingSlash = true;
            Scheme = string.Empty;
            Host = string.Empty;
        }

        _userInfo = null;
        _authority = null;
        _path = null;
        _root = null;
    }
}