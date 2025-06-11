namespace AnxiousAnt.Locking;

/// <summary>
/// Represents an abstraction for acquiring and managing locks to ensure mutual exclusivity
/// in a distributed or concurrent environment.
/// </summary>
public interface IAsyncLocker
{
    /// <summary>
    /// Acquires a lock for the specified key, ensuring mutual exclusivity.
    /// </summary>
    /// <param name="key">The key identifying the lock to be acquired.</param>
    /// <param name="expiration">The optional expiration time for the lock. If not specified, the lock will have
    /// no expiration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.
    /// </returns>
    ValueTask<ILock> AcquireLockAsync(
        string key,
        TimeSpan? expiration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to acquire a lock for the specified key within the provided timeout period.
    /// </summary>
    /// <param name="key">The key identifying the lock to attempt to acquire.</param>
    /// <param name="timeout">The maximum duration to wait for the lock to become available.</param>
    /// <param name="expiration">The optional expiration time for the lock. If not specified, the lock will have
    /// no expiration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.
    /// </returns>
    ValueTask<Maybe<ILock>> TryAcquireLockAsync(
        string key,
        TimeSpan timeout,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether a lock for the specified key is currently acquired.
    /// </summary>
    /// <param name="key">The key identifying the lock to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.
    /// </returns>
    ValueTask<bool> IsLockAcquiredAsync(string key, CancellationToken cancellationToken = default);
}