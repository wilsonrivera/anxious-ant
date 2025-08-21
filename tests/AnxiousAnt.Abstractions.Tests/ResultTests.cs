using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

public class ResultTests
{
    [Fact]
    public void FromValue_ShouldReturnSuccess()
    {
        // Arrange
        var result = Result.FromValue(42);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void FromException_ShouldThrowWhenGivenNullException()
    {
        // Arrange
        Action act = () => Result.FromException<int>((Exception)null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void FromException_ShouldReturnFailureWhenGivenAnException()
    {
        // Arrange
        var ex = new Exception();
        var result = Result.FromException<int>(ex);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(ex);
    }

    [Fact]
    public void FromException_ShouldThrowWhenGivenExceptionDispatchInfo()
    {
        // Arrange
        Action act = () => Result.FromException<int>((ExceptionDispatchInfo)null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void FromException_ShouldReturnFailureWhenGivenAnExceptionDispatchInfo()
    {
        // Arrange
        var ex = new Exception();
        var edi = ExceptionDispatchInfo.Capture(ex);
        var result = Result.FromException<int>(edi);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(ex);
    }

    [Fact]
    public void IsResult_ShouldThrowWhenGivenNull()
    {
        // Arrange
        Action act = () => Result.IsResult(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void IsResult_ShouldReturnFalseWhenGivenUnboundResultType()
    {
        // Act
        var result = Result.IsResult(typeof(Result<>));

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsResult_ShouldReturnFalseWhenNotGivenResultType()
    {
        // Act
        var result = Result.IsResult(typeof(int));

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsResult_ShouldReturnTrueWhenGivenBoundResultType()
    {
        // Act
        var result = Result.IsResult(typeof(Result<int>));

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void GetUnderlyingType_ShouldThrowWhenGivenNull()
    {
        // Arrange
        Action act = () => Result.GetUnderlyingType(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void GetUnderlyingType_ShouldReturnNullWhenGivenUnboundResultType()
    {
        // Act
        var result = Result.GetUnderlyingType(typeof(Result<>));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetUnderlyingType_ShouldReturnNullWhenNotGivenResultType()
    {
        // Act
        var result = Result.GetUnderlyingType(typeof(int));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetUnderlyingType_ShouldReturnUnderlyingTypeWhenGivenBoundResultType()
    {
        // Act
        var result = Result.GetUnderlyingType(typeof(Result<int>));

        // Assert
        result.ShouldBe(typeof(int));
    }

    [Fact]
    public void Try_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        Action act = () => Result.Try<int>(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Try_ShouldCallFactory()
    {
        // Arrange
        var fakeFactory = A.Fake<Func<int>>();

        // Act
        var result = Result.Try(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Try_ShouldExceptionThrownByFactory()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = Result.Try<int>(() => throw exception);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public async Task TryAsync_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var act1 = () => Result.TryAsync<int>(null!).AsTask();
        var act2 = () => Result.TryAsync<int>(null!, CancellationToken.None).AsTask();

        // Assert
        await act1.ShouldThrowAsync<ArgumentNullException>();
        await act2.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task TryAsync_ShouldReturnFailureWhenCancelled()
    {
        // Arrange
        var fakeFactory = A.Fake<Func<CancellationToken, Task<int>>>();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var result = await Result.TryAsync(fakeFactory, cts.Token).AsTask();

        // Assert
        A.CallTo(fakeFactory).MustNotHaveHappened();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeOfType<OperationCanceledException>();
    }

    [Fact]
    public async Task TryAsync_ShouldReturnFailureWhenFactoryReturnsCompletedTaskWithException()
    {
        // Arrange
        var exception = new Exception();

        Task<int> Factory(CancellationToken _) => Task.FromException<int>(exception);

        // Act
        var result = await Result.TryAsync(Factory, CancellationToken.None).AsTask();

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public async Task TryAsync_ShouldHandleExceptionThrownByFactory()
    {
        // Arrange
        var exception = new Exception();

        Task<int> Factory(CancellationToken _)
        {
            throw exception;
        }

        // Act
        var result = await Result.TryAsync(Factory, CancellationToken.None).AsTask();

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public async Task TryAsync_ShouldHandleExceptionThrownByFactoryTask()
    {
        // Arrange
        var exception = new Exception();

        async Task<int> Factory(CancellationToken _)
        {
            await Task.Delay(100, CancellationToken.None);
            throw exception;
        }

        // Act
        var result = await Result.TryAsync(Factory, CancellationToken.None).AsTask();

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public async Task FromTaskAsync_ShouldThrowWhenGivenNullTask()
    {
        // Arrange
        var act = () => Result.FromTaskAsync<int>(null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task FromTaskAsync_ShouldReturnSuccessWhenTaskCompletesSuccessfully()
    {
        // Arrange
        var task = Task.FromResult(42);

        // Act
        var result = await Result.FromTaskAsync(task);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task FromTaskAsync_ShouldWaitForTaskToCompleteAndReturnSuccess()
    {
        async Task<int> Factory()
        {
            await Task.Delay(100, CancellationToken.None);
            return 42;
        }

        // Act
        var task = Factory();
        var result = await Result.FromTaskAsync(task);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBeNull();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task FromTaskAsync_ShouldReturnFailureWhenCancelled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var task = Task.FromCanceled<int>(cts.Token);

        // Act
        var result = await Result.FromTaskAsync(task);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeOfType<TaskCanceledException>();
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
        var result = await Result.FromTaskAsync(task);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }
}