// Inspired by https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Url.cs

using System.Globalization;

using AnxiousAnt.Http;
using AnxiousAnt.Text;

namespace AnxiousAnt;

/// <summary>
/// Represents a URL with various components and provides methods for parsing, manipulating,
/// and converting URLs into their constituent parts or full representations.
/// </summary>
public sealed partial class Url : IEquatable<Url>
{
    private const string EmptySlashPath = "/";

    private static readonly Lazy<IdnMapping> LazyIdnMapping = new(static () => new IdnMapping());

    private static readonly SearchValues<char> AsciiHostNameChars =
        SearchValues.Create("abcdefgjijklmnopqrstuvwxyz0123456789.-");

    private string? _userInfo;
    private string? _authority;
    private string? _root;
    private List<string>? _pathSegments;
    private string? _path;
    private bool _hasLeadingSlash;
    private bool _hasTrailingSlash;
    private bool _changed;
    private Uri? _uri;

    public Url()
    {
        QueryParams = new QueryParamCollection();
    }

    private Url(Uri uri)
    {
        _pathSegments = ExtractPathSegments(uri.AbsolutePath);
        _uri = uri;

        Scheme = uri.Scheme;
        Host = uri.Host;
        Port = uri.IsDefaultPort ? null : (ushort)uri.Port;
        Fragment = !string.IsNullOrWhiteSpace(uri.Fragment) ? uri.Fragment.TrimStart('#') : string.Empty;
        QueryParams = new QueryParamCollection(uri.Query);

        ParseUserInfo(uri.UserInfo);
    }

    private Url(Url other)
    {
        ArgumentNullException.ThrowIfNull(other);

        _userInfo = other._userInfo;
        _authority = other._authority;
        _root = other._root;
        _path = other._path;
        _hasLeadingSlash = other._hasLeadingSlash;
        _hasTrailingSlash = other._hasTrailingSlash;
        _changed = other._changed;
        _uri = other._uri;

        if (other._pathSegments?.Count is > 0)
        {
            _pathSegments = [];
            _pathSegments.AddRange(other._pathSegments);
        }

        Scheme = other.Scheme;
        Username = other.Username;
        Password = other.Password;
        Host = other.Host;
        Port = other.Port;
        QueryParams = other.QueryParams.Clone();
        Fragment = other.Fragment;
    }

