namespace AnxiousAnt.Locking.InMemory;

public class InMemoryLockerTests
{
    #region ILocker

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void AcquireLock(string? key)
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var act = () => locker.AcquireLock(key!);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void AcquireLock_ShouldAcquireLock()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        using var @lock = locker.AcquireLock("test");

        // Assert
        @lock.ShouldNotBeNull();
        @lock.IsReleased.ShouldBeFalse();
    }

    [Fact]
    public void AcquireLock_ShouldReleaseLockWhenDisposed()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var @lock = locker.AcquireLock("test");
        @lock.Dispose();

        // Assert
        @lock.ShouldNotBeNull();
        @lock.IsReleased.ShouldBeTrue();
    }

    [Fact]
    public void AcquireLock_ShouldReleaseLockAutomaticallyWhenGivenAnExpiration()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        using var @lock = locker.AcquireLock("test", TimeSpan.FromSeconds(1));

        // Assert
        @lock.ShouldNotBeNull();
        @lock.IsReleased.ShouldBeFalse();

        Thread.Sleep(2000);
        @lock.IsReleased.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void TryAcquireLock_ShouldNotAcquireLockWhenKeyIsNullEmptyOrWhiteSpace(string? key)
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var maybe = locker.TryAcquireLock(key!);

        // Assert
        maybe.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void TryAcquireLock_ShouldAcquireLock()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var maybe = locker.TryAcquireLock("test");

        // Assert
        maybe.HasValue.ShouldBeTrue();
        maybe.Value.IsReleased.ShouldBeFalse();

        maybe.Dispose();
    }

    [Fact]
    public void TryAcquireLock_ShouldNotAcquireLockWhenKeyIsAlreadyLocked()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        using var @lock = locker.AcquireLock("test");
        var maybe = locker.TryAcquireLock("test", TimeSpan.FromSeconds(1));

        // Assert
        @lock.IsReleased.ShouldBeFalse();
        maybe.HasValue.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void IsLockAcquired_ShouldReturnFalseWhenKeyIsNullEmptyOrWhiteSpace(string? key)
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var isLocked = locker.IsLockAcquired(key!);

        // Assert
        isLocked.ShouldBeFalse();
    }

    [Fact]
    public void IsLockAcquired_ShouldReturnFalseWhenKeyIsNotLocked()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var isLocked = locker.IsLockAcquired("test");

        // Assert
        isLocked.ShouldBeFalse();
    }

    [Fact]
    public void IsLockAcquired_ShouldReturnTrueWhenKeyIsLocked()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        using var @lock = locker.AcquireLock("test");
        var isLocked = locker.IsLockAcquired("test");

        // Assert
        @lock.IsReleased.ShouldBeFalse();
        isLocked.ShouldBeTrue();
    }

    #endregion

    #region IAsyncLocker

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task AcquireLockAsync_ShouldThrowWhenKeyIsNullEmptyOrWhiteSpace(string? key)
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var act = async () => await locker.AcquireLockAsync(key!);

        // Assert
        await act.ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AcquireLockAsync_ShouldAcquireLock()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        await using var @lock = await locker.AcquireLockAsync("test");

        // Assert
        @lock.ShouldNotBeNull();
        @lock.IsReleased.ShouldBeFalse();
    }

    [Fact]
    public async Task AcquireLockAsync_ShouldReleaseLockWhenDisposed()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var @lock = await locker.AcquireLockAsync("test");
        await @lock.DisposeAsync();

        // Assert
        @lock.ShouldNotBeNull();
        @lock.IsReleased.ShouldBeTrue();
    }

    [Fact]
    public async Task AcquireLockAsync_ShouldReleaseLockAutomaticallyWhenGivenAnExpiration()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        await using var @lock = await locker.AcquireLockAsync("test", TimeSpan.FromSeconds(1));

        // Assert
        @lock.ShouldNotBeNull();
        @lock.IsReleased.ShouldBeFalse();

        await Task.Delay(2000);
        @lock.IsReleased.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task TryAcquireLockAsync_ShouldNotAcquireLockWhenKeyIsNullEmptyOrWhiteSpace(string? key)
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var maybe = await locker.TryAcquireLockAsync(key!);

        // Assert
        maybe.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task TryAcquireLockAsync_ShouldAcquireLock()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var maybe = await locker.TryAcquireLockAsync("test");

        // Assert
        maybe.HasValue.ShouldBeTrue();
        maybe.Value.IsReleased.ShouldBeFalse();

        await maybe.DisposeAsync();
    }

    [Fact]
    public async Task TryAcquireLockAsync_ShouldNotAcquireLockWhenKeyIsAlreadyLocked()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        await using var @lock = await locker.AcquireLockAsync("test");
        var maybe = await locker.TryAcquireLockAsync("test", TimeSpan.FromSeconds(1));

        // Assert
        @lock.IsReleased.ShouldBeFalse();
        maybe.HasValue.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task IsLockAcquiredAsync_ShouldReturnFalseWhenKeyIsNullEmptyOrWhiteSpace(string? key)
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var isLocked = await locker.IsLockAcquiredAsync(key!);

        // Assert
        isLocked.ShouldBeFalse();
    }

    [Fact]
    public async Task IsLockAcquiredAsync_ShouldReturnFalseWhenKeyIsNotLocked()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var isLocked = await locker.IsLockAcquiredAsync("test");

        // Assert
        isLocked.ShouldBeFalse();
    }

    [Fact]
    public async Task IsLockAcquiredAsync_ShouldReturnTrueWhenKeyIsLocked()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        await using var @lock = await locker.AcquireLockAsync("test");
        var isLocked = await locker.IsLockAcquiredAsync("test");

        // Assert
        @lock.IsReleased.ShouldBeFalse();
        isLocked.ShouldBeTrue();
    }

    #endregion

    [Fact]
    public void DisposingLockMultipleTimesShouldNotThrow()
    {
        // Arrange
        var locker = new InMemoryLocker();

        // Act
        var l = locker.AcquireLock("test");
        Action dispose = l.Dispose;

        l.Dispose();

        // Assert
        dispose.ShouldNotThrow();
        l.IsReleased.ShouldBeTrue();
    }
}