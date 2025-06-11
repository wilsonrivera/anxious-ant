namespace AnxiousAnt.Locking;

/// <summary>
/// Provides extension methods for the <see cref="ILocker"/> interface.
/// </summary>
public static class LockerExtensions
{
    /// <summary>
    /// Acquires a lock asynchronously using a unique key, with optional cancellation support.
    /// </summary>
    /// <param name="locker">The <see cref="ILocker"/> instance that manages the lock acquisition.</param>
    /// <param name="key">The unique key identifying the lock.</param>
    /// <returns>
    /// The acquired lock.
    /// </returns>
    public static ILock AcquireLock(this ILocker locker, string key)
    {
        ArgumentNullException.ThrowIfNull(locker);
        return locker.AcquireLock(key, null);
    }

    /// <summary>
    /// Attempts to acquire a lock asynchronously with an optional expiration time and cancellation support.
    /// </summary>
    /// <param name="locker">The <see cref="ILocker"/> instance that manages the lock acquisition.</param>
    /// <param name="key">The unique key identifying the lock.</param>
    /// <returns>
    /// If the lock is acquired, the result contains the lock; otherwise, it contains no value.
    /// </returns>
    public static Maybe<ILock> TryAcquireLock(this ILocker locker, string key) =>
        TryAcquireLock(locker, key, null);

    /// <summary>
    /// Attempts to acquire a lock asynchronously with an optional expiration time and cancellation support.
    /// </summary>
    /// <param name="locker">The <see cref="ILocker"/> instance that manages the lock acquisition.</param>
    /// <param name="key">The unique key identifying the lock.</param>
    /// <param name="expiration">The optional expiration time for the lock.</param>
    /// <returns>
    /// If the lock is acquired, the result contains the lock; otherwise, it contains no value.
    /// </returns>
    public static Maybe<ILock> TryAcquireLock(this ILocker locker, string key, TimeSpan? expiration)
    {
        ArgumentNullException.ThrowIfNull(locker);
        return locker.TryAcquireLock(key, TimeSpan.Zero, expiration);
    }
}