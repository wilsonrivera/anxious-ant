using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace AnxiousAnt;

/// <summary>
/// Represents the result of an operation that either has a value of a specified type or an exception
/// indicating failure.
/// </summary>
/// <typeparam name="T">The type of the value stored in the result when the operation is successful.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T> where T : notnull
{
    private readonly bool _initialized;
    private readonly T? _value;
    private readonly ExceptionDispatchInfo? _exceptionDispatchInfo;

    /// <summary>
    /// Initializes a new successful result.
    /// </summary>
    /// <param name="value">The value to be stored as result.</param>
    public Result(T? value)
    {
        _initialized = true;
        _value = value;
    }

    /// <summary>
    /// Initializes a new unsuccessful result.
    /// </summary>
    /// <param name="exceptionDispatchInfo">The exception representing error. Cannot be <c>null</c>.</param>
    public Result(ExceptionDispatchInfo exceptionDispatchInfo)
    {
        ArgumentNullException.ThrowIfNull(exceptionDispatchInfo);

        Unsafe.SkipInit(out _value);
        _initialized = true;
        _exceptionDispatchInfo = exceptionDispatchInfo;
    }

    /// <summary>
    /// Gets a value indicating whether the operation resulting in this instance was successful.
    /// </summary>
    public bool IsSuccessful => _initialized && _exceptionDispatchInfo is null;

    /// <summary>
    /// Gets a value indicating whether the operation resulting in this instance wasn't successful.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => _initialized && _exceptionDispatchInfo is not null;

    /// <summary>
    /// Gets a value indicating whether the current instance contains a valid and non-null value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(true, nameof(ValueRef))]
    public bool HasValue => IsSuccessful && _value is not null;

    /// <summary>
    /// Gets the value of the result if the operation was successful; otherwise throws the stored exception.
    /// </summary>
    public T Value
    {
        get
        {
            ThrowIfUninitialized();
            ThrowIfFailed();
            return _value;
        }
    }

    /// <summary>
    /// Gets a reference to the value stored in the result when the operation is successful; otherwise throws
    /// the stored exception.
    /// </summary>
    public ref readonly T ValueRef
    {
        [UnscopedRef]
        get
        {
            ThrowIfUninitialized();
            ThrowIfFailed();
            return ref _value;
        }
    }

    /// <summary>
    /// Gets the exception associated with a failed operation, or <c>null</c> if the operation was successful.
    /// </summary>
    public Exception? Error => _exceptionDispatchInfo?.SourceException;

    /// <inheritdoc />
    [Pure]
    public override string? ToString()
    {
        if (!_initialized)
        {
            return string.Empty;
        }

        if (_exceptionDispatchInfo is not null)
        {
            return _exceptionDispatchInfo?.SourceException.ToString();
        }

        T? value = _value;
        ref T? local = ref value;
        return local is not null ? local.ToString() : string.Empty;
    }

    /// <summary>
    /// Attempts to retrieve the value stored in the result, if the operation was successful.
    /// </summary>
    /// <param name="value">When this method returns, contains the value of type <typeparamref name="T"/> if
    /// the operation was successful; otherwise, the default value for type <typeparamref name="T"/>.</param>
    /// <returns>
    /// A boolean indicating whether the value was successfully retrieved.
    /// </returns>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        value = default;
        if (!HasValue)
        {
            return false;
        }

        value = _value!;
        return true;
    }

    /// <summary>
    /// Retrieves the stored value if the operation is successful; otherwise, returns the default value of
    /// the underlying type.
    /// </summary>
    /// <returns>
    /// The stored value if the operation is successful; otherwise, the default value of the underlying type.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public T? GetValueOrDefault() => GetValueOrDefault(default);

    /// <summary>
    /// Gets the value stored in the result if the operation was successful; otherwise, returns the provided
    /// <paramref name="defaultValue"/>.
    /// </summary>
    /// <returns>
    /// The value stored in the result if successful; otherwise, the provided <paramref name="defaultValue"/>.
    /// </returns>
    [Pure]
    public T? GetValueOrDefault(T? defaultValue) => IsSuccessful ? _value : defaultValue;

    /// <summary>
    /// Executes the specified action if the result is successful, passing the result value to the action.
    /// </summary>
    /// <param name="onSuccess">The action to execute if the result is successful. This action receives the result
    /// value as its parameter.</param>
    /// <returns>
    /// The current <see cref="Result{T}"/> instance, allowing for method chaining.
    /// </returns>
    public Result<T> OnSuccess(Action<T?> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        if (IsSuccessful)
        {
            onSuccess(Value);
        }

        return this;
    }

    /// <summary>
    /// Executes the specified action if the result represents a failure,
    /// allowing custom handling of the exception.
    /// </summary>
    /// <param name="onFailure">The action to execute. Receives the exception as an argument.</param>
    /// <returns>
    /// The current <see cref="Result{T}"/> instance, allowing for method chaining.
    /// </returns>
    public Result<T> OnFailure(Action<Exception> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onFailure);
        if (IsFailure)
        {
            onFailure(Error);
        }

        return this;
    }

    /// <summary>
    /// Returns a new result with the specified default value if the current instance represents a failure.
    /// If the current instance is successful, it returns the existing result unchanged.
    /// </summary>
    /// <param name="defaultValue">The default value to be used to create the new result in case of a failure.</param>
    /// <returns>
    /// A new <see cref="Result{T}"/> instance containing <paramref name="defaultValue"/> if this instance represents
    /// a failure, or the current instance if it represents success.
    /// </returns>
    [Pure]
    public Result<T> IfFailure(T? defaultValue)
    {
        ThrowIfUninitialized();
        return IsFailure ? new Result<T>(defaultValue) : this;
    }

    /// <summary>
    /// Returns a new <see cref="Result{T}"/> instance with a specified default value if the current result is a failure.
    /// </summary>
    /// <param name="onFailure">A function to invoke if the result is a failure. It provides the exception from the
    /// failure and returns a value to be used.</param>
    /// <returns>
    /// A new <see cref="Result{T}"/> instance with the value returned by the <paramref name="onFailure"/> function
    /// if the result is a failure, otherwise the current result.
    /// </returns>
    [Pure]
    public Result<T> IfFailure(Func<Exception, T?> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onFailure);
        ThrowIfUninitialized();
        if (IsSuccessful)
        {
            return this;
        }

        try
        {
            var result = onFailure(Error);
            return new Result<T>(result);
        }
        catch (Exception ex)
        {
            return new Result<T>(ExceptionDispatchInfo.Capture(ex));
        }
    }

    /// <summary>
    /// Transforms the result of the operation into a value of type <typeparamref name="TOutput"/>
    /// by applying a function on the success value or the failure exception.
    /// </summary>
    /// <typeparam name="TOutput">The type of the value to return.</typeparam>
    /// <param name="onSuccess">A function to process the result value if the operation was successful.</param>
    /// <param name="onFailure">A function to process the exception if the operation failed.</param>
    /// <returns>
    /// A value of type <typeparamref name="TOutput"/> resulting from either applying
    /// <paramref name="onSuccess"/> on the success value or <paramref name="onFailure"/> on the exception.
    /// </returns>
    [Pure]
    public TOutput Fold<TOutput>(Func<T?, TOutput> onSuccess, Func<Exception, TOutput> onFailure)
        where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        ThrowIfUninitialized();

        return IsFailure ? onFailure(Error) : onSuccess(Value);
    }

    /// <summary>
    /// Asynchronously transforms the result into another value based on the success or failure state.
    /// </summary>
    /// <typeparam name="TOutput">The type of the transformed result.</typeparam>
    /// <param name="onSuccess">A function to invoke if the result is successful, which returns a task that
    /// produces a transformed value.</param>
    /// <param name="onFailure">A function to invoke if the result is a failure, which returns a task that
    /// produces a transformed value.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result of the task is the transformed value.
    /// </returns>
    [Pure]
    public Task<TOutput> FoldAsync<TOutput>(
        Func<T?, CancellationToken, Task<TOutput>> onSuccess,
        Func<Exception, CancellationToken, Task<TOutput>> onFailure,
        CancellationToken cancellationToken = default)
        where TOutput : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        ThrowIfUninitialized();

        return IsFailure
            ? onFailure(Error, cancellationToken)
            : onSuccess(Value, cancellationToken);
    }

    /// <summary>
    /// Maps the current result value to a new result value using the provided mapping function.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output result value.</typeparam>
    /// <param name="transform">The mapping function to apply to the result value.</param>
    /// <returns>
    /// A new <see cref="Result{TOutput}"/> instance containing the mapped value,
    /// or a new <see cref="Result{TOutput}"/> instance containing the exception if an error occurred.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transform"/> is <c>null</c>.</exception>
    [Pure]
    public Result<TOutput> Map<TOutput>(Func<T?, TOutput?> transform)
        where TOutput : notnull
    {
        ArgumentNullException.ThrowIfNull(transform);
        ThrowIfUninitialized();

        if (_exceptionDispatchInfo is not null)
        {
            return new Result<TOutput>(_exceptionDispatchInfo);
        }

        try
        {
            var result = transform(Value);
            return new Result<TOutput>(result);
        }
        catch (Exception ex)
        {
            return new Result<TOutput>(ExceptionDispatchInfo.Capture(ex));
        }
    }

    /// <summary>
    /// Asynchronously maps the current <see cref="Result{TValue}"/> to a <see cref="Result{TOutput}"/> using
    /// the specified mapping function.
    /// </summary>
    /// <typeparam name="TOutput">The type of the result after mapping.</typeparam>
    /// <param name="transform">The mapping function to be applied to the <typeparamref name="T"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the mapped
    /// <see cref="Result{TOutput}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="transform"/> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the <paramref name="cancellationToken"/> is canceled.</exception>
    [Pure]
    public async ValueTask<Result<TOutput>> MapAsync<TOutput>(
        Func<T?, CancellationToken, Task<TOutput?>> transform,
        CancellationToken cancellationToken = default)
        where TOutput : notnull
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(transform);
        ThrowIfUninitialized();

        if (_exceptionDispatchInfo is not null)
        {
            return new Result<TOutput>(_exceptionDispatchInfo);
        }

        try
        {
            var result = await transform(Value, cancellationToken);
            return new Result<TOutput>(result);
        }
        catch (Exception ex)
        {
            return new Result<TOutput>(ExceptionDispatchInfo.Capture(ex));
        }
    }

