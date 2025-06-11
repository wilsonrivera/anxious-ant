namespace AnxiousAnt;

partial class Url
{
    /// <summary>
    /// Gets the <see cref="IEqualityComparer{T}"/> for comparing <see cref="Url"/> instances.
    /// </summary>
    public static IEqualityComparer<Url> Comparer { get; } = new UrlComparer();

    private sealed class UrlComparer : IEqualityComparer<Url>
    {
        /// <inheritdoc />
        public bool Equals(Url? x, Url? y)
        {
            if (ReferenceEquals(null, x))
            {
                return ReferenceEquals(y, null);
            }

            return !ReferenceEquals(null, y) && x.Equals(y);
        }

        /// <inheritdoc />
        public int GetHashCode(Url? obj) => obj is null ? 0 : obj.GetHashCode();
    }
}