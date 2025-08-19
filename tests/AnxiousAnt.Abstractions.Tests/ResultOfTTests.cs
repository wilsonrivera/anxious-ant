using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

public class ResultOfTTests
{
    [Fact]
    public void DefaultCtor_ShouldNotBeInitialized()
    {
        // Arrange
        Result<int> result = default;
        Action act = () => _ = result.Value;
        Action act2 = () => _ = result.ValueRef;

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
        result.ToString().ShouldBeEmpty();
        result.TryGetValue(out _).ShouldBeFalse();
        result.GetValueOrDefault(42).ShouldBe(42);

        act.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Ctor_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var result = new Result<int>(42);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.ValueRef.ShouldBe(42);
        result.Error.ShouldBeNull();
        result.ToString().ShouldBe("42");
        result.GetValueOrDefault(100).ShouldBe(42);
        result.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void Ctor_ShouldCreateFailureResult()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<int>(ExceptionDispatchInfo.Capture(exception));

        Action act = () => _ = result.Value;
        Action act2 = () => _ = result.ValueRef;

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.HasValue.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
        result.ToString().ShouldBe(exception.ToString());
        result.TryGetValue(out _).ShouldBeFalse();
        result.GetValueOrDefault(42).ShouldBe(42);

        act.ShouldThrow<Exception>().ShouldBeSameAs(exception);
        act2.ShouldThrow<Exception>().ShouldBeSameAs(exception);
    }

    [Fact]
    public void OnSuccess_ActionShouldNotBeCalledWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Action<int>>();

