using System.Diagnostics;
using System.Net.Mail;

namespace AnxiousAnt;

/// <summary>
/// Represents a well-formed email address.
/// </summary>
public readonly partial struct EmailAddress : IEquatable<EmailAddress>
{
    private const int MaxEmailLength = 320;
    private const int MaxLocalPartLength = 64;

    private static readonly SearchValues<char> IllegalHostCharacters = SearchValues.Create('_');

    /// <summary>
    /// Represents an empty or uninitialized <see cref="EmailAddress"/> instance.
    /// </summary>
    public static readonly EmailAddress Empty = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly string? _toString;

    public EmailAddress()
    {
        this = default;
    }

    private EmailAddress(MailAddress mailAddress)
        : this(mailAddress.Address, mailAddress.DisplayName, mailAddress.User, mailAddress.Host)
    {
    }

    private EmailAddress(string address, string? displayName, string? user, string? host)
    {
        _toString = GetStringRepresentation(address, displayName);

        IsValid = true;
        Address = address;
        DisplayName = displayName.IfNullOrWhiteSpace(null);
        User = user.IfNullOrWhiteSpace(null);
        Host = host.IfNullOrWhiteSpace(null);
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="EmailAddress"/> is valid.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Address))]
    public bool IsValid { get; }

    /// <summary>
    /// Gets the email address specified when this instance was created.
    /// </summary>
    public string? Address { get; }

    /// <summary>
    /// Gets the display name associated with this <see cref="EmailAddress"/>.
    /// </summary>
    public string? DisplayName { get; }

    /// <summary>
    /// Gets the user information from the address specified when this instance was created.
    /// </summary>
    public string? User { get; }

    /// <summary>
    /// Gets the host portion of the address specified when this instance was created.
    /// </summary>
    public string? Host { get; }

    /// <summary>
    /// Parses a string into an <see cref="EmailAddress"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>
    /// The result of parsing <paramref name="s"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">When <paramref name="s"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">When <paramref name="s"/> is empty or on contains whitespaces.</exception>
    /// <exception cref="FormatException">When <paramref name="s"/> is not a valid email address.</exception>
    public static EmailAddress Parse(string? s)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s);
        return TryParse(s, out var result) ? result : throw new FormatException();
    }

    /// <inheritdoc />
    public override string ToString() => _toString ?? string.Empty;

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (!IsValid)
        {
            return 0;
        }

        var hc = new HashCode();
        hc.Add(Address, StringComparer.OrdinalIgnoreCase);
        hc.Add(DisplayName, StringComparer.Ordinal);

        return hc.ToHashCode();
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj switch
        {
            EmailAddress other => Equals(other),
            string str when TryParse(str, out var email) => Equals(email),
            _ => false
        };

    /// <inheritdoc />
    public bool Equals(EmailAddress other) =>
        !IsValid
            ? !other.IsValid
            : other.IsValid &&
              other.Address.OrdinalEquals(Address, true) &&
              other.DisplayName.OrdinalEquals(DisplayName);

    /// <summary>
    /// Determines whether the addresses of two <see cref="EmailAddress"/> instances are equal,
    /// regardless of their display names.
    /// </summary>
    /// <param name="other">The <see cref="EmailAddress"/> instance to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the current instance and the specified instance have equal addresses; otherwise, <c>false</c>.
    /// </returns>
    public bool AddressEquals(EmailAddress other) =>
        !IsValid ? !other.IsValid : other.IsValid && other.Address.OrdinalEquals(Address, true);

    /// <summary>
    /// Creates a new <see cref="EmailAddress"/> instance with the specified display name while
    /// retaining other properties.
    /// </summary>
    /// <param name="displayName">The display name to associate with the email address.</param>
    /// <returns>
    /// A new <see cref="EmailAddress"/> instance with the updated display name.
    /// </returns>
    public EmailAddress WithDisplayName(string? displayName)
    {
        if (!IsValid)
        {
            throw new InvalidOperationException("Cannot change the display name of an invalid email address.");
        }

        displayName = displayName.IfNullOrWhiteSpace(null);
        return DisplayName.OrdinalEquals(displayName)
            ? this
            : new EmailAddress(Address, displayName, User, Host);
    }

    internal static string GetStringRepresentation(string address, string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return address;
        }

        if (displayName.OrdinalContains('"'))
        {
            displayName = displayName.JsonEscape();
        }

        return $"\"{displayName}\" <{address}>";
    }

    /// <summary>
    /// Determines whether the provided <see cref="EmailAddress"/> instance is valid.
    /// </summary>
    /// <param name="emailAddress">The <see cref="EmailAddress"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="EmailAddress"/> instance is valid; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static implicit operator bool(EmailAddress emailAddress) => emailAddress.IsValid;

    /// <summary>
    /// Determines whether two <see cref="EmailAddress"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="EmailAddress"/> to compare.</param>
    /// <param name="right">The second <see cref="EmailAddress"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="EmailAddress"/> instances are equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator ==(EmailAddress left, EmailAddress right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="EmailAddress"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="EmailAddress"/> to compare.</param>
    /// <param name="right">The second <see cref="EmailAddress"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two <see cref="EmailAddress"/> instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator !=(EmailAddress left, EmailAddress right) => !(left == right);
}