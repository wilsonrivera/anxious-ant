namespace AnxiousAnt;

/// <summary>
/// Provides factory methods and utilities for creating and inspecting <see cref="Optional{T}"/> instances.
/// </summary>
public static class Optional
{
    internal const int UninitializedValueKind = 0;
    internal const int EmptyValueKind = 1;
    internal const int NullValueKind = 2;
    internal const int NotEmptyValueKind = 3;

    internal static readonly Type OptionalTypeDefinition = typeof(Optional<>);

    /// <summary>
    /// Creates an instance of <see cref="Optional{T}"/> with no value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored in the optional.</typeparam>
    /// <returns>
    /// An instance of <see cref="Optional{T}"/> representing a value that is absent.
    /// </returns>
    public static Optional<T> None<T>() where T : notnull => Optional<T>.None;

    /// <summary>
    /// Creates an instance of <see cref="Optional{T}"/> that contains the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored in the optional.</typeparam>
    /// <param name="value">The value to be wrapped in an <see cref="Optional{T}"/>. Cannot be <c>null</c>.</param>
    /// <returns>
    /// An instance of <see cref="Optional{T}"/> containing the provided value.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided value is <c>null</c>.</exception>
    public static Optional<T> Of<T>(T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Optional<T>(value);
    }

    /// <summary>
    /// Creates an instance of <see cref="Optional{T}"/> from a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored in the optional.</typeparam>
    /// <param name="value">The nullable value to create the optional from.</param>
    /// <returns>
    /// An instance of <see cref="Optional{T}"/> containing the provided value if it is non-null, or an empty
    /// <see cref="Optional{T}"/> if the value is <c>null</c>.
    /// </returns>
    public static Optional<T> OfNullable<T>(T? value) where T : notnull =>
        value is null ? new Optional<T>(default) : Of(value);

    /// <summary>
    /// Determines whether the specified type is a constructed generic type of <see cref="Optional{T}"/>.
    /// </summary>
    /// <param name="optionalType">The type to be checked. This cannot be <c>null</c>.</param>
    /// <returns>
    /// A boolean indicating whether the specified type is a constructed generic type of <see cref="Optional{T}"/>.
    /// </returns>
    public static bool IsOptional(Type optionalType)
    {
        ArgumentNullException.ThrowIfNull(optionalType);
        return optionalType.IsConstructedGenericType &&
               ReferenceEquals(optionalType.GetGenericTypeDefinition(), OptionalTypeDefinition);
    }

    /// <summary>
    /// Retrieves the underlying generic type of the given constructed <see cref="Optional{T}"/>,  if applicable.
    /// </summary>
    /// <param name="optionalType">The type to analyze. This cannot be <c>null</c>.</param>
    /// <returns>
    /// The underlying generic type of the specified <see cref="Optional{T}"/> if it is a constructed generic
    /// type of <see cref="Optional{T}"/>, otherwise <c>null</c>.
    /// </returns>
    public static Type? GetUnderlyingType(Type optionalType)
    {
        ArgumentNullException.ThrowIfNull(optionalType);
        return IsOptional(optionalType) ? optionalType.GenericTypeArguments[0] : null;
    }

    public static Optional<T> Try<T>(Func<T> action) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            var result = action();
            return OfNullable(result);
        }
        catch
        {
            // ignore
        }

        return Optional<T>.None;
    }

    /// <summary>
    /// Attempts to asynchronously execute the provided factory function, which produces a result of
    /// type <see cref="Optional{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="factory">A factory function that produces a task to compute the value.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing an <see cref="Optional{T}"/> that either contains the computed
    /// value or represents an absence of value if an exception occurs or if the operation is canceled.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static ValueTask<Optional<T>> TryAsync<T>(Func<Task<T>> factory) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return TryAsync<T>(_ => factory(), CancellationToken.None);
    }

    /// <summary>
    /// Attempts to asynchronously execute the provided factory function, which produces a result of
    /// type <see cref="Optional{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="factory">A factory function that produces a task to compute the value.</param>
    /// <param name="cancellationToken">A token to observe cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing an <see cref="Optional{T}"/> that either contains the computed
    /// value or represents an absence of value if an exception occurs or if the operation is canceled.
    /// </returns>
    public static async ValueTask<Optional<T>> TryAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (cancellationToken.IsCancellationRequested)
        {
            return Optional<T>.None;
        }

        try
        {
            var result = factory(cancellationToken);
            return await FromTaskAsync(result).ConfigureAwait(false);
        }
        catch
        {
            // ignore
        }

        return Optional<T>.None;
    }

    /// <summary>
    /// Creates an instance of <see cref="Optional{T}"/> from a given task, handling any exceptions or <c>null</c>
    /// results encountered during the task's execution.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="task">The task representing the asynchronous operation to retrieve the value.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing an <see cref="Optional{T}"/> instance.
    /// If the task completes successfully, the result is wrapped in an optional.
    /// If the task fails or returns null, an empty optional is returned.
    /// </returns>
    public static async ValueTask<Optional<T>> FromTaskAsync<T>(Task<T> task) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(task);

        try
        {
            if (!task.IsCompleted)
            {
                // Only start the async state machine if the task hasn't been completed
                await task.ConfigureAwait(false);
            }

            return task.Exception is null ? OfNullable(task.Result) : None<T>();
        }
        catch
        {
            // ignore
        }

        return Optional<T>.None;
    }
}