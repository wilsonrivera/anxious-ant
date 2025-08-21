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
        result.IsDefault.ShouldBeTrue();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
        result.ToString().ShouldBeEmpty();
        result.TryGetValue(out _).ShouldBeFalse();
        result.ValueOr(42).ShouldBe(42);

        act.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void ParameterlessCtor_ShouldCreateSuccessfulResultWithDefaultValue()
    {
        // Arrange
        Result<int> result = new();

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(0);
        result.ValueRef.ShouldBe(0);
        result.Error.ShouldBeNull();
        result.ToString().ShouldBe("0");
        result.ValueOr(100).ShouldBe(0);
        result.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(0);
    }

    [Fact]
    public void Ctor_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var result = new Result<int>(42);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.ValueRef.ShouldBe(42);
        result.Error.ShouldBeNull();
        result.ToString().ShouldBe("42");
        result.ValueOr(100).ShouldBe(42);
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
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.HasValue.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
        result.ToString().ShouldBe(exception.ToString());
        result.TryGetValue(out _).ShouldBeFalse();
        result.ValueOr(42).ShouldBe(42);

        act.ShouldThrow<Exception>().ShouldBeSameAs(exception);
        act2.ShouldThrow<Exception>().ShouldBeSameAs(exception);
    }

    [Fact]
    public void Or_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Func<Exception, int>>();

        Action act = () => _ = result.Or(42);
        Action act2 = () => _ = result.Or(fakeAction);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void Or_ShouldReturnGivenValueWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        // Act
        var result2 = result.Or(42);

        // Assert
        result2.IsSuccessful.ShouldBeTrue();
        result2.IsFailure.ShouldBeFalse();
        result2.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeAction = A.Fake<Func<Exception, int>>();

        // Act
        var result2 = result.Or(12);
        _ = result.Or(fakeAction);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result2.Value.ShouldBe(42);
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void Or_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        Action act = () => _ = result.Or(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Or_ShouldReturnActionResultWhenNotSuccessful()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        // Act
        var result = input.Or(_ => 42);

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
        var fakeAction = A.Fake<Func<Exception, int>>();
        A.CallTo(fakeAction).Throws(exception2);

        // Act
        var result = input.Or(fakeAction);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception2);
        result.Error.ShouldNotBeSameAs(exception);
    }

    [Fact]
    public void OrElse_ShouldReturnAlternativeWhenUninitialized()
    {
        // Arrange
        var input = default(Result<int>);
        var alternative = new Result<int>(42);

        // Act
        var result = input.OrElse(alternative);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_ShouldReturnAlternativeWhenFailure()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));;
        var alternative = new Result<int>(42);

        // Act
        var result = input.OrElse(alternative);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_ShouldReturnOriginalWhenSuccessful()
    {
        // Arrange
        var input = new Result<int>(42);
        var alternative = new Result<int>(100);

        // Act
        var result = input.OrElse(alternative);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>();
        Action act = () => _ = result.OrElse(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void OrElse_ShouldNotCallFactoryWhenSuccessful()
    {
        // Arrange
        var input = new Result<int>(42);
        var fakeAlternative = A.Fake<Func<Result<int>>>();

        // Act
        var result = input.OrElse(fakeAlternative);

        // Assert
        A.CallTo(fakeAlternative).MustNotHaveHappened();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_ShouldCallFactoryWhenUnsuccessful()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeAlternative = A.Fake<Func<Result<int>>>();
        A.CallTo(() => fakeAlternative()).Returns(new Result<int>(42));

        // Act
        var result = input.OrElse(fakeAlternative);

        // Assert
        A.CallTo(fakeAlternative).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_ShouldHandleExceptionThrownByFactory()
    {
        // Arrange
        var exception = new Exception();
        var exception2 = new Exception();

        var input = new Result<int>(ExceptionDispatchInfo.Capture(exception));
        var fakeAlternative = A.Fake<Func<Result<int>>>();
        A.CallTo(fakeAlternative).Throws(exception2);

        // Act
        var result = input.OrElse(fakeAlternative);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception2);
        result.Error.ShouldNotBeSameAs(exception);
    }

    [Fact]
    public void OnSuccess_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Action<int>>();

        Action act = () => result.OnSuccess(fakeAction);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
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
    public void OnFailure_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Action<Exception>>();

        Action act = () => result.OnFailure(fakeAction);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
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

    [Fact]
    public void Match_ShouldThrowWhenGivenNullOnSuccessCallback()
    {
        // Arrange
        var result = new Result<int>();
        Action act = () => result.Match(null!, null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Match_ShouldThrowWhenGivenNullOnFailureCallback()
    {
        // Arrange
        var result = new Result<int>();
        var fakeOnSuccess = A.Fake<Action<int>>();
        Action act = () => result.Match(fakeOnSuccess, null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
        A.CallTo(fakeOnSuccess).MustNotHaveHappened();
    }

    [Fact]
    public void Match_ShouldCallOnSuccessWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeOnSuccess = A.Fake<Action<int>>();
        var fakeOnFailure = A.Fake<Action<Exception>>();

        // Act
        result.Match(fakeOnSuccess, fakeOnFailure);

        // Assert
        A.CallTo(fakeOnSuccess).MustHaveHappenedOnceExactly();
        A.CallTo(fakeOnFailure).MustNotHaveHappened();
    }

    [Fact]
    public void Match_ShouldCallOnFailureWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeOnSuccess = A.Fake<Action<int>>();
        var fakeOnFailure = A.Fake<Action<Exception>>();

        // Act
        result.Match(fakeOnSuccess, fakeOnFailure);

        // Assert
        A.CallTo(fakeOnSuccess).MustNotHaveHappened();
        A.CallTo(fakeOnFailure).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task MatchAsync_ShouldThrowWhenGivenNullOnSuccessCallback()
    {
        // Arrange
        var result = new Result<int>();
        Func<Task> act = () => result.MatchAsync(null!, null!, CancellationToken.None).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task MatchAsync_ShouldThrowWhenGivenNullOnFailureCallback()
    {
        // Arrange
        var result = new Result<int>();
        var fakeOnSuccess = A.Fake<Func<int, CancellationToken, Task>>();
        Func<Task> act = () => result.MatchAsync(fakeOnSuccess, null!, CancellationToken.None).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
        A.CallTo(fakeOnSuccess).MustNotHaveHappened();
    }

    [Fact]
    public async Task MatchAsync_ShouldCallOnSuccessWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);
        var fakeOnSuccess = A.Fake<Func<int, CancellationToken, Task>>();
        var fakeOnFailure = A.Fake<Func<Exception, CancellationToken, Task>>();

        // Act
        await result.MatchAsync(fakeOnSuccess, fakeOnFailure, CancellationToken.None);

        // Assert
        A.CallTo(fakeOnSuccess).MustHaveHappenedOnceExactly();
        A.CallTo(fakeOnFailure).MustNotHaveHappened();
    }

    [Fact]
    public async Task MatchAsync_ShouldCallOnFailureWhenNotSuccessful()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeOnSuccess = A.Fake<Func<int, CancellationToken, Task>>();
        var fakeOnFailure = A.Fake<Func<Exception, CancellationToken, Task>>();

        // Act
        await result.MatchAsync(fakeOnSuccess, fakeOnFailure, CancellationToken.None);

        // Assert
        A.CallTo(fakeOnSuccess).MustNotHaveHappened();
        A.CallTo(fakeOnFailure).MustHaveHappenedOnceExactly();
    }

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

    [Fact]
    public void LogicalNotOperator_SmokeTest()
    {
        // Only failure should be `true`

        (!default(Result<int>)).ShouldBeFalse();
        (!new Result<int>()).ShouldBeFalse();
        (!new Result<int>(ExceptionDispatchInfo.Capture(new Exception()))).ShouldBeTrue();
    }

    [Fact]
    public void TrueOperator_SmokeTest()
    {
        Result<int> result = default;
        if (result)
        {
            throw new Exception("Default result should return false");
        }

        result = new Result<int>();
        if (result)
        {
            // ignore
        }
        else
        {
            throw new Exception("Successful result should return true");
        }

        result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        if (result)
        {
            throw new Exception("Failure result should return false");
        }
    }

    [Fact]
    public void BitwiseAndOperator_SmokeTest()
    {
        // Only two successes should be `true`

        (default(Result<int>) & default(Result<int>)).ShouldBeFalse();
        (default(Result<int>) & new Result<int>()).ShouldBeFalse();
        (new Result<int>() & new Result<int>()).ShouldBeTrue();
        (default(Result<int>) & new Result<int>(ExceptionDispatchInfo.Capture(new Exception()))).ShouldBeFalse();
        (
            new Result<int>(ExceptionDispatchInfo.Capture(new Exception())) &
            new Result<int>(ExceptionDispatchInfo.Capture(new Exception()))
        ).ShouldBeFalse();
    }

    [Fact]
    public void BitwiseOrOperator_SmokeTest()
    {
        (default(Result<int>) | 42).ShouldBe(42);
        (default(Result<int>) | new Result<int>(42)).Value.ShouldBe(42);
        (new Result<int>(42) | default(Result<int>)).Value.ShouldBe(42);
        (new Result<int>() | new Result<int>(42)).Value.ShouldBe(0);
        (new Result<int>(ExceptionDispatchInfo.Capture(new Exception())) | 42).ShouldBe(42);
        (new Result<int>(ExceptionDispatchInfo.Capture(new Exception())) | new Result<int>(42)).Value.ShouldBe(42);
        (new Result<int>(42) | new Result<int>(ExceptionDispatchInfo.Capture(new Exception()))).Value.ShouldBe(42);
    }
}