namespace AnxiousAnt;

public class MaybeExtensionsTests
{
    [Fact]
    public void TryGetValue_ShouldReturnFalseWhenMaybeDoesNotHaveValue()
    {
        // Arrange
        Maybe<int> maybe1 = default;
        Maybe<int> maybe2 = Maybe<int>.None;

        // Assert
        maybe1.TryGetValue(out _).ShouldBeFalse();
        maybe2.TryGetValue(out _).ShouldBeFalse();
    }

    [Fact]
    public void TryGetValue_ShouldReturnTrueWhenMaybeHasValue()
    {
        // Arrange
        Maybe<int> maybe = 1;

        // Assert
        maybe.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(1);
    }

    [Fact]
    public void GetValueOrDefault_ShouldReturnValue_WhenHasValue()
    {
        // Arrange
        var maybe = Maybe<int>.FromValue(10);

        // Act
        var value = maybe.GetValueOrDefault();

        // Assert
        value.ShouldBe(10);
    }

    [Fact]
    public void GetValueOrDefault_ShouldReturnDefaultValue_WhenHasNoValue()
    {
        // Arrange
        var maybe = Maybe<int>.None;

        // Act
        var value = maybe.GetValueOrDefault();

        // Assert
        value.ShouldBe(0);
    }

    [Fact]
    public void GetValueOrDefaultWithParam_ShouldReturnProvidedDefaultValue_WhenHasNoValue()
    {
        // Arrange
        var maybe = Maybe<string>.None;

        // Act
        var value = maybe.GetValueOrDefault("DefaultValue");

        // Assert
        value.ShouldBe("DefaultValue");
    }

    [Fact]
    public void Dispose_ShouldNotThrowWhenMaybeDoesNotHaveValue()
    {
        // Arrange
        Maybe<Disposable> maybe = default;

        // Act
        maybe.Dispose();
    }

    [Fact]
    public void Dispose_ShouldDisposeValue()
    {
        // Arrange
        var obj = new Disposable();
        Maybe<Disposable> maybe = obj;

        // Act
        maybe.Dispose();

        // Assert
        obj.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public async Task DisposeAsync_ShouldNotThrowWhenMaybeDoesNotHaveValue()
    {
        // Arrange
        Maybe<AsyncDisposable> maybe = default;

        // Act
        await maybe.DisposeAsync();
    }

    [Fact]
    public async Task DisposeAsync_ShouldDisposeValue()
    {
        // Arrange
        var obj = new AsyncDisposable();
        Maybe<AsyncDisposable> maybe = obj;

        // Act
        await maybe.DisposeAsync();

        // Assert
        obj.IsDisposed.ShouldBeTrue();
    }

    private sealed class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    private sealed class AsyncDisposable : IAsyncDisposable
    {
        public bool IsDisposed { get; private set; }
        public ValueTask DisposeAsync()
        {
            IsDisposed = true;
            return ValueTask.CompletedTask;
        }
    }
}