        // Act
        result.OnSuccess(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void OnSuccess_ShouldThrowWhenGivenANullAction()
    {
        // Arrange
        var result = new Result<int>(42);
        Action act = () => result.OnSuccess(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void OnSuccess_ShouldCallActionOnceWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeAction = A.Fake<Action<int>>();

        // Act
        result.OnSuccess(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void OnSuccess_ShouldNotCallActionWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeAction = A.Fake<Action<int>>();

        // Act
        result.OnSuccess(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void OnFailure_ActionShouldNotBeCalledWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Action<Exception>>();

        // Act
        result.OnFailure(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void OnFailure_ShouldThrowWhenGivenANullAction()
    {
        // Arrange
        var result = new Result<int>(42);
        Action act = () => result.OnFailure(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void OnFailure_ShouldCallActionWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeAction = A.Fake<Action<Exception>>();

        // Act
        result.OnFailure(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void OnFailure_ShouldNotCallActionOnceWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeAction = A.Fake<Action<Exception>>();

        // Act
        result.OnFailure(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    #region IfFailure

    [Fact]
    public void IfFailure_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Func<Exception, int>>();

        Action act = () => _ = result.IfFailure(42);
        Action act2 = () => _ = result.IfFailure(fakeAction);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void IfFailure_ShouldReturnGivenValueWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        // Act
        var result2 = result.IfFailure(42);

        // Assert
        result2.IsSuccessful.ShouldBeTrue();
        result2.IsFailure.ShouldBeFalse();
        result2.Value.ShouldBe(42);
    }

    [Fact]
    public void IfFailure_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeAction = A.Fake<Func<Exception, int>>();

        // Act
        var result2 = result.IfFailure(12);
        _ = result.IfFailure(fakeAction);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result2.Value.ShouldBe(42);
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void IfFailure_ShouldThrowWhenGivenANullAction()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        Action act = () => _ = result.IfFailure(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void IfFailure_ShouldReturnActionResultWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        // Act
        var result2 = result.IfFailure(_ => 42);

        // Assert
        result2.IsSuccessful.ShouldBeTrue();
        result2.IsFailure.ShouldBeFalse();
        result2.Value.ShouldBe(42);
    }

    [Fact]
    public void IfFailure_ShouldHandleExceptionThrownByAction()
    {
        // Arrange
        var exception = new Exception();
        var exception2 = new Exception();
        var result = new Result<int>(ExceptionDispatchInfo.Capture(exception));

        // Act
        var result2 = result.IfFailure(_ => throw exception2);

        // Assert
        result2.IsSuccessful.ShouldBeFalse();
        result2.IsFailure.ShouldBeTrue();
        result2.Error.ShouldNotBeNull();
        result2.Error.ShouldBeSameAs(exception2);
        result2.Error.ShouldNotBeSameAs(exception);
    }

    #endregion

    #region Fold

    [Fact]
    public void Fold_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var successFakeAction = A.Fake<Func<int, int>>();
        var failureFakeAction = A.Fake<Func<Exception, int>>();

        Action act = () => _ = result.Fold(successFakeAction, failureFakeAction);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
        A.CallTo(successFakeAction).MustNotHaveHappened();
        A.CallTo(failureFakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void Fold_ShouldThrowWhenGivenNullSuccessAction()
    {
        // Arrange
        var result = new Result<int>(42);
        Action act = () => _ = result.Fold(null!, A.Fake<Func<Exception, int>>());

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Fold_ShouldThrowWhenGivenNullFailAction()
    {
        // Arrange
        var result = new Result<int>(42);
        Action act = () => _ = result.Fold(A.Fake<Func<int, int>>(), null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Fold_ShouldCallSuccessActionWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var successFakeAction = A.Fake<Func<int, int>>();
        var failureFakeAction = A.Fake<Func<Exception, int>>();

        // Act
        _ = result.Fold(successFakeAction, failureFakeAction);

        // Assert
        A.CallTo(successFakeAction).MustHaveHappenedOnceExactly();
        A.CallTo(failureFakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void Fold_ShouldCallFailActionWhenNotSuccessful()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<int>(ExceptionDispatchInfo.Capture(exception));
        var successFakeAction = A.Fake<Func<int, int>>();
        var failureFakeAction = A.Fake<Func<Exception, int>>();

        // Act
        _ = result.Fold(successFakeAction, failureFakeAction);

        // Assert
        A.CallTo(successFakeAction).MustNotHaveHappened();
        A.CallTo(failureFakeAction).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region FoldAsync

    [Fact]
    public async Task FoldAsync_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var successFakeAction = A.Fake<Func<int, CancellationToken, Task<int>>>();
        var failureFakeAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();

        Func<Task<int>> act = async () => _ = await result.FoldAsync(
            successFakeAction,
            failureFakeAction
        );

        // Assert
        await act.ShouldThrowAsync<InvalidOperationException>();
        A.CallTo(successFakeAction).MustNotHaveHappened();
        A.CallTo(failureFakeAction).MustNotHaveHappened();
    }

    [Fact]
    public async Task FoldAsync_ShouldThrowWhenGivenNullSuccessAction()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeFailureAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();
        var act = () => result.FoldAsync(null!, fakeFailureAction);

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
        A.CallTo(fakeFailureAction).MustNotHaveHappened();
    }

    [Fact]
    public async Task FoldAsync_ShouldThrowWhenGivenNullFailAction()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeSuccessAction = A.Fake<Func<int, CancellationToken, Task<int>>>();
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
        var successFakeAction = A.Fake<Func<int, CancellationToken, Task<int>>>();
        var failureFakeAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();

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
        var successFakeAction = A.Fake<Func<int, CancellationToken, Task<int>>>();
        var failureFakeAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();

        // Act
        _ = await result.FoldAsync(successFakeAction, failureFakeAction);

        // Assert
        A.CallTo(successFakeAction).MustNotHaveHappened();
        A.CallTo(failureFakeAction).MustHaveHappenedOnceExactly();
    }

    #endregion

    #region Map

    [Fact]
    public void Map_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeTransform = A.Fake<Func<int, int>>();
        Action act = () => _ = result.Map(fakeTransform);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
        A.CallTo(fakeTransform).MustNotHaveHappened();
    }

    [Fact]
    public void Map_ShouldThrowWhenTransformIsNull()
    {
        // Arrange
        var result = new Result<int>(42);
        Action act = () => _ = result.Map<int>(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Map_ShouldNotCallTransformWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeTransform = A.Fake<Func<int, int>>();

        // Act
        var result2 = result.Map(fakeTransform);

        // Assert
        A.CallTo(fakeTransform).MustNotHaveHappened();
        result2.IsSuccessful.ShouldBeFalse();
        result2.IsFailure.ShouldBeTrue();
        result2.Error.ShouldNotBeNull();
        result2.Error.ShouldBeSameAs(result.Error);
    }

    [Fact]
    public void Map_ShouldCallTransformWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeTransform = A.Fake<Func<int, int>>();

        // Act
        _ = result.Map(fakeTransform);

        // Assert
        A.CallTo(fakeTransform).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Map_ShouldHandleExceptionThrownByTransform()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<int>(42);

        // Act
        var result2 = result.Map<int>(_ => throw exception);

        // Assert
        result2.IsSuccessful.ShouldBeFalse();
        result2.IsFailure.ShouldBeTrue();
        result2.Error.ShouldNotBeNull();
        result2.Error.ShouldBeSameAs(exception);
    }

    #endregion

    #region MapAsync

    [Fact]
    public async Task MapAsync_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeTransform = A.Fake<Func<int, CancellationToken, Task<int>>>();
        var act = () => result.MapAsync(fakeTransform).AsTask();

        // Assert
        await act.ShouldThrowAsync<InvalidOperationException>();
        A.CallTo(fakeTransform).MustNotHaveHappened();
    }

    [Fact]
    public async Task MapAsync_ShouldThrowWhenTransformIsNull()
    {
        // Arrange
        var result = new Result<int>(42);
        var act = () => result.MapAsync<int>(null!, CancellationToken.None).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MapAsync_ShouldNotCallTransformWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeTransform = A.Fake<Func<int, CancellationToken, Task<int>>>();

        // Act
        var result2 = await result.MapAsync(fakeTransform);

        // Assert
        A.CallTo(fakeTransform).MustNotHaveHappened();
        result2.IsSuccessful.ShouldBeFalse();
        result2.IsFailure.ShouldBeTrue();
        result2.Error.ShouldNotBeNull();
        result2.Error.ShouldBeSameAs(result.Error);
    }

    [Fact]
    public async Task MapAsync_ShouldCallTransformWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeTransform = A.Fake<Func<int, CancellationToken, Task<int>>>();

        // Act
        _ = await result.MapAsync(fakeTransform);

        // Assert
        A.CallTo(fakeTransform).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task MapAsync_ShouldHandleExceptionThrownByTransform()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<int>(42);

        // Act
        var result2 = await result.MapAsync<int>((_, _) => throw exception);

        // Assert
        result2.IsSuccessful.ShouldBeFalse();
        result2.IsFailure.ShouldBeTrue();
        result2.Error.ShouldNotBeNull();
        result2.Error.ShouldBeSameAs(exception);
    }

    #endregion
}