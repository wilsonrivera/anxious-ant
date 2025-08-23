namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for the <see cref="Result{T}"/> struct.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts the current <see cref="Result{T}"/> instance to a nullable value type. If the
    /// <see cref="Result{T}.HasValue"/> property is <c>true</c>, the method returns the value; otherwise,
    /// it returns <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <returns>
    /// A nullable value of type <typeparamref name="T"/> that represents the result's value, or <c>null</c> if the
    /// <see cref="Result{T}.HasValue"/> property is <c>false</c>.
    /// </returns>
    [Pure]
    public static T? ValueOrNull<T>(this in Result<T> result) where T : struct =>
        result.TryGetValue(out var value) ? value : null;

    /// <summary>
    /// Flattens a nested <see cref="Result{T}"/> structure.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The nested <see cref="Result{T}"/> to flatten.</param>
    /// <returns>
    /// The flatten <see cref="Result{T}"/>.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static Result<T> Flatten<T>(this in Result<Result<T>> result) where T : notnull =>
        new(in result);

    /// <summary>
    /// Converts a task returning a value of type <typeparamref name="T"/> into a wrapped <see cref="Result{T}"/>
    /// asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="task">The task to convert to a <see cref="Result{T}"/>.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="Result{T}"/> with the result of
    /// the original task.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static Task<Result<T>> ToResultAsync<T>(this Task<T> task) where T : notnull =>
        Result.FromTaskAsync(task).AsTask();

    /// <summary>
    /// Returns a reference to the value of the input <see cref="Result{T}"/> instance, regardless of whether the
    /// <see cref="Result{T}.HasValue"/> property is returning <c>true</c> or not. If that is not the case, this
    /// method will still return a reference to the underlying <see langword="default"/> value.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <returns>
    /// A reference to the underlying value from the input <see cref="Result{T}"/> instance.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static ref T DangerousGetValueOrDefaultReference<T>(this ref Result<T> result)
        where T : struct
    {
        return ref Unsafe.AsRef(in result.ValueRef);
    }

    /// <summary>
    /// Returns a reference to the value of the input <see cref="Result{T}"/> instance, or a <c>null</c>
    /// <typeparamref name="T"/> reference.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <returns>
    /// A reference to the underlying value from the input <see cref="Result{T}"/> instance.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static unsafe ref T DangerousGetValueOrNullReference<T>(ref readonly this Result<T> result)
        where T : struct
    {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        ref T resultRef = ref *(T*)null;
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        if (result.HasValue)
        {
            resultRef = ref Unsafe.AsRef(in result.ValueRef);
        }

        return ref resultRef;
    }

    /// <summary>
    /// Returns a new <see cref="Result{T}"/> instance with a specified default value if the current result is
    /// a failure.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="factory">A function to invoke if the result is a failure. It provides the exception from the
    /// failure and returns a value to be used.</param>
    /// <returns>
    /// A new <see cref="Result{T}"/> instance with the value returned by the <paramref name="factory"/> function
    /// if the result is a failure, otherwise the current result.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static Result<T> Or<T>(this in Result<T> result, Func<T?> factory) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return result.Or(_ => factory());
    }

    /// <summary>
    /// Returns the current successful result if it exists; otherwise, invokes the specified factory
    /// function to produce a new result.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="factory">A delegate that is invoked to produce a new <see cref="Result{T}"/> if the current
    /// result is not successful.</param>
    /// <returns>
    /// A new result that is either the current successful result or the result produced by the factory function.
    /// If an exception occurs while invoking the factory, a failure result containing the exception is returned.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static ValueTask<Result<T>> OrAsync<T>(this in Result<T> result, Func<Exception, Task<T?>> factory)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return result.OrAsync((error, _) => factory(error));
    }

    /// <summary>
    /// Returns the current <see cref="Result{T}"/> if it is successful; otherwise, invokes the specified factory and
    /// returns the result produced by the factory.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="factory">A delegate that is invoked to produce a new <see cref="Result{T}"/> if the current
    /// result is not successful.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> that contains either the current value or the value produced by the factory.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the factory is <c>null</c>.</exception>
    [Pure, ExcludeFromCodeCoverage]
    public static Result<T> OrElse<T>(this in Result<T> result, Func<Result<T>> factory) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return result.OrElse(_ => factory());
    }

    /// <summary>
    /// Asynchronously returns the current <see cref="Result{T}"/> if it is successful; otherwise, invokes the
    /// specified factory and returns the result produced by the factory.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="factory">A delegate that is invoked to produce a new <see cref="Result{T}"/> if the current
    /// result is not successful.</param>
    /// <returns>
    /// A task that resolves to the current result if it is successful, or the result returned by the factory
    /// function in case of a failure.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static ValueTask<Result<T>> OrElseAsync<T>(this in Result<T> result, Func<Task<Result<T>>> factory)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return OrElseAsync(in result, _ => factory());
    }

    /// <summary>
    /// Asynchronously returns the current <see cref="Result{T}"/> if it is successful; otherwise, invokes the
    /// specified factory and returns the result produced by the factory.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="factory">A delegate that is invoked to produce a new <see cref="Result{T}"/> if the current
    /// result is not successful.</param>
    /// <returns>
    /// A task that resolves to the current result if it is successful, or the result returned by the factory
    /// function in case of a failure.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public static ValueTask<Result<T>> OrElseAsync<T>(
        this in Result<T> result,
        Func<Exception, Task<Result<T>>> factory)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return result.OrElseAsync((error, _) => factory(error));
    }

    /// <summary>
    /// Asynchronously maps the current <see cref="Result{TValue}"/> to a <see cref="Result{TOutput}"/> using
    /// the specified mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <typeparam name="TOutput">The type of the result after mapping.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="transform">The mapping function to be applied to the <typeparamref name="T"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the mapped
    /// <see cref="Result{TOutput}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="transform"/> is <c>null</c>.</exception>
    [Pure, ExcludeFromCodeCoverage]
    public static ValueTask<Result<TOutput>> MapAsync<T, TOutput>(
        this in Result<T> result,
        Func<T?, Task<TOutput?>> transform)
        where T : notnull
        where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(transform);
        return result.MapAsync((value, _) => transform(value), CancellationToken.None);
    }
}