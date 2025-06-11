namespace AnxiousAnt;

partial struct EmailAddress
{
    /// <summary>
    /// Gets the default <see cref="IEqualityComparer{T}"/> for <see cref="EmailAddress"/> instances,
    /// comparing all properties including address, display name, and other attributes.
    /// </summary>
    public static IEqualityComparer<EmailAddress> Comparer { get; } = new EmailAddressComparer();

    /// <summary>
    /// Gets a <see cref="IEqualityComparer{T}"/> that compares <see cref="EmailAddress"/> instances
    /// based solely on the address portion, ignoring display names and other properties.
    /// </summary>
    public static IEqualityComparer<EmailAddress> AddressOnlyComparer { get; } = new AddressOnlyEmailAddressComparer();

    private sealed class EmailAddressComparer : IEqualityComparer<EmailAddress>
    {
        public bool Equals(EmailAddress x, EmailAddress y) => x.Equals(y);
        public int GetHashCode(EmailAddress obj) => obj.GetHashCode();
    }

    private sealed class AddressOnlyEmailAddressComparer : IEqualityComparer<EmailAddress>
    {
        public bool Equals(EmailAddress x, EmailAddress y) => x.AddressEquals(y);

        public int GetHashCode(EmailAddress obj) =>
            !obj.IsValid ? 0 : string.GetHashCode(obj.Address, StringComparison.OrdinalIgnoreCase);
    }
}