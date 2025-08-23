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
    public void Ctor_WithoutParameters_ShouldCreateSuccessfulResultWithDefaultValue()
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
    public void Ctor_WithValue_ShouldCreateSuccessfulResult()
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
    public void Ctor_WithValue_ShouldCreateFailureResult()
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
    public void Ctor_WithResult_ShouldBeUninitializedIfGivenUninitialized()
    {
        // Arrange
        Result<Result<int>> input = default;

        // Assert
        var result = new Result<int>(in input);

        // Assert
        result.IsDefault.ShouldBeTrue();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Ctor_WithResult_ShouldCreateFailureWhenGivenFailure()
    {
        // Arrange
        var exception = new Exception();
        var input = new Result<Result<int>>(ExceptionDispatchInfo.Capture(exception));

        // Act
        var result = new Result<int>(in input);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public void Ctor_WithResult_ShouldBeUninitializedWhenValueIsUninitialized()
    {
        // Arrange
        Result<Result<int>> input = new(default(Result<int>));

        // Assert
        var result = new Result<int>(in input);

        // Assert
        result.IsDefault.ShouldBeTrue();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Ctor_WithResult_ShouldCreateFailureWhenValueIsFailure()
    {
        // Arrange
        var exception = new Exception();
        var input = new Result<Result<int>>(new Result<int>(ExceptionDispatchInfo.Capture(exception)));

        // Act
        var result = new Result<int>(in input);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception);
    }

    [Fact]
    public void Ctor_WithResult_ShouldCreateSuccessfulWhenValueIsSuccessful()
    {
        // Arrange
        var input = new Result<Result<int>>(new Result<int>(42));

        // Act
        var result = new Result<int>(in input);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.ValueRef.ShouldBe(42);
    }

    [Fact]
    public void Or_WithAlternative_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        Action act = () => _ = result.Or(42);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Or_WithAlternative_ShouldReturnGivenAlternativeWhenNotSuccessful()
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
    public void Or_WithAlternative_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var input = new Result<int>(42);

        // Act
        var result = input.Or(12);

        // Assert
        input.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithFactory_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        Action act = () => _ = result.Or(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Or_WithFactory_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Func<Exception, int>>();
        Action act = () => _ = result.Or(fakeAction);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void Or_WithFactory_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var input = new Result<int>(42);
        var fakeAction = A.Fake<Func<Exception, int>>();

        A.CallTo(() => fakeAction(A<Exception>._)).Returns(12);

        // Act
        var result = input.Or(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithFactory_ShouldReturnFactoryResultWhenNotSuccessful()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeAction = A.Fake<Func<Exception, int>>();

        A.CallTo(() => fakeAction(A<Exception>._)).Returns(42);

        // Act
        var result = input.Or(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithFactory_ShouldHandleExceptionThrownByAction()
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
    public async Task OrAsync_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>();
        Func<Task> act = () => result.OrAsync(null!, CancellationToken.None).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrAsync_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();
        Func<Task> act = () => result.OrAsync(fakeAction).AsTask();

        // Assert
        await act.ShouldThrowAsync<InvalidOperationException>();
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public async Task OrAsync_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var input = new Result<int>(42);
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();

        // Act
        var result = await input.OrAsync(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
        result.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrAsync_ShouldReturnFactoryResultWhenNotSuccessful()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();

        A.CallTo(() => fakeAction(A<Exception>._, A<CancellationToken>._)).Returns(42);

        // Act
        var result = await input.OrAsync(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrAsync_ShouldHandleExceptionThrownByAction()
    {
        // Arrange
        var exception = new Exception();
        var exception2 = new Exception();

        var input = new Result<int>(ExceptionDispatchInfo.Capture(exception));
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<int>>>();
        A.CallTo(fakeAction).Throws(exception2);

        // Act
        var result = await input.OrAsync(fakeAction);

        // Assert
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception2);
        result.Error.ShouldNotBeSameAs(exception);
    }

    [Fact]
    public void OrElse_WithAlternative_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        Action act = () => _ = result.OrElse(new Result<int>(42));

        // Assert
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void OrElse_WithAlternative_ShouldReturnAlternativeWhenFailure()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var alternative = new Result<int>(42);

        // Act
        var result = input.OrElse(alternative);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithAlternative_ShouldReturnOriginalWhenSuccessful()
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
    public void OrElse_WithFactory_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>();
        Action act = () => _ = result.OrElse(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void OrElse_WithFactory_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Func<Exception, Result<int>>>();

        Action act = () => _ = result.OrElse(fakeAction);

        // Assert
        act.ShouldThrow<InvalidOperationException>();
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public void OrElse_WithFactory_ShouldNotCallFactoryWhenSuccessful()
    {
        // Arrange
        var input = new Result<int>(42);
        var fakeAction = A.Fake<Func<Exception, Result<int>>>();

        // Act
        var result = input.OrElse(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithFactory_ShouldCallFactoryWhenUnsuccessful()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeAction = A.Fake<Func<Exception, Result<int>>>();
        A.CallTo(() => fakeAction(A<Exception>._)).Returns(new Result<int>(42));

        // Act
        var result = input.OrElse(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithFactory_ShouldHandleExceptionThrownByFactory()
    {
        // Arrange
        var exception = new Exception();
        var exception2 = new Exception();

        var input = new Result<int>(ExceptionDispatchInfo.Capture(exception));
        var fakeAction = A.Fake<Func<Exception, Result<int>>>();
        A.CallTo(fakeAction).Throws(exception2);

        // Act
        var result = input.OrElse(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception2);
        result.Error.ShouldNotBeSameAs(exception);
    }

    [Fact]
    public async Task OrElseAsync_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        var result = new Result<int>();
        Func<Task> act = () => result.OrElseAsync(null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_ShouldThrowWhenUninitialized()
    {
        // Arrange
        Result<int> result = default;
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<Result<int>>>>();

        Func<Task> act = () => _ = result.OrElseAsync(fakeAction).AsTask();

        // Assert
        await act.ShouldThrowAsync<InvalidOperationException>();
        A.CallTo(fakeAction).MustNotHaveHappened();
    }

    [Fact]
    public async Task OrElseAsync_ShouldNotCallFactoryWhenSuccessful()
    {
        // Arrange
        var input = new Result<int>(42);
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<Result<int>>>>();

        // Act
        var result = await input.OrElseAsync(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustNotHaveHappened();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrElseAsync_ShouldCallFactoryWhenUnsuccessful()
    {
        // Arrange
        var input = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<Result<int>>>>();
        A.CallTo(() => fakeAction(A<Exception>._, A<CancellationToken>._)).Returns(new Result<int>(42));

        // Act
        var result = await input.OrElseAsync(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrElseAsync_ShouldHandleExceptionThrownByFactory()
    {
        // Arrange
        var exception = new Exception();
        var exception2 = new Exception();

        var input = new Result<int>(ExceptionDispatchInfo.Capture(exception));
        var fakeAction = A.Fake<Func<Exception, CancellationToken, Task<Result<int>>>>();
        A.CallTo(fakeAction).Throws(exception2);

        // Act
        var result = await input.OrElseAsync(fakeAction);

        // Assert
        A.CallTo(fakeAction).MustHaveHappenedOnceExactly();
        result.IsSuccessful.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBeSameAs(exception2);
        result.Error.ShouldNotBeSameAs(exception);
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
    public void LogicalNotOperator_ShouldReturnFalseWhenUninitialized()
    {
        (!default(Result<int>)).ShouldBeFalse();
    }

    [Fact]
    public void LogicalNotOperator_ShouldReturnFalseWhenSuccessful()
    {
        (!new Result<int>()).ShouldBeFalse();
        (!new Result<int>(42)).ShouldBeFalse();
    }

    [Fact]
    public void LogicalNotOperator_ShouldReturnTrueWhenUnsuccessful()
    {
        (!new Result<int>(ExceptionDispatchInfo.Capture(new Exception()))).ShouldBeTrue();
    }

    [Fact]
    public void TrueOperator_ShouldReturnFalseWhenUninitialized()
    {
        if (default(Result<int>))
        {
            throw new Exception("Default result should not be true");
        }
    }

    [Fact]
    public void TrueOperator_ShouldReturnTrueWhenSuccessful()
    {
        if (new Result<int>())
        {
            // ignore
        }
        else
        {
            throw new Exception("Successful result should be true");
        }
    }

    [Fact]
    public void TrueOperator_ShouldReturnFalseWhenUnsuccessful()
    {
        if (new Result<int>(ExceptionDispatchInfo.Capture(new Exception())))
        {
            throw new Exception("Unsuccessful result should not be true");
        }
    }

    [Fact]
    public void BitwiseAndOperator_ShouldReturnFalseWhenEitherIsUninitializedOrUnsuccessful()
    {
        var unsuccessful = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        (default(Result<int>) & new Result<int>()).ShouldBeFalse();
        (new Result<int>() & default(Result<int>)).ShouldBeFalse();
        (unsuccessful & new Result<int>()).ShouldBeFalse();
        (new Result<int>() & unsuccessful).ShouldBeFalse();
    }

    [Fact]
    public void BitwiseAndOperator_ShouldReturnTrueWhenBothAreSuccessful()
    {
        (new Result<int>() & new Result<int>()).ShouldBeTrue();
        (new Result<int>() & new Result<int>(42)).ShouldBeTrue();
        (new Result<int>(42) & new Result<int>()).ShouldBeTrue();
        (new Result<int>(42) & new Result<int>(42)).ShouldBeTrue();
    }

    [Fact]
    public void BitwiseOrOperator_ShouldReturnAlternativeWhenUninitialized()
    {
        // Act
        var result = default(Result<int>) | new Result<int>(42);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void BitwiseOrOperator_ShouldReturnAlternativeWhenUnsuccessful()
    {
        // Act
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception())) | new Result<int>(42);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void BitwiseOrOperator_ShouldReturnSelfWhenSuccessful()
    {
        // Act
        var result = new Result<int>(42) | new Result<int>(100);

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsSuccessful.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }
}