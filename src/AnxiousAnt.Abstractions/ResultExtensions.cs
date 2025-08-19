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
    public static T? GetValueOrNull<T>(this in Result<T> result) where T : struct =>
        result.TryGetValue(out var value) ? value : null;

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
    /// Asynchronously transforms the result into another value based on the success or failure state.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <typeparam name="TOutput">The type of the transformed result.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/>.</param>
    /// <param name="onSuccess">A function to invoke if the result is successful, which returns a task that
    /// produces a transformed value.</param>
    /// <param name="onFailure">A function to invoke if the result is a failure, which returns a task that
    /// produces a transformed value.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is the transformed value.
    /// </returns>
    [Pure]
    public static Task<TOutput> FoldAsync<T, TOutput>(
        in this Result<T> result,
        Func<T?, Task<TOutput>> onSuccess,
        Func<Exception, Task<TOutput>> onFailure)
        where T : notnull
        where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return result.FoldAsync(
            (v, _) => onSuccess(v),
            (e, _) => onFailure(e),
            CancellationToken.None
        );
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
    [Pure]
    public static ValueTask<Result<TOutput>> MapAsync<T, TOutput>(
        in this Result<T> result,
        Func<T?, Task<TOutput?>> transform)
        where T : notnull
        where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(transform);
        return result.MapAsync((value, _) => transform(value), CancellationToken.None);
    }
}