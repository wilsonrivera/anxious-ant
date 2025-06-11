using System.Runtime.InteropServices;

using AnxiousAnt.Http;

namespace AnxiousAnt;

partial class Url
{
    /// <summary>
    /// Sets the URL scheme to the specified value.
    /// </summary>
    /// <param name="scheme">The new scheme to set (e.g., "http", "https").</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url WithScheme(string? scheme)
    {
        Scheme = scheme.IfNullOrWhiteSpace(string.Empty);
        OnRootChange();
        return this;
    }

    /// <summary>
    /// Removes both the username and password from the URL's user info.
    /// </summary>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url ClearUserInfo()
    {
        Username = string.Empty;
        Password = string.Empty;
        OnUserInfoChange();
        return this;
    }

    /// <summary>
    /// Sets the username to the specified value.
    /// </summary>
    /// <param name="username">The new username to set.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url WithUsername(string? username)
    {
        Username = username.IfNullOrWhiteSpace(string.Empty);
        OnUserInfoChange();
        return this;
    }

    /// <summary>
    /// Sets the password to the specified value.
    /// </summary>
    /// <param name="password">The new password to set.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url WithPassword(string? password)
    {
        Password = password.IfNullOrWhiteSpace(string.Empty);
        OnUserInfoChange();
        return this;
    }

    /// <summary>
    /// Sets the host of the URL to the specified value.
    /// </summary>
    /// <param name="host">The new host to set (e.g., "example.com").</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url WithHost(string host)
    {
        Host = host.IfNullOrWhiteSpace(string.Empty);
        OnAuthorityChange();
        return this;
    }

