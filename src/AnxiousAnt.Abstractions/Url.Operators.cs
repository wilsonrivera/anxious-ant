namespace AnxiousAnt;

partial class Url
{
    /// <summary>
    /// Determines whether two <see cref="Url"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="Url"/> to compare.</param>
    /// <param name="right">The second <see cref="Url"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="Url"/> instances are equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator ==(Url? left, Url? right)
    {
        if (ReferenceEquals(null, left))
        {
            return ReferenceEquals(null, right);
        }

        return !ReferenceEquals(null, right) && left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="Url"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="Url"/> to compare.</param>
    /// <param name="right">The second <see cref="Url"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="Url"/> instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator !=(Url? left, Url? right) => !(left == right);

    /// <summary>
    /// Implicitly converts a <see cref="Uri"/> to a <see cref="Url"/>.
    /// </summary>
    /// <param name="uri">The <see cref="Uri"/> instance to convert.</param>
    /// <returns>
    /// A new <see cref="Url"/> instance created from the provided <paramref name="uri"/>, or <c>null</c> if
    /// <paramref name="uri"/> is <c>null</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    [return: NotNullIfNotNull(nameof(uri))]
    public static implicit operator Url?(Uri? uri) => uri is null ? null : new Url(uri);

    /// <summary>
    /// Appends a path segment to the current <see cref="Url"/>.
    /// </summary>
    /// <param name="url">The base <see cref="Url"/> to which the segment will be appended.</param>
    /// <param name="segmentToAdd">The path segment to append to the <paramref name="url"/>.</param>
    /// <returns>
    /// A new <see cref="Url"/> instance with the appended path segment.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <c>null</c>.</exception>
    [ExcludeFromCodeCoverage]
    public static Url operator /(Url url, string segmentToAdd)
    {
        ArgumentNullException.ThrowIfNull(url);
        return url.AddPathSegment(segmentToAdd);
    }
}