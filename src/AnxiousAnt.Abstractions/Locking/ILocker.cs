namespace AnxiousAnt.Locking;

public interface ILocker
{
    /// <summary>
    /// Acquires a lock for the specified key, ensuring mutual exclusivity.
    /// </summary>
    /// <param name="key">The key identifying the lock to be acquired.</param>
    /// <param name="expiration">The optional expiration time for the lock. If not specified, the lock will
    /// have no expiration.</param>
    /// <returns>
    /// The acquired lock.
    /// </returns>
    ILock AcquireLock(string key, TimeSpan? expiration);

    /// <summary>
    /// Attempts to acquire a lock for the specified key within the provided timeout period.
    /// </summary>
    /// <param name="key">The key identifying the lock to attempt to acquire.</param>
    /// <param name="timeout">The maximum duration to wait for the lock to become available.</param>
    /// <param name="expiration">The optional expiration time for the lock. If not specified, the lock will have
    /// no expiration.</param>
    /// <returns>
    /// If the lock is acquired, the result contains the lock; otherwise, it contains no value.
    /// </returns>
    Maybe<ILock> TryAcquireLock(string key, TimeSpan timeout, TimeSpan? expiration = null);

    /// <summary>
    /// Determines whether a lock for the specified key is currently acquired.
    /// </summary>
    /// <param name="key">The key identifying the lock to check.</param>
    /// <returns>
    /// A value indicating whether the lock is currently acquired.
    /// </returns>
    bool IsLockAcquired(string key);
}