    /// <summary>
    /// Converts the host of the URL to its ASCII representation if it is an Internationalized Domain Name (IDN).
    /// </summary>
    /// <returns>
    /// The current <see cref="Url"/> instance with the host updated to its ASCII representation, or the same
    /// instance if the host is already in a valid ASCII format or is null/empty.
    /// </returns>
    public Url WithAsciiHost()
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            return this;
        }

        var host = Host;
        var hostType = Uri.CheckHostName(host);
        return hostType is UriHostNameType.Basic or UriHostNameType.Dns &&
               host.AsSpan().ContainsAnyExcept(AsciiHostNameChars)
            ? new Url(this).WithHost(LazyIdnMapping.Value.GetAscii(host))
            : this;
    }

    /// <summary>
    /// Converts the current host to its Unicode representation if it contains Punycode encoding.
    /// </summary>
    /// <returns>
    /// The current <see cref="Url"/> instance with the Unicode host set, or the original instance
    /// if no conversion is necessary.
    /// </returns>
    public Url WithUnicodeHost()
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            return this;
        }

        var host = Host;
        var hostSpan = host.AsSpan();
        foreach (var part in hostSpan.Split('.'))
        {
            var value = hostSpan[part];
            if (value.StartsWith("xn--", StringComparison.Ordinal))
            {
                return new Url(this).WithHost(LazyIdnMapping.Value.GetUnicode(host));
            }
        }

        return this;
    }

    /// <summary>
    /// Sets the port of the URL to the specified value.
    /// </summary>
    /// <param name="port">The new port number to set.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url WithPort(ushort port)
    {
        Port = port;
        OnAuthorityChange();
        return this;
    }

    /// <summary>
    /// Removes the port from the URL.
    /// </summary>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url ClearPort()
    {
        Port = null;
        OnAuthorityChange();
        return this;
    }

    /// <summary>
    /// Adds a new path segment to the existing path.
    /// </summary>
    /// <param name="segment">The segment to add.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url AddPathSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
        {
            return this;
        }

        if (segment.Contains('/'))
        {
            var segmentSpan = segment.AsSpan();
            var segmentSeparatorCount = ReadOnlySpanExtensions.Count(segmentSpan, '/');
            using var owner = SpanOwner<Range>.Allocate(segmentSeparatorCount + 1);
            var splitCount = segmentSpan.Split(owner.Span, '/', StringSplitOptions.RemoveEmptyEntries);
            if (splitCount == 0)
            {
                return this;
            }

            _pathSegments ??= [];
            ref var searchSpace = ref MemoryMarshal.GetReference(owner.Span);
            for (var i = 0; i < splitCount; i++)
            {
                ref var range = ref Unsafe.Add(ref searchSpace, i);

                _pathSegments.Add(Uri.EscapeDataString(segment[range]));
            }

            _hasLeadingSlash |= !IsRelative;
            _hasTrailingSlash = segmentSpan.EndsWith('/');
            OnPathChange();
        }
        else
        {
            _pathSegments ??= [];
            _pathSegments.Add(Uri.EscapeDataString(segment));
            _hasLeadingSlash |= !IsRelative;
            _hasTrailingSlash = false;
            OnPathChange();
        }

        return this;
    }

    /// <summary>
    /// Adds multiple path segments to the existing URL path.
    /// </summary>
    /// <param name="segments">The segments to add.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url AddPathSegments(params ReadOnlySpan<string> segments)
    {
        if (segments.IsEmpty)
        {
            return this;
        }

        ref var searchSpace = ref MemoryMarshal.GetReference(segments);
        for (var i = 0; i < segments.Length; i++)
        {
            ref var segment = ref Unsafe.Add(ref searchSpace, i);
            AddPathSegment(segment);
        }

        return this;
    }

    /// <summary>
    /// Removes and returns the last segment of the URL path, if any exist.
    /// </summary>
    /// <returns>
    /// The last path segment that was removed, or null if no segments are present.
    /// </returns>
    public string? PopPathSegment()
    {
        if (_pathSegments is not { Count: > 0 })
        {
            return null;
        }

        var popped = _pathSegments[^1];
        _pathSegments.RemoveAt(_pathSegments.Count - 1);
        OnPathChange();

        return popped;
    }

    /// <summary>
    /// Clears all path segments, effectively resetting the path to "/".
    /// </summary>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url ClearPath()
    {
        _pathSegments?.Clear();
        _hasLeadingSlash = false;
        _hasTrailingSlash = false;

        OnPathChange();
        return this;
    }

    /// <summary>
    /// Adds a query parameter without a value to the URL.
    /// </summary>
    /// <param name="key">The query parameter key.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Url AddQueryParam(string key) => AddQueryParam(key, null, QueryParamCollection.NullValueHandling.NameOnly);

    /// <summary>
    /// Adds a query parameter to the URL with an optional value.
    /// </summary>
    /// <param name="key">The query parameter key.</param>
    /// <param name="value">The query parameter value.</param>
    /// <param name="nullValueHandling">Specifies how to handle <c>null</c> values.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url AddQueryParam(
        string key,
        string? value,
        QueryParamCollection.NullValueHandling nullValueHandling = QueryParamCollection.NullValueHandling.Remove)
    {
        QueryParams.Add(key, value, nullValueHandling);

        _changed = true;
        return this;
    }

    /// <summary>
    /// Add or updates a query parameter with the specified key.
    /// </summary>
    /// <param name="key">The key of the query parameter to set.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Url SetQueryParam(string key) => SetQueryParam(key, null, QueryParamCollection.NullValueHandling.NameOnly);

    /// <summary>
    /// Adds or updates the query parameter with the given key and value, handling null values as specified.
    /// </summary>
    /// <param name="key">The key of the query parameter to set.</param>
    /// <param name="value">The value of the query parameter. Can be null.</param>
    /// <param name="nullValueHandling">Determines how to handle null values for the query parameter.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url SetQueryParam(
        string key,
        string? value,
        QueryParamCollection.NullValueHandling nullValueHandling = QueryParamCollection.NullValueHandling.Remove)
    {
        QueryParams.AddOrReplace(key, value, nullValueHandling);

        _changed = true;
        return this;
    }

    /// <summary>
    /// Removes the specified query parameter from the URL.
    /// </summary>
    /// <param name="key">The query parameter key to remove.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url RemoveQueryParam(string key)
    {
        QueryParams.Remove(key);

        _changed = true;
        return this;
    }

    /// <summary>
    /// Removes multiple query parameters from the URL.
    /// </summary>
    /// <param name="keys">The keys to remove.</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url RemoveQueryParams(params ReadOnlySpan<string> keys)
    {
        ref var searchSpace = ref MemoryMarshal.GetReference(keys);
        for (var i = 0; i < keys.Length; i++)
        {
            ref var key = ref Unsafe.Add(ref searchSpace, i);
            QueryParams.Remove(key);
        }

        _changed = true;
        return this;
    }

    /// <summary>
    /// Clears all query parameters from the URL.
    /// </summary>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url ClearQuery()
    {
        QueryParams.Clear();
        _changed = true;

        return this;
    }

    /// <summary>
    /// Sets the URL fragment to the specified value.
    /// </summary>
    /// <param name="fragment">The part of the URL after #</param>
    /// <returns>
    /// The current <see cref="Url"/> instance for chaining.
    /// </returns>
    public Url WithFragment(string? fragment)
    {
        Fragment = fragment.IfNullOrWhiteSpace(string.Empty);

        _changed = true;
        return this;
    }
}