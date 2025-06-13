using System.Security.Claims;

namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for working with <see cref="ClaimsPrincipal"/> objects.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Attempts to find a claim of the specified type in the <see cref="ClaimsPrincipal"/> and retrieve its value
    /// as a string.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> containing the claims to search.</param>
    /// <param name="type">The claim type to search for.</param>
    /// <param name="result">When this method returns, contains the value of the claim if found; otherwise, <c>null</c>.</param>
    /// <returns>
    /// Returns <c>true</c> if a claim of the specified type is found and contains a non-null, non-whitespace value;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool TryFindClaim(
        this ClaimsPrincipal? principal,
        string? type,
        [MaybeNullWhen(false)] out string result)
    {
        result = null;
        if (principal is null || string.IsNullOrWhiteSpace(type) || principal.FindFirst(type) is not { } claim ||
            string.IsNullOrWhiteSpace(claim.Value))
        {
            return false;
        }

        result = claim.Value;
        return true;
    }

    /// <summary>
    /// Attempts to find a claim of the specified type in the <see cref="ClaimsPrincipal"/>
    /// and parse its value into the specified type.
    /// </summary>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> containing the claims to search.</param>
    /// <param name="type">The claim type to search for.</param>
    /// <param name="result">When this method returns, contains the parsed value of the claim if found and
    /// successfully parsed; otherwise, the default value of the specified type.</param>
    /// <typeparam name="T">The type to parse the claim value into.</typeparam>
    /// <returns>
    /// Returns <c>true</c> if a claim of the specified type is found and successfully parsed; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryFindClaim<T>(
        this ClaimsPrincipal? principal,
        string? type,
        [MaybeNullWhen(false)] out T result)
        where T : IParsable<T>
    {
        result = default;
        return TryFindClaim(principal, type, out var claimValue) && T.TryParse(claimValue, null, out result);
    }
}