using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

/// <summary>
/// Represents the outcome of an operation that can either be a successful result with a value
/// of type <typeparamref name="TValue"/> or a failure with an associated exception.
/// </summary>
/// <typeparam name="TValue">The type of the value in a successful result.</typeparam>
[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
public readonly struct Result<TValue> : IEquatable<Result<TValue>>
{
    private readonly bool _initialized;

    private Result(TValue? value) : this()
    {
        _initialized = true;
        Value = value;
        HasValue = value is not null;
    }

    private Result(ExceptionDispatchInfo exceptionDispatchInfo) : this()
    {
        _initialized = true;
        ExceptionDispatchInfo = exceptionDispatchInfo;
    }

    /// <summary>
    /// Indicates whether the result is a default, uninitialized instance.
    /// </summary>
    [Pure]
    public bool IsDefault => !_initialized;

    /// <summary>
    /// Indicates whether the result is in a faulted state.
    /// </summary>
    [Pure]
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool IsFaulted => ExceptionDispatchInfo is not null;

    /// <summary>
    /// Indicates whether the operation resulted in a success.
    /// </summary>
    [Pure]
    public bool IsSuccess => ExceptionDispatchInfo is null;

    /// <summary>
    /// Gets a value indicating whether the operation produced a non-null result.
    /// </summary>
    [Pure]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    /// <summary>
    /// Indicates whether the operation is successful and produced a non-null value.
    /// </summary>
    [Pure]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccessAndHasValue => IsSuccess && HasValue;

    /// <summary>
    /// Gets the value stored in this instance or if the value was not set.
    /// </summary>
    [Pure]
    public TValue? Value { get; }

    /// <summary>
    /// Gets the exception that occurred during the operation, if any.
    /// </summary>
    [Pure]
    public Exception? Exception => ExceptionDispatchInfo?.SourceException;

    /// <summary>
    /// Gets the <see cref="ExceptionDispatchInfo"/> associated with the exception, if any.
    /// </summary>
    [Pure]
    private ExceptionDispatchInfo? ExceptionDispatchInfo { get; }

    /// <summary>
    /// Returns a <see cref="Result{TValue}"/> with the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>An instance of <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> FromValue(TValue? value) => new(value);

    /// <summary>
    /// Returns a <see cref="Result{TValue}"/> with the given <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>
    /// An instance of <see cref="Result{TValue}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception"/> is <c>null</c>.</exception>
    public static Result<TValue> FromException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return new Result<TValue>(ExceptionDispatchInfo.Capture(exception));
    }

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> instance representing a faulted result from an exception.
    /// </summary>
    /// <param name="exceptionDispatchInfo">The exception to encapsulate in the result.</param>
    /// <returns>
    /// An instance of <see cref="Result{TValue}"/> representing a faulted state.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exceptionDispatchInfo"/> is <c>null</c>.</exception>
    public static Result<TValue> FromException(ExceptionDispatchInfo exceptionDispatchInfo)
    {
        ArgumentNullException.ThrowIfNull(exceptionDispatchInfo);
        return new Result<TValue>(exceptionDispatchInfo);
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString()
    {
        if (IsFaulted)
        {
            return ExceptionDispatchInfo?.SourceException.ToString() ?? string.Empty;
        }

        TValue? value = Value;
        ref TValue? local = ref value;
        return (local is not null ? local.ToString() : null) ?? string.Empty;
    }

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode() =>
        HashCode.Combine(IsFaulted, ExceptionDispatchInfo, Value);

    /// <inheritdoc />
    [Pure]
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Result<TValue> other && Equals(other);

    /// <inheritdoc />
    [Pure]
    public bool Equals(Result<TValue> other)
    {
        if (IsFaulted)
        {
            return other.IsFaulted &&
                   (ReferenceEquals(Exception, other.Exception) || Exception.Equals(other.Exception));
        }

        return !other.IsFaulted &&
               (
                   (ReferenceEquals(null, Value) && ReferenceEquals(null, other.Value)) ||
                   (
                       !ReferenceEquals(null, Value) &&
                       !ReferenceEquals(null, other.Value) &&
                       EqualityComparer<TValue>.Default.Equals(Value, other.Value)
                   )
               );
    }

    /// <summary>
    /// Tries to get the value, if available.
    /// </summary>
    /// <param name="result">Output parameter for the result.</param>
    /// <returns>
    /// <see langword="true"/> if the result is available; <see langword="false"/> otherwise.
    /// </returns>
    [Pure]
    public bool TryGetValue([NotNullWhen(true)] out TValue? result)
    {
        if (HasValue)
        {
            result = Value!;
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Retrieves the value stored in this instance, or rethrows the stored exception if one exists.
    /// </summary>
    /// <returns>
    /// The stored value of type <typeparamref name="TValue"/>.
    /// </returns>
    [Pure]
    public TValue? GetValueOrRethrow()
    {
        ExceptionDispatchInfo?.Throw();
        return Value;
    }

    /// <summary>
    /// Retrieves the value stored in this instance if it is not in a faulted state; otherwise, returns
    /// the specified <paramref name="defaultValue"/>.
    /// </summary>
    /// <param name="defaultValue">The value to return if the instance is in a faulted state.</param>
    /// <returns>
    /// The stored value of type <typeparamref name="TValue"/> if the instance is not faulted;
    /// otherwise, the provided <paramref name="defaultValue"/>.
    /// </returns>
    [Pure]
    public TValue IfFaulted(TValue defaultValue) => !IsFaulted ? Value! : defaultValue;

    /// <summary>
    /// Retrieves the value stored in this instance if it is not in a faulted state;
    /// otherwise, invokes the specified <paramref name="onFault"/> function with the faulted exception
    /// and returns its result.
    /// </summary>
    /// <param name="onFault">A function that takes the faulted <see cref="Exception"/> as a parameter and returns
    /// a value of type <typeparamref name="TValue"/>.</param>
    /// <returns>
    /// The stored value of type <typeparamref name="TValue"/> if the instance is not faulted;
    /// otherwise, the result of the <paramref name="onFault"/> function.
    /// </returns>
    [Pure]
    public TValue IfFaulted(Func<Exception, TValue> onFault)
    {
        ArgumentNullException.ThrowIfNull(onFault);

        return !IsFaulted ? Value! : onFault(Exception);
    }

    /// <summary>
    /// Matches the state of the instance and returns a result based on whether the operation was successful or faulted.
    /// </summary>
    /// <typeparam name="TOutput">The type of the result returned by the matching functions.</typeparam>
    /// <param name="onSuccess">A function that takes the stored value of type <typeparamref name="TValue"/> as a
    /// parameter and returns a result of type <typeparamref name="TOutput"/>.</param>
    /// <param name="onFailure">A function that takes the faulted <see cref="Exception"/> as a parameter and returns
    /// a result of type <typeparamref name="TOutput"/>.</param>
    /// <returns>
    /// The result of invoking the <paramref name="onSuccess"/> function if the instance is not faulted;
    /// otherwise, the result of invoking the <paramref name="onFailure"/> function.
    /// </returns>
    [Pure]
    public TOutput Match<TOutput>(Func<TValue, TOutput> onSuccess, Func<Exception, TOutput> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return !IsFaulted ? onSuccess(Value!) : onFailure(Exception!);
    }

    /// <summary>
    /// Maps the current result value to a new result value using the provided mapping function.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output result value.</typeparam>
    /// <param name="selector">The mapping function to apply to the result value.</param>
    /// <returns>
    /// A new <see cref="Result{TOutput}"/> instance containing the mapped value,
    /// or a new <see cref="Result{TOutput}"/> instance containing the exception if an error occurred.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="selector"/> is null.</exception>
    [Pure]
    public Result<TOutput> Map<TOutput>(Func<TValue, TOutput> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (ExceptionDispatchInfo is not null)
        {
            return new Result<TOutput>(ExceptionDispatchInfo);
        }

        try
        {
            return selector(Value!);
        }
        catch (Exception e)
        {
            return Result<TOutput>.FromException(e);
        }
    }

    /// <summary>
    /// Asynchronously maps the current <see cref="Result{TValue}"/> to a <see cref="Result{TOutput}"/> using
    /// the specified mapping function.
    /// </summary>
    /// <typeparam name="TOutput">The type of the result after mapping.</typeparam>
    /// <param name="selector">The mapping function to be applied to the <typeparamref name="TValue"/>.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the mapped
    /// <see cref="Result{TOutput}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="selector"/> is null.</exception>
    [Pure]
    public ValueTask<Result<TOutput>> MapAsync<TOutput>(Func<TValue, Task<TOutput>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return MapAsync<TOutput>((value, _) => selector(value));
    }

    /// <summary>
    /// Asynchronously maps the current <see cref="Result{TValue}"/> to a <see cref="Result{TOutput}"/> using
    /// the specified mapping function.
    /// </summary>
    /// <typeparam name="TOutput">The type of the result after mapping.</typeparam>
    /// <param name="selector">The mapping function to be applied to the <typeparamref name="TValue"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the mapped
    /// <see cref="Result{TOutput}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="selector"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the <paramref name="cancellationToken"/> is canceled.</exception>
    [Pure]
    public async ValueTask<Result<TOutput>> MapAsync<TOutput>(
        Func<TValue, CancellationToken, Task<TOutput>> selector,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(selector);

        if (ExceptionDispatchInfo is not null)
        {
            return new Result<TOutput>(ExceptionDispatchInfo);
        }

        try
        {
            return await selector(Value!, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return Result<TOutput>.FromException(e);
        }
    }

    /// <summary>
    /// Throws an exception if the operation produced an exception.
    /// </summary>
    /// <remarks>
    /// If the operation produced a result, this method does nothing. The thrown exception maintains its
    /// original stack trace.
    /// </remarks>
    public void ThrowIfException() => ExceptionDispatchInfo?.Throw();

    [ExcludeFromCodeCoverage]
    private string GetDebuggerDisplay()
    {
        if (IsFaulted)
        {
            return ExceptionDispatchInfo?.SourceException.ToString() ?? "(faulted)";
        }

        TValue? value = Value;
        ref TValue? local = ref value;
        return (local is not null ? local.ToString() : null) ?? "(null)";
    }

    /// <summary>
    /// Determines whether two specified <see cref="Result{TValue}"/> instances have the same value.
    /// </summary>
    /// <param name="left">The first <see cref="Result{TValue}"/> to compare.</param>
    /// <param name="right">The second <see cref="Result{TValue}"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the values of the two instances are equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator ==(in Result<TValue> left, in Result<TValue> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two specified <see cref="Result{TValue}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="Result{TValue}"/> to compare.</param>
    /// <param name="right">The second <see cref="Result{TValue}"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two specified <see cref="Result{TValue}"/> instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator !=(in Result<TValue> left, in Result<TValue> right) => !(left == right);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> to an instance of
    /// <see cref="Result{TValue}"/>. If the value is already a <see cref="Result{TValue}"/>,
    /// it is returned directly; otherwise, a new successful result containing the value is returned.
    /// </summary>
    /// <param name="value">The value to convert to a <see cref="Result{TValue}"/>.</param>
    /// <returns>
    /// An instance of <see cref="Result{TValue}"/> containing the given value.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static implicit operator Result<TValue>(TValue? value)
    {
        if (value is Result<TValue> result)
        {
            return result;
        }

        return FromValue(value);
    }

    /// <summary>
    /// Converts the specified <see cref="Result{TValue}"/> to a potentially nullable <typeparamref name="TValue"/>
    /// by extracting the value if successful, or rethrows the encapsulated exception if the result is faulted.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>
    /// The value contained within the result if it is a success; otherwise, rethrows the encapsulated exception.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static explicit operator TValue?(in Result<TValue> result) => result.GetValueOrRethrow();
}