#pragma warning disable CS8763 // A method marked [DoesNotReturn] should not return.
    [StackTraceHidden, DoesNotReturn, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfUninitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("The result is not initialized.");
        }
    }

    [StackTraceHidden, DoesNotReturn, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfFailed()
    {
        if (IsFailure)
        {
            _exceptionDispatchInfo?.Throw();
        }
    }
#pragma warning restore CS8763 // A method marked [DoesNotReturn] should not return.

    /// <summary>
    /// Evaluates whether a specified result contains an exception, returning <c>true</c> if it does.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the result contains an exception, otherwise <c>false</c>.
    /// </returns>
    public static bool operator !(in Result<T> result) => result.IsFailure;

    /// <summary>
    /// Determines whether the result is successful for conditional statements.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <returns>
    /// <c>true</c> if the result is successful, otherwise <c>false</c>.
    /// </returns>
    public static bool operator true(in Result<T> result) => result.IsSuccessful;

    /// <summary>
    /// Determines whether the result is not successful for conditional statements.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <returns>
    /// <c>true</c> if the result is not successful, otherwise <c>false</c>.
    /// </returns>
    public static bool operator false(in Result<T> result) => !result;

    /// <summary>
    /// Computes a logical AND operation between two results, returning true if both are successful.
    /// </summary>
    /// <param name="left">The first result in comparison.</param>
    /// <param name="right">The second result in comparison.</param>
    /// <returns>
    /// <c>true</c> if both results are successful, otherwise <c>false</c>.
    /// </returns>
    public static bool operator &(in Result<T> left, in Result<T> right) =>
        left.IsSuccessful && right.IsSuccessful;

    /// <summary>
    /// Provides a fallback value on unsuccessful result through logical OR operation.
    /// </summary>
    /// <param name="left">The result being evaluated.</param>
    /// <param name="right">The fallback value.</param>
    /// <returns>
    /// The value of the result if successful, otherwise the fallback value.
    /// </returns>
    public static T? operator |(in Result<T> left, T? right) =>
        left.GetValueOrDefault(right);

    /// <summary>
    /// Selects the first successful result in a logical OR operation between two results.
    /// </summary>
    /// <param name="left">The primary result to evaluate.</param>
    /// <param name="right">The alternative result to evaluate if the primary is not successful.</param>
    /// <returns>
    /// The first successful result, or the alternative if the primary is not successful.
    /// </returns>
    public static Result<T> operator |(in Result<T> left, in Result<T> right) =>
        left.IsSuccessful ? left : right;
}