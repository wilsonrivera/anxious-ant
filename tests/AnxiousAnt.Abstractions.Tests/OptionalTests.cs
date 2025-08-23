namespace AnxiousAnt;

public class OptionalTests
{
    [Fact]
    public void None_ShouldReturnOptionalWithNoValue()
    {
        // Act
        var optional = Optional.None<string>();

        // Assert
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Of_ShouldThrowWhenGivenNull()
    {
        // Arrange
        Action act = () => _ = Optional.Of<string>(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Of_ShouldReturnOptionalWithValue()
    {
        // Arrange
        const string value = "test";

        // Act
        var optional = Optional.Of(value);

        // Assert
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeFalse();
        optional.Value.ShouldBeSameAs(value);
    }

    [Fact]
    public void OfNullable_ShouldReturnOptionalWithNoValueWhenGivenNull()
    {
        // Act
        var optional = Optional.OfNullable<string>(null);

        // Assert
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
        optional.ValueKind.ShouldBe(Optional.NullValueKind);
    }

    [Fact]
    public void OfNullable_ShouldReturnOptionalWithValue()
    {
        // Arrange
        const string value = "test";

        // Act
        var optional = Optional.OfNullable(value);

        // Assert
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeFalse();
        optional.Value.ShouldBeSameAs(value);
    }

    [Fact]
    public void IsOptional_ShouldThrowWhenGivenNull()
    {
        // Arrange
        Action act = () => Optional.IsOptional(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void IsOptional_ShouldReturnFalseWhenGivenUnboundResultType()
    {
        // Act
        var result = Optional.IsOptional(typeof(Optional<>));

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsOptional_ShouldReturnFalseWhenNotGivenResultType()
    {
        // Act
        var result = Optional.IsOptional(typeof(int));

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsOptional_ShouldReturnTrueWhenGivenBoundResultType()
    {
        // Act
        var result = Optional.IsOptional(typeof(Optional<int>));

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void GetUnderlyingType_ShouldThrowWhenGivenNull()
    {
        // Arrange
        Action act = () => Optional.GetUnderlyingType(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void GetUnderlyingType_ShouldReturnNullWhenGivenUnboundResultType()
    {
        // Act
        var result = Optional.GetUnderlyingType(typeof(Optional<>));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetUnderlyingType_ShouldReturnNullWhenNotGivenResultType()
    {
        // Act
        var result = Optional.GetUnderlyingType(typeof(int));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetUnderlyingType_ShouldReturnUnderlyingTypeWhenGivenBoundResultType()
    {
        // Act
        var result = Optional.GetUnderlyingType(typeof(Optional<int>));

        // Assert
        result.ShouldBe(typeof(int));
    }

    [Fact]
    public void Try_ShouldThrowWhenGivenNullAction()
    {
        // Arrange
        Action act = () => Optional.Try<int>(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Try_ShouldHandleExceptionThrownByAction()
    {
        // Arrange
        var exception = new Exception();
        var fakeFactory = A.Fake<Func<int>>();
        A.CallTo(fakeFactory).Throws(exception);

        // Act
        var result = Optional.Try(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        result.IsDefault.ShouldBeFalse();
        result.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Try_ShouldReturnEmptyWhenActionCompletesSuccessfullyButValueIsNull()
    {
        // Arrange
        var fakeFactory = A.Fake<Func<string>>();
        A.CallTo(() => fakeFactory())!.Returns(null);

        // Act
        var result = Optional.Try(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        result.IsDefault.ShouldBeFalse();
        result.IsEmpty.ShouldBeTrue();
        result.ValueKind.ShouldBe(Optional.NullValueKind);
    }

    [Fact]
    public async Task TryAsync_ShouldThrowWhenGivenNullAction()
    {
        // Arrange
        var act = () => Result.TryAsync<int>(null!, CancellationToken.None).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task TryAsync_ShouldReturnEmptyWhenCancelled()
    {
        // Arrange
        var fakeFactory = A.Fake<Func<CancellationToken, Task<int>>>();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var result = await Optional.TryAsync(fakeFactory, cts.Token);

        // Assert
        A.CallTo(fakeFactory).MustNotHaveHappened();
        result.IsDefault.ShouldBeFalse();
        result.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public async Task TryAsync_ShouldReturnFailureWhenActionReturnsCompletedTaskWithException()
    {
        // Arrange
        var exception = new Exception();

        Task<int> Factory(CancellationToken _) => Task.FromException<int>(exception);

        // Act
        var result = await Optional.TryAsync(Factory);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public async Task TryAsync_ShouldHandleExceptionThrownByAction()
    {
        // Arrange
        var exception = new Exception();
        var fakeFactory = A.Fake<Func<CancellationToken, Task<int>>>();
        A.CallTo(() => fakeFactory(A<CancellationToken>._)).Throws(exception);

        // Act
        var result = await Optional.TryAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        result.IsDefault.ShouldBeFalse();
        result.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public async Task FromTaskAsync_ShouldThrowWhenGivenNullTask()
    {
        // Arrange
        var act = () => Optional.FromTaskAsync<int>(null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task FromTaskAsync_ShouldReturnNonEmptyWhenTaskCompletesSuccessfully()
    {
        // Arrange
        var task = Task.FromResult(42);

        // Act
        var optional = await Optional.FromTaskAsync(task);

        // Assert
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task FromTaskAsync_ShouldWaitForTaskToCompleteAndReturnNonEmpty()
    {
        async Task<int> Factory()
        {
            await Task.Delay(100, CancellationToken.None);
            return 42;
        }

        // Act
        var task = Factory();
        var optional = await Optional.FromTaskAsync(task);

        // Assert
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task FromTaskAsync_ShouldReturnEmptyWhenCancelled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var task = Task.FromCanceled<int>(cts.Token);

        // Act
        var optional = await Optional.FromTaskAsync(task);

        // Assert
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public async Task FromTaskAsync_ShouldHandleExceptionThrownByTask()
    {
        // Arrange
        var exception = new Exception();

        async Task<int> Factory()
        {
            await Task.Delay(100, CancellationToken.None);
            throw exception;
        }

        // Act
        var task = Factory();
        var optional = await Optional.FromTaskAsync(task);

        // Assert
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
    }
}