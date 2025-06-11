namespace AnxiousAnt.Locking;

/// <summary>
/// Provides extension methods for the <see cref="IAsyncLocker"/> interface.
/// </summary>
public static class AsyncLockerExtensions
{
    /// <summary>
    /// Acquires a lock asynchronously using a unique key, with optional cancellation support.
    /// </summary>
    /// <param name="locker">The <see cref="IAsyncLocker"/> instance that manages the lock acquisition.</param>
    /// <param name="key">The unique key identifying the lock.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.
    /// </returns>
    public static ValueTask<ILock> AcquireLockAsync(
        this IAsyncLocker locker,
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(locker);

        return locker.AcquireLockAsync(key, null, cancellationToken);
    }

    /// <summary>
    /// Attempts to acquire a lock asynchronously with an optional expiration time and cancellation support.
    /// </summary>
    /// <param name="locker">The <see cref="IAsyncLocker"/> instance that manages the lock acquisition.</param>
    /// <param name="key">The unique key identifying the lock.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.
    /// </returns>
    public static ValueTask<Maybe<ILock>> TryAcquireLockAsync(
        this IAsyncLocker locker,
        string key,
        CancellationToken cancellationToken = default) =>
        TryAcquireLockAsync(locker, key, null, cancellationToken);

    /// <summary>
    /// Attempts to acquire a lock asynchronously with an optional expiration time and cancellation support.
    /// </summary>
    /// <param name="locker">The <see cref="IAsyncLocker"/> instance that manages the lock acquisition.</param>
    /// <param name="key">The unique key identifying the lock.</param>
    /// <param name="expiration">The optional expiration time for the lock.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.
    /// </returns>
    public static ValueTask<Maybe<ILock>> TryAcquireLockAsync(
        this IAsyncLocker locker,
        string key,
        TimeSpan? expiration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(locker);

        return locker.TryAcquireLockAsync(key, TimeSpan.Zero, expiration, cancellationToken);
    }
}