namespace AnxiousAnt.Locking.InMemory;

// Adapted from https://stackoverflow.com/a/31194647

/// <summary>
/// Provides an in-memory implementation of the <see cref="IAsyncLocker"/> interface.
/// </summary>
public sealed class InMemoryLocker : ILocker, IAsyncLocker
{
    private readonly Dictionary<string, RefCounted> _semaphores = [];

    /// <inheritdoc />
    public ILock AcquireLock(string key, TimeSpan? expiration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var semaphore = GetOrCreateSemaphore(key);
        semaphore.Value.Wait();

        return new Lock(this, semaphore, expiration);
    }

    /// <inheritdoc />
    public Maybe<ILock> TryAcquireLock(string key, TimeSpan timeout, TimeSpan? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Maybe<ILock>.None;
        }

        var semaphore = GetOrCreateSemaphore(key);
        if (semaphore.Value.Wait(timeout != TimeSpan.MaxValue ? timeout : Timeout.InfiniteTimeSpan))
        {
            return Maybe<ILock>.FromValue(new Lock(this, semaphore, expiration));
        }

        return Maybe<ILock>.None;
    }

    /// <inheritdoc />
    public bool IsLockAcquired(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        lock (_semaphores)
        {
            return _semaphores.TryGetValue(key, out var semaphore) && semaphore.Value.CurrentCount == 0;
        }
    }

    /// <inheritdoc />
    public async ValueTask<ILock> AcquireLockAsync(
        string key,
        TimeSpan? expiration,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var semaphore = GetOrCreateSemaphore(key);
        await semaphore.Value.WaitAsync(cancellationToken);

        return new Lock(this, semaphore, expiration);
    }

    /// <inheritdoc />
    public async ValueTask<Maybe<ILock>> TryAcquireLockAsync(
        string key,
        TimeSpan timeout,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            return Maybe<ILock>.None;
        }

        var semaphore = GetOrCreateSemaphore(key);
        if (await semaphore.Value.WaitAsync(
                timeout != TimeSpan.MaxValue ? timeout : Timeout.InfiniteTimeSpan,
                cancellationToken
            ))
        {
            return Maybe<ILock>.FromValue(new Lock(this, semaphore, expiration));
        }

        return Maybe<ILock>.None;
    }

    /// <inheritdoc />
    public ValueTask<bool> IsLockAcquiredAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key) || cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromResult(false);
        }

        lock (_semaphores)
        {
            return _semaphores.TryGetValue(key, out var semaphore)
                ? ValueTask.FromResult(semaphore.Value.CurrentCount == 0)
                : ValueTask.FromResult(false);
        }
    }

    private RefCounted GetOrCreateSemaphore(string key)
    {
        lock (_semaphores)
        {
            if (_semaphores.TryGetValue(key, out var semaphore))
            {
                semaphore.RefCount++;
            }
            else
            {
                semaphore = new RefCounted(key, new SemaphoreSlim(1));
                _semaphores[key] = semaphore;
            }

            return semaphore;
        }
    }

    private sealed class RefCounted(string key, SemaphoreSlim value)
    {
        public string Key { get; } = key;
        public SemaphoreSlim Value { get; } = value;
        public int RefCount { get; set; } = 1;
    }

    private sealed class Lock : ILock
    {
        private readonly InMemoryLocker _locker;
        private readonly RefCounted _refCounted;
        private readonly CancellationTokenSource? _cts;
        private volatile int _released;
        private bool _disposed;

        public Lock(InMemoryLocker locker, RefCounted refCounted, TimeSpan? expiration)
        {
            _locker = locker;
            _refCounted = refCounted;

            if (expiration.HasValue && expiration.Value != TimeSpan.MaxValue)
            {
                _cts = new CancellationTokenSource(expiration.Value);
                _cts.Token.Register(Release);
            }
        }

        /// <inheritdoc />
        public bool IsReleased => _released != 0;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _cts?.Dispose();

            Release();
        }

        private void Release()
        {
            if (Interlocked.Exchange(ref _released, 1) != 0)
            {
                return;
            }

            var refCounted = _refCounted;
            lock (_locker._semaphores)
            {
                if (_locker._semaphores.TryGetValue(_refCounted.Key, out var currentRefCounted))
                {
                    currentRefCounted.RefCount--;
                    if (currentRefCounted.RefCount == 0)
                    {
                        _locker._semaphores.Remove(_refCounted.Key);
                        refCounted = currentRefCounted;
                    }
                }
            }

            refCounted.Value.Release();
            if (refCounted.RefCount == 0)
            {
                refCounted.Value.Dispose();
            }
        }
    }
}