using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

/// <summary>
/// Provides factory methods and utilities for creating and inspecting result instances.
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
    /// Retrieves the underlying generic type of a given constructed generic type of <see cref="Result{T}"/>,
    /// if applicable.
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
}