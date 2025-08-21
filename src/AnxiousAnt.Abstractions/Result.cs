using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

/// <summary>
/// Provides factory methods and utilities for creating and inspecting <see cref="Result{T}"/> instances.
/// </summary>
public static class Result
{
    private static readonly Type ResultTypeDefinition = typeof(Result<>);

    /// <summary>
    /// Creates a new successful result containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored in the result.</typeparam>
    /// <param name="value">The value to be encapsulated in the result.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance encapsulating the specified value.
    /// </returns>
    public static Result<T> FromValue<T>(T? value) where T : notnull =>
        new(value);

    /// <summary>
    /// Creates a new failed result encapsulating the specified exception.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the result.</typeparam>
    /// <param name="e">The exception to be encapsulated in the result. This cannot be <c>null</c>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance encapsulating the specified exception, indicating a failure.
    /// </returns>
    [StackTraceHidden]
    public static Result<T> FromException<T>(Exception e) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(e);
        return new Result<T>(ExceptionDispatchInfo.Capture(e));
    }

    /// <summary>
    /// Creates a new failed result encapsulating the specified exception dispatch information.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the result.</typeparam>
    /// <param name="exceptionDispatchInfo">The exception dispatch information to be encapsulated in the result.
    /// This cannot be <c>null</c>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance encapsulating the specified exception dispatch information, indicating a failure.
    /// </returns>
    [StackTraceHidden]
    public static Result<T> FromException<T>(ExceptionDispatchInfo exceptionDispatchInfo)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(exceptionDispatchInfo);
        return new Result<T>(exceptionDispatchInfo);
    }

    /// <summary>
    /// Determines whether the specified type is a constructed generic type of <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="resultType">The type to be checked. This cannot be <c>null</c>.</param>
    /// <returns>
    /// A boolean indicating whether the specified type is a constructed generic type of <see cref="Result{T}"/>.
    /// </returns>
    public static bool IsResult(Type resultType)
    {
        ArgumentNullException.ThrowIfNull(resultType);
        return resultType.IsConstructedGenericType &&
               ReferenceEquals(resultType.GetGenericTypeDefinition(), ResultTypeDefinition);
    }

    /// <summary>
    /// Retrieves the underlying generic type of the given constructed <see cref="Result{T}"/>,  if applicable.
    /// </summary>
    /// <param name="resultType">The type to analyze. This cannot be <c>null</c>.</param>
    /// <returns>
    /// The underlying generic type of the specified <see cref="Result{T}"/> if it is a constructed generic
    /// type of <see cref="Result{T}"/>, otherwise <c>null</c>.
    /// </returns>
    public static Type? GetUnderlyingType(Type resultType)
    {
        ArgumentNullException.ThrowIfNull(resultType);
        return IsResult(resultType) ? resultType.GenericTypeArguments[0] : null;
    }

    /// <summary>
    /// Executes a specified function and returns a result encapsulating the outcome.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the result.</typeparam>
    /// <param name="factory">The function to execute, which produces the value to encapsulate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance containing the value returned by the function
    /// if successful, or encapsulating the exception thrown during execution.
    /// </returns>
    public static Result<T> Try<T>(Func<T> factory) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);

        try
        {
            var result = factory();
            return FromValue(result);
        }
        catch (Exception e)
        {
            return FromException<T>(e);
        }
    }

    /// <summary>
    /// Asynchronously executes a function and encapsulates the result or exception into a <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the result.</typeparam>
    /// <param name="factory">The asynchronous function to be executed.</param>
    /// <returns>
    /// A task producing a <see cref="Result{T}"/> which represents the result of the operation.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static ValueTask<Result<T>> TryAsync<T>(Func<Task<T>> factory) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        return TryAsync<T>(_ => factory(), CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously executes a function and encapsulates the result or exception into a <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the result.</typeparam>
    /// <param name="factory">The asynchronous function to be executed.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to
    /// complete.</param>
    /// <returns>
    /// A task producing a <see cref="Result{T}"/> which represents the result of the operation.
    /// </returns>
    public static ValueTask<Result<T>> TryAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromResult(
                FromException<T>(new OperationCanceledException(cancellationToken))
            );
        }

        try
        {
            var task = factory(cancellationToken);
            return FromTaskAsync(task);
        }
        catch (Exception e)
        {
            return ValueTask.FromResult(FromException<T>(e));
        }
    }

    /// <summary>
    /// Creates a result from the asynchronous task, encapsulating either the successful result or
    /// any exception that occurred.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by the task.</typeparam>
    /// <param name="task">The asynchronous task to wrap in a result.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance encapsulating the task's result or an error if the task failed.
    /// </returns>
    public static async ValueTask<Result<T>> FromTaskAsync<T>(Task<T> task)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(task);

        try
        {
            if (!task.IsCompleted)
            {
                // Only start the async state machine if the task hasn't been completed
                await task.ConfigureAwait(false);
            }

            return task.Exception is null
                ? FromValue(task.Result)
                : FromException<T>(task.Exception.GetBaseException());
        }
        catch (AggregateException e)
        {
            return FromException<T>(e.GetBaseException());
        }
        catch (Exception e)
        {
            return FromException<T>(e);
        }
    }
}