    /// <summary>
    /// Gets the scheme (e.g., "http", "https").
    /// </summary>
    public string Scheme { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the username for authentication.
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the password for authentication.
    /// </summary>
    public string Password { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the host (e.g., "example.com").
    /// </summary>
    public string Host { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the port number.
    /// </summary>
    public int? Port { get; private set; }

    /// <summary>
    /// Gets the path segments as a read-only list.
    /// </summary>
    /// <remarks>
    /// Returns an empty array if the path is empty or null.
    /// </remarks>
    public IReadOnlyList<string> PathSegments => _pathSegments ?? [];

    /// <summary>
    /// Gets the collection of query parameters.
    /// </summary>
    public QueryParamCollection QueryParams { get; }

    /// <summary>
    /// Gets the fragment identifier (e.g., "#section").
    /// </summary>
    public string Fragment { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the user info (username and password) as a single string.
    /// </summary>
    /// <remarks>
    /// Combines the username and password with a colon separator if both are present.
    /// </remarks>
    public string UserInfo
    {
        get
        {
            if (_userInfo is not null)
            {
                return _userInfo;
            }

            if (string.IsNullOrEmpty(Username))
            {
                return string.Empty;
            }

            var sb = StringBuilderPool.Rent().Append(Username);
            if (!string.IsNullOrEmpty(Password))
            {
                sb.Append(':').Append(Password);
            }

            return _userInfo = StringBuilderPool.ToStringAndReturn(sb);
        }
    }

    /// <summary>
    /// Gets the authority (host and port) as a single string.
    /// </summary>
    /// <remarks>
    /// Includes the user info (if available) followed by the host and optional port.
    /// </remarks>
    public string Authority
    {
        get
        {
            if (_authority is not null)
            {
                return _authority;
            }

            if (string.IsNullOrEmpty(Host))
            {
                return string.Empty;
            }

            var sb = StringBuilderPool.Rent();

            sb.Append(UserInfo);
            sb.AppendIfNotEmpty('@').Append(Host);
            if (Port is > 0 && sb.Length > 0)
            {
                sb.Append(':').Append(Port.Value);
            }

            return _authority = StringBuilderPool.ToStringAndReturn(sb);
        }
    }

    /// <summary>
    /// Gets the root (scheme and authority) as a single string.
    /// </summary>
    /// <remarks>
    /// Combines the scheme, host, and port into a full base URL without a path or query string.
    /// </remarks>
    public string Root
    {
        get
        {
            if (_root is not null)
            {
                return _root;
            }

            var authority = Authority;
            if (string.IsNullOrEmpty(Scheme) && string.IsNullOrEmpty(authority))
            {
                return string.Empty;
            }

            var sb = StringBuilderPool.Rent().Append(Scheme);
            if (!string.IsNullOrEmpty(authority))
            {
                sb.AppendIfNotEmpty("://").Append(authority);
            }

            return _root = StringBuilderPool.ToStringAndReturn(sb);
        }
    }

    /// <summary>
    /// Gets or sets the full path as a single string.
    /// </summary>
    /// <remarks>
    /// Combines all path segments into a single string starting with a leading "/".
    /// If there are no path segments, returns "/".
    /// </remarks>
    public string Path
    {
        get
        {
            if (_path is not null)
            {
                return _path;
            }

            if (_pathSegments?.Count is not > 0)
            {
                return _hasLeadingSlash ? EmptySlashPath : string.Empty;
            }

            var sb = StringBuilderPool.Rent();
            if (_hasLeadingSlash)
            {
                sb.Append(EmptySlashPath);
            }

            sb.AppendJoin(EmptySlashPath, _pathSegments);
            if (_hasTrailingSlash)
            {
                sb.Append(EmptySlashPath);
            }

            return _path = StringBuilderPool.ToStringAndReturn(sb);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Url"/> is relative.
    /// </summary>
    public bool IsRelative => string.IsNullOrWhiteSpace(Scheme);

    /// <inheritdoc />
    public override string ToString() => ToUri().OriginalString;

    /// <inheritdoc />
    public override int GetHashCode() => ToUri().GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj switch
        {
            Uri uri => ToUri().Equals(uri),
            Url url => Equals(url),
            _ => false
        };

    /// <inheritdoc />
    public bool Equals(Url? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return ReferenceEquals(this, other) || ToUri().Equals(other.ToUri());
    }

    /// <summary>
    /// Converts the URL to a <see cref="Uri"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="Uri"/> representation of the URL.
    /// </returns>
    /// <remarks>
    /// This method constructs the URI by combining all components, including the scheme, authority, path,
    /// query parameters, and fragment.
    /// </remarks>
    public Uri ToUri()
    {
        if (!_changed && _uri is not null)
        {
            return _uri;
        }

        var sb = StringBuilderPool.Rent();
        sb.Append(Root).Append(Path);
        if (QueryParams.Count > 0)
        {
            sb.AppendIfNotEmpty('?').Append(QueryParams);
        }

        if (!string.IsNullOrWhiteSpace(Fragment))
        {
            if (!Fragment.OrdinalStartsWith("#"))
            {
                sb.Append('#');
            }

            sb.Append(Fragment);
        }

        _uri = new Uri(StringBuilderPool.ToStringAndReturn(sb), UriKind.RelativeOrAbsolute);
        _changed = false;
        return _uri;
    }

    /// <summary>
    /// Creates a copy of this <see cref="Url"/>.
    /// </summary>
    /// <returns>
    /// The created <see cref="Url"/>.
    /// </returns>
    [Pure]
    public Url Clone() => new(this);

    /// <summary>
    /// Resets the URL to its root, including the scheme, any user info, host, and port (if specified).
    /// </summary>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url ResetToRoot()
    {
        _pathSegments?.Clear();
        _hasLeadingSlash = false;
        _hasTrailingSlash = false;

        QueryParams.Clear();
        Fragment = string.Empty;

        OnPathChange();
        return this;
    }

    private static List<string>? ExtractPathSegments(string path)
    {
        var pathSpan = path.AsSpan();
        if (pathSpan.TrimStart('/').IsWhiteSpace())
        {
            return null;
        }

        var separatorCount = ReadOnlySpanExtensions.Count(pathSpan, '/');
        using var splitOwner = SpanOwner<Range>.Allocate(separatorCount + 1);

        var splitDestination = splitOwner.Span;
        var splitCount = pathSpan.Split(splitDestination, '/', StringSplitOptions.RemoveEmptyEntries);
        if (splitCount == 0)
        {
            return null;
        }

        var result = new List<string>(separatorCount);
        for (var i = 0; i < splitCount; i++)
        {
            var range = splitDestination[i];
            var segment = path[range];
            result.Add(segment);
        }

        return result;
    }

    private void OnUserInfoChange()
    {
        _userInfo = null;
        OnAuthorityChange();
    }

    private void OnAuthorityChange()
    {
        _authority = null;
        OnRootChange();
    }

    private void OnRootChange()
    {
        _root = null;
        _changed = true;
    }

    private void OnPathChange()
    {
        _path = null;
        _changed = true;
    }
}