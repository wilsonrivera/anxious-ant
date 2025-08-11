namespace AnxiousAnt;

public class SingletonTests
{
    [Fact]
    public void Exists_ShouldBeFalseWhenNoInstanceIsSet()
    {
        // Arrange
        Singleton<object>.Clear();

        // Act
        var exists = Singleton<object>.Exists;

        // Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public void Instance_ShouldThrowInvalidOperationExceptionWhenNoInstanceIsSet()
    {
        // Arrange
        Singleton<object>.Clear();

        // Act
        Action act = () => _ = Singleton<object>.Instance;

        // Assert
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void ReplaceWith_ShouldSetInstanceWhenCalledWithValidValue()
    {
        // Arrange
        var value = new object();

        // Act
        Singleton<object>.ReplaceWith(value);

        // Assert
        Singleton<object>.Instance.ShouldBeSameAs(value);
        Singleton<object>.Exists.ShouldBeTrue();
    }

    [Fact]
    public void ReplaceWith_ShouldThrowArgumentNullExceptionWhenCalledWithNull()
    {
        // Act
        Action act = () => Singleton<object>.ReplaceWith(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ReplaceWith_ShouldDisposeExistingInstanceWhenCalledWithNewInstance()
    {
        // Act
        var inst1 = new Disposable();
        var inst2 = new Disposable();

        // Act
        Singleton<Disposable>.ReplaceWith(inst1);
        Singleton<Disposable>.ReplaceWith(inst2);

        // Assert
        Singleton<Disposable>.Exists.ShouldBeTrue();
        Singleton<Disposable>.Instance.ShouldBe(inst2);
        inst1.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void Clear_ShouldResetInstanceToNullWhenCalled()
    {
        // Arrange
        var value = new object();
        Singleton<object>.ReplaceWith(value);

        // Act
        Singleton<object>.Clear();

        // Assert
        Singleton<object>.Exists.ShouldBeFalse();
    }

    [Fact]
    public void TryGet_ShouldReturnFalseAndDefaultWhenNoInstanceIsSet()
    {
        // Arrange
        Singleton<object>.Clear();

        // Act
        var success = Singleton<object>.TryGet(out var result);

        // Assert
        success.ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Fact]
    public void TryGet_ShouldReturnTrueAndInstanceWhenInstanceIsSet()
    {
        // Arrange
        var value = new object();
        Singleton<object>.ReplaceWith(value);

        // Act
        var success = Singleton<object>.TryGet(out var result);

        // Assert
        success.ShouldBeTrue();
        result.ShouldBeSameAs(value);
    }

    [Fact]
    public void Or_ShouldThrowWhenGivenNull()
    {
        // Arrange
        Action act = () => Singleton<object>.Or(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Or_ShouldReturnExistingValue()
    {
        // Arrange
        var value = new object();
        var defaultValue = new object();
        Singleton<object>.ReplaceWith(value);

        // Act
        var result = Singleton<object>.Or(defaultValue);

        // Assert
        result.ShouldBeSameAs(value);
    }

    [Fact]
    public void Or_ShouldReturnGivenValueWhenValueDoesNotExist()
    {
        // Arrange
        var defaultValue = new Temp();

        // Act
        var result = Singleton<Temp>.Or(defaultValue);

        // Assert
        result.ShouldBeSameAs(defaultValue);
    }

    [Fact]
    public void GetOrSet_ShouldThrowArgumentNullExceptionWhenDefaultValueIsNull()
    {
        // Arrange
        Action act = () => Singleton<object>.GetOrSet(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void GetOrSet_ShouldReturnDefaultValueWhenInstanceDoesNotExist()
    {
        // Arrange
        var defaultValue = new object();
        Singleton<object>.Clear();

        // Act
        var result = Singleton<object>.GetOrSet(defaultValue);

        // Assert
        result.ShouldBeSameAs(defaultValue);
        Singleton<object>.Instance.ShouldBeSameAs(defaultValue);
    }

    [Fact]
    public void GetOrSet_ShouldReturnExistingInstanceWhenInstanceExists()
    {
        // Arrange
        var value = new object();
        var defaultValue = new object();
        Singleton<object>.Clear();

        Singleton<object>.ReplaceWith(value);

        // Act
        var result = Singleton<object>.GetOrSet(defaultValue);

        // Assert
        result.ShouldBeSameAs(value);
        result.ShouldNotBeSameAs(defaultValue);
    }

    [Fact]
    public void GetOrCreate_ShouldReturnExistingInstanceWhenInstanceIsSet()
    {
        // Arrange
        var value = new object();
        Singleton<object>.ReplaceWith(value);

        // Act
        var result = Singleton<object>.GetOrCreate(() => new object());

        // Assert
        result.ShouldBeSameAs(value);
    }

    [Fact]
    public void GetOrCreate_ShouldCreateAndReturnNewInstanceWhenNoInstanceExists()
    {
        // Arrange
        var newValue = new object();
        Singleton<object>.Clear();

        // Act
        var result = Singleton<object>.GetOrCreate(() => newValue);

        // Assert
        result.ShouldBeSameAs(newValue);
        Singleton<object>.Instance.ShouldBeSameAs(newValue);
    }

    [Fact]
    public void GetOrCreate_ShouldThrowArgumentNullExceptionWhenFactoryIsNull()
    {
        // Act
        Action act = () => Singleton<object>.GetOrCreate(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void GetOrCreate_WithOutParam_ShouldIndicateCreatedWhenInstanceIsCreated()
    {
        // Arrange
        var newValue = new object();
        Singleton<object>.Clear();

        // Act
        var result = Singleton<object>.GetOrCreate(() => newValue, out var created);

        // Assert
        created.ShouldBeTrue();
        result.ShouldBeSameAs(newValue);
    }

    [Fact]
    public void GetOrCreate_WithOutParam_ShouldIndicateNotCreatedWhenInstanceExists()
    {
        // Arrange
        var existingValue = new object();
        Singleton<object>.ReplaceWith(existingValue);

        // Act
        var result = Singleton<object>.GetOrCreate(() => new object(), out var created);

        // Assert
        created.ShouldBeFalse();
        result.ShouldBeSameAs(existingValue);
    }

    private sealed class Temp;

    private sealed class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}