using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

public class ResultExtensionsTests
{
    [Fact]
    public void ValueOrNull_ShouldReturnNullWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
        result.ValueOrNull().ShouldBeNull();
    }

    [Fact]
    public void ValueOrNull_ShouldReturnNullWhenFailed()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.ValueOrNull().ShouldBeNull();
    }

    [Fact]
    public void ValueOrNull_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.ValueOrNull().ShouldBe(42);
    }

    [Fact]
    public void Flatten_ShouldReturnSuccessfulWhenSuccessful()
    {
        // Arrange
        var result = new Result<Result<int>>(new Result<int>(42));

        // Act
        var flatten = result.Flatten();

        // Assert
        flatten.IsSuccessful.ShouldBeTrue();
        flatten.Value.ShouldBe(42);
    }

    [Fact]
    public void Flatten_ShouldReturnFailureWhenFailure()
    {
        // Arrange
        var exception = new Exception();
        var input = new Result<Result<int>>(ExceptionDispatchInfo.Capture(exception));

        // Act
        var result = input.Flatten();

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public void Flatten_ShouldReturnFailureWhenNestedResultIsFailure()
    {
        // Arrange
        var exception = new Exception();
        var input = new Result<Result<int>>(new Result<int>(ExceptionDispatchInfo.Capture(exception)));

        // Act
        var result = input.Flatten();

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public void Flatten_ShouldBeUninitializedWhenTopmostIsUninitialized()
    {
        // Arrange
        Result<Result<string>> input = default;

        // Act
        var result = input.Flatten();

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Flatten_ShouldBeUninitializedWhenInnermostIsUninitialized()
    {
        // Arrange
        var input = new Result<Result<string>>(default(Result<string>));

        // Act
        var result = input.Flatten();

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Or_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        Action act = () => _ = ResultExtensions.Or(result, null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Or_ShouldReturnActionResultWhenNotSuccessful()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        // Act
        var result = input.Or(() => 42);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_ShouldHandleExceptionThrownByAction()
    {
        // Arrange
        var exception = new Exception();
        var exception2 = new Exception();

        var input = new Result<int>(ExceptionDispatchInfo.Capture(exception));
        var fakeAction = A.Fake<Func<int>>();
        A.CallTo(fakeAction).Throws(exception2);

        // Act
        var result = input.Or(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception2);
        result.Error.ShouldNotBeSameAs(exception);
    }

    [Fact]
    public void OrElse_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>();
        Action act = () => _ = ResultExtensions.OrElse(result, null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_ShouldThrowWhenGivenNullOnSuccessCallback()
    {
        // Arrange
        var result = new Result<int>();
        Func<Task> act = () => ResultExtensions.MatchAsync(result, null!, null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_ShouldThrowWhenGivenNullOnFailureCallback()
    {
        // Arrange
        var result = new Result<int>();
        var fakeOnSuccess = A.Fake<Func<int, Task>>();
        Func<Task> act = () => result.MatchAsync(fakeOnSuccess, null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
        A.CallTo(fakeOnSuccess).MustNotHaveHappened();
    }

    [Fact]
    public async Task MatchAsync_ShouldCallOnSuccessWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeOnSuccess = A.Fake<Func<int, Task>>();
        var fakeOnFailure = A.Fake<Func<Exception, Task>>();

        // Act
        await result.MatchAsync(fakeOnSuccess, fakeOnFailure);

        // Assert
        A.CallTo(fakeOnSuccess).MustHaveHappenedOnceExactly();
        A.CallTo(fakeOnFailure).MustNotHaveHappened();
    }

    [Fact]
    public async Task MatchAsync_ShouldCallOnFailureWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeOnSuccess = A.Fake<Func<int, Task>>();
        var fakeOnFailure = A.Fake<Func<Exception, Task>>();

        // Act
        await result.MatchAsync(fakeOnSuccess, fakeOnFailure);

        // Assert
        A.CallTo(fakeOnSuccess).MustNotHaveHappened();
        A.CallTo(fakeOnFailure).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task FoldAsync_ShouldThrowWhenGivenNullSuccessAction()
    {
        // Arrange
        Result<int> result = default;
        var fakeFailureAction = A.Fake<Func<Exception, Task<int>>>();
        var act = () => result.FoldAsync(null!, fakeFailureAction);

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
        A.CallTo(fakeFailureAction).MustNotHaveHappened();
    }

    [Fact]
    public async Task FoldAsync_ShouldThrowWhenGivenNullFailureAction()
    {
        // Arrange
        Result<int> result = default;
        var fakeSuccessAction = A.Fake<Func<int, Task<int>>>();
        var act = () => result.FoldAsync(fakeSuccessAction, null!);

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
        A.CallTo(fakeSuccessAction).MustNotHaveHappened();
    }

    [Fact]
    public async Task FoldAsync_ShouldCallSuccessActionWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var successFakeAction = A.Fake<Func<int, Task<int>>>();
        var failureFakeAction = A.Fake<Func<Exception, Task<int>>>();

        // Act
        _ = await result.FoldAsync(successFakeAction, failureFakeAction);

        // Assert
        A.CallTo(successFakeAction).MustHaveHappenedOnceExactly();
        A.CallTo(failureFakeAction).MustNotHaveHappened();
    }

    [Fact]
    public async Task FoldAsync_ShouldCallFailureActionWhenNotSuccessful()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<int>(ExceptionDispatchInfo.Capture(exception));
        var successFakeAction = A.Fake<Func<int, Task<int>>>();
        var failureFakeAction = A.Fake<Func<Exception, Task<int>>>();

        // Act
        _ = await result.FoldAsync(successFakeAction, failureFakeAction);

        // Assert
        A.CallTo(successFakeAction).MustNotHaveHappened();
        A.CallTo(failureFakeAction).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task MapAsync_ShouldThrowWhenTransformIsNull()
    {
        // Arrange
        var result = new Result<int>(42);
        var act = () => result.MapAsync<int, int>(null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_ShouldCallTransformWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeTransform = A.Fake<Func<int, Task<int>>>();

        // Act
        _ = await result.MapAsync(fakeTransform);

        // Assert
        A.CallTo(fakeTransform).MustHaveHappenedOnceExactly();
    }
}