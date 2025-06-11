using System.Net.Mail;
using System.Reflection;

namespace AnxiousAnt;

partial struct EmailAddress : IParsable<EmailAddress>
{
    private static readonly TryParseMailAddress? TryParseDelegate = typeof(MailAddress)
        .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
        .FirstOrDefault(x => x.Name == nameof(TryParse))?
        .CreateDelegate<TryParseMailAddress>();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    static EmailAddress IParsable<EmailAddress>.Parse(string? s, IFormatProvider? provider) => Parse(s);

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    static bool IParsable<EmailAddress>.TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out EmailAddress result) =>
        TryParse(s, out result);

    /// <summary>
    /// Tries to parse a string into an <see cref="EmailAddress"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing
    /// <paramref name="s" /> or an undefined value on failure.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="s" /> was successfully parsed; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse([NotNullWhen(true)] string? s, out EmailAddress result)
    {
        result = default;
        var span = s.AsSpan();
        if (span.IsWhiteSpace() || span.Length > MaxEmailLength)
        {
            return false;
        }

        var indexOfFirstAtSign = span.IndexOf('@');
        if (indexOfFirstAtSign == -1 ||
            !TryParse(s!, out var address, out var displayName, out var user, out var host) ||
            user is { Length: > MaxLocalPartLength } || host.AsSpan().ContainsAny(IllegalHostCharacters))
        {
            return false;
        }

        result = new EmailAddress(address, displayName, user, host);
        return true;
    }

    [ExcludeFromCodeCoverage(
        Justification = "This method uses reflection to conditionally call a private static method."
    )]
    private static bool TryParse(
        string s,
        [NotNullWhen(true)] out string? address,
        out string? displayName,
        out string? user,
        out string? host)
    {
        address = displayName = user = host = null;
        if (TryParseDelegate is null)
        {
            if (!MailAddress.TryCreate(s, out var mailAddress))
            {
                return false;
            }

            address = mailAddress.Address.AsLowerInvariant();
            displayName = mailAddress.DisplayName;
            user = mailAddress.User;
            host = mailAddress.Host;
            return true;
        }

        if (!TryParseDelegate(s, null, null, out var parsedData, false))
        {
            return false;
        }

        address = $"{parsedData.user}@{parsedData.host}".AsLowerInvariant();
        displayName = parsedData.displayName;
        user = parsedData.user;
        host = parsedData.host;
        return true;
    }

    private delegate bool TryParseMailAddress(
        string? s,
        string? displayName,
        Encoding? encoding,
        out (string displayName, string user, string host, Encoding displayNameEncoding) parsedData,
        bool throwExceptionIfFail);
}