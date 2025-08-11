namespace AnxiousAnt;

partial class Url
{
    /// <summary>
    /// Determines whether the specified host name is valid.
    /// </summary>
    /// <param name="host">The host name to validate. This can be null or empty.</param>
    /// <returns>
    /// <c>true</c> if the host name is valid; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidHostName(string? host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return false;
        }

        // Handle the host name type
        var hostSpan = host.AsSpan();
        var hostType = Uri.CheckHostName(host);
        switch (hostType)
        {
            case UriHostNameType.Unknown:
                // An unknown host name type means we don't support it :)
                return false;
            case UriHostNameType.Basic:
            case UriHostNameType.Dns:
                if (hostSpan.ContainsAnyExcept(AsciiHostNameChars))
                {
                    try
                    {
                        host = LazyIdnMapping.Value.GetAscii(host);
                    }
                    catch
                    {
                        return false;
                    }

                    hostSpan = host.AsSpan();
                    if (hostSpan.ContainsAnyExcept(AsciiHostNameChars))
                    {
                        return false;
                    }
                }

                break;
            case UriHostNameType.IPv4:
            case UriHostNameType.IPv6:
                return true;
        }

        // Ensure that the length of the hostname doesn't exceeds the limit
        if (hostSpan.Length > 253)
        {
            return false;
        }

        // Validate each label
        foreach (var range in hostSpan.Split("."))
        {
            var label = hostSpan[range];
            if (label.IsEmpty || label.Length > 63 ||
                label.StartsWith("-", StringComparison.Ordinal) ||
                label.EndsWith("-", StringComparison.Ordinal))
            {
                return false;
            }

            if (label.StartsWith(InternationalHostNamePrefix, StringComparison.OrdinalIgnoreCase))
            {
                // International hostnames can start with this prefix
                label = label[(InternationalHostNamePrefix.Length)..];
            }

            if (label.Contains("--", StringComparison.Ordinal))
            {
                // We are not allowed double dashes
                return false;
            }
        }

        // The hostname is valid
        return true;
    }
}