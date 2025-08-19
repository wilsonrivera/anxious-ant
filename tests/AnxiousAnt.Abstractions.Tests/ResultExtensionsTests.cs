using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

public class ResultExtensionsTests
{
    [Fact]
    public void GetValueOrNull_ShouldReturnNullWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
        result.GetValueOrNull().ShouldBeNull();
    }

    [Fact]
    public void GetValueOrNull_ShouldReturnNullWhenFailed()
    {
        // Arrange
        var result = Result.FromException<int>(new Exception());

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.GetValueOrNull().ShouldBeNull();
    }

    [Fact]
    public void GetValueOrNull_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var result = Result.FromValue(42);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.GetValueOrNull().ShouldBe(42);
    }

    #region FoldAsync

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

    #endregion

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