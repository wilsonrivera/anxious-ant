namespace AnxiousAnt.Locking;

/// <summary>
/// Represents a lock abstraction that can be acquired and released, supporting both synchronous and
/// asynchronous disposal.
/// </summary>
public interface ILock : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the lock has been released.
    /// </summary>
    bool IsReleased { get; }
}