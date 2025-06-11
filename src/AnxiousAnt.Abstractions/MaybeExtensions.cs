namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for the <see cref="Maybe{TValue}"/> structure.
/// </summary>
public static class MaybeExtensions
{
    /// <summary>
    /// Attempts to retrieve the value of the <see cref="Maybe{T}"/> structure.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="Maybe{T}"/>.</typeparam>
    /// <param name="maybe">The <see cref="Maybe{T}"/> structure to retrieve the value from.</param>
    /// <param name="value">When this method returns, contains the value if the <see cref="Maybe{T}"/> has one;
    /// otherwise, the default value for the type.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Maybe{T}"/> has a value; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool TryGetValue<T>(this Maybe<T> maybe, [MaybeNullWhen(false)] out T value)
    {
        if (!maybe.HasValue)
        {
            value = default;
            return false;
        }

        value = maybe.Value;
        return true;
    }

    /// <summary>
    /// Retrieves the value of the <see cref="Maybe{TValue}"/> structure if it has one;
    /// otherwise, returns the default value for the type.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the <see cref="Maybe{TValue}"/>.</typeparam>
    /// <param name="maybe">The <see cref="Maybe{TValue}"/> structure to retrieve the value from.</param>
    /// <returns>
    /// The value if the <see cref="Maybe{TValue}"/> has one; otherwise, the default value
    /// of <typeparamref name="TValue"/>.
    /// </returns>
    [Pure]
    public static TValue GetValueOrDefault<TValue>(this Maybe<TValue> maybe) =>
        maybe.HasValue ? maybe.Value : default!;

    /// <summary>
    /// Retrieves the value of the <see cref="Maybe{TValue}"/> structure if it has a value, otherwise the
    /// provided <paramref name="defaultValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the <see cref="Maybe{TValue}"/>.</typeparam>
    /// <param name="maybe">The <see cref="Maybe{TValue}"/> structure from which to retrieve the value.</param>
    /// <param name="defaultValue">The value to return if the <see cref="Maybe{TValue}.HasValue"/> property
    /// is <see langword="false"/>.</param>
    /// <returns>
    /// The value of the <see cref="Maybe{TValue}"/> if it has one, otherwise the default value for the type.
    /// </returns>
    [Pure]
    public static TValue GetValueOrDefault<TValue>(this Maybe<TValue> maybe, TValue defaultValue) =>
        maybe.HasValue ? maybe.Value : defaultValue;

    /// <summary>
    /// Disposes the value inside the <see cref="Maybe{T}"/> structure if it is disposable.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="Maybe{T}"/>, which must
    /// implement <see cref="IDisposable"/>.</typeparam>
    /// <param name="maybe">The <see cref="Maybe{T}"/> structure containing the disposable value.</param>
    public static void Dispose<T>(this Maybe<T> maybe) where T : IDisposable
    {
        if (!TryGetValue(maybe, out var disposable))
        {
            return;
        }

        disposable.Dispose();
    }

    /// <summary>
    /// Asynchronously disposes the value inside the <see cref="Maybe{T}"/> structure if it
    /// is asynchronously disposable.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the <see cref="Maybe{T}"/>, which must
    /// implement <see cref="IAsyncDisposable"/>.</typeparam>
    /// <param name="maybe">The <see cref="Maybe{T}"/> structure containing the asynchronously disposable value.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous disposal operation.
    /// </returns>
    public static ValueTask DisposeAsync<T>(this Maybe<T> maybe) where T : IAsyncDisposable
    {
        return !TryGetValue(maybe, out var asyncDisposable)
            ? ValueTask.CompletedTask
            : asyncDisposable.DisposeAsync();
    }
}