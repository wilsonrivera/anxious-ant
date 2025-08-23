namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for the <see cref="Optional{T}"/> struct.
/// </summary>
public static class OptionalExtensions
{
    /// <summary>
    /// Retrieves the value of an <see cref="Optional{T}"/> as a nullable type if it is not empty;
    /// otherwise, returns <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="optional">The <see cref="Optional{T}"/>.</param>
    /// <returns>
    /// The value of the <see cref="Optional{T}"/> as a nullable type if it contains a value; otherwise, <c>null</c>.
    /// </returns>
    [Pure]
    public static T? ValueOrNull<T>(this in Optional<T> optional) where T : struct =>
        optional.TryGetValue(out var value) ? value : null;

    /// <summary>
    /// Flattens a nested <see cref="Optional{T}"/> structure.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="optional">The nested <see cref="Optional{T}"/> to flatten.</param>
    /// <returns>
    /// The flatten <see cref="Optional{T}"/>.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static Optional<T> Flatten<T>(this in Optional<Optional<T>> optional) where T : notnull =>
        new(in optional);

    /// <summary>
    /// Converts a task returning a value of type <typeparamref name="T"/> into a wrapped <see cref="Optional{T}"/>
    /// asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="task">The task to convert to a <see cref="Optional{T}"/>.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing an <see cref="Optional{T}"/> with the result of
    /// the original task.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static Task<Optional<T>> ToOptionalAsync<T>(this Task<T> task) where T : notnull =>
        Optional.FromTaskAsync(task).AsTask();

    /// <summary>
    /// Converts the value of the specified <see cref="Optional{T}"/> containing a <see cref="string"/>
    /// to a <see cref="ReadOnlySpan{T}"/> of characters, or returns an empty span if the instance is default or empty.
    /// </summary>
    /// <param name="optional">The <see cref="Optional{T}"/>.</param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}"/> of characters representing the string value if the <see cref="Optional{T}"/>
    /// is not default or empty; otherwise, an empty span.
    /// </returns>
    [Pure]
    public static ReadOnlySpan<char> AsSpan(this in Optional<string> optional) =>
        optional.IsDefaultOrEmpty ? ReadOnlySpan<char>.Empty : optional.ValueRef.AsSpan();

    /// <summary>
    /// Computes the hash code of the value contained in an <see cref="Optional{T}"/>
    /// using the specified comparison criteria, or returns the value kind if the optional is empty.
    /// </summary>
    /// <param name="optional">The <see cref="Optional{T}"/>.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// An <see cref="int"/> representing the hash code of the string value if the optional is not empty;
    /// otherwise, the value kind of the optional.
    /// </returns>
    [Pure]
    public static int GetHashCode(this in Optional<string> optional, StringComparison comparisonType)
        => optional.ValueKind switch
        {
            Optional.NotEmptyValueKind => string.GetHashCode(optional.ValueRef.AsSpan(), comparisonType),
            _ => optional.ValueKind,
        };

    /// <summary>
    /// Returns the current <see cref="Optional{T}"/> instance if it has a value; otherwise, asynchronously invokes
    /// the provided factory function to produce an alternative value.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="factory">A function that asynchronously produces an alternative value of type
    /// <typeparamref name="T"/> if the current instance is uninitialized or empty.</param>
    /// <param name="optional">The <see cref="Optional{T}"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> representing the result of the operation: the current instance if it has a value,
    /// or a new instance containing the value returned by the factory function.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="factory"/> function is <c>null</c>.</exception>
    [Pure, ExcludeFromCodeCoverage]
    public static ValueTask<Optional<T>> OrAsync<T>(this in Optional<T> optional, Func<Task<T?>> factory)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return optional.OrAsync(_ => factory());
    }

    /// <summary>
    /// Returns the current instance if it contains a value, or asynchronously evaluates the provided factory function
    /// to produce an alternative value.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="factory">A function that produces an alternative <see cref="Optional{T}"/> asynchronously when the
    /// current instance is empty or uninitialized.</param>
    /// <param name="optional">The <see cref="Optional{T}"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> that represents the result of the operation. It is the current instance if
    /// it contains a value, or the result of the factory function if the current instance is empty or uninitialized.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static ValueTask<Optional<T>> OrElseAsync<T>(this in Optional<T> optional, Func<Task<Optional<T>>> factory)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return optional.OrElseAsync(_ => factory());
    }
}