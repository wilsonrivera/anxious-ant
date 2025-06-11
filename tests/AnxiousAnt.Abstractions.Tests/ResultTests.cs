using System.Runtime.ExceptionServices;

namespace AnxiousAnt;

public class ResultTests
{
    [Fact]
    public void Ctor_ShouldConstructDefaultInstance()
    {
        // Arrange
        var result = new Result<string>();

        // Assert
        result.IsDefault.ShouldBeTrue();
        result.IsFaulted.ShouldBeFalse();
        result.IsSuccess.ShouldBeTrue();
        result.HasValue.ShouldBeFalse();
        result.IsSuccessAndHasValue.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Exception.ShouldBeNull();
    }

    [Fact]
    public void FromValue_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var result = Result<int>.FromValue(42);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFaulted.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void FromException_ShouldThrowWhenGivenNullException()
    {
        // Assert
        Should.Throw<ArgumentNullException>(() => Result<int>.FromException((Exception?)null!));
        Should.Throw<ArgumentNullException>(() => Result<int>.FromException((ExceptionDispatchInfo?)null!));
    }

    [Fact]
    public void FromException_ShouldCreateFaultedResult()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var result = Result<int>.FromException(exception);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFaulted.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public void TryGetValue_ShouldReturnValueIfSuccessful()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Act
        var result = instance.TryGetValue(out var value);

        // Assert
        result.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalseIfFaulted()
    {
        // Arrange
        var exception = new Exception("Test");
        var instance = Result<string>.FromException(exception);

        // Act
        var result = instance.TryGetValue(out var value);

        // Assert
        result.ShouldBeFalse();
        value.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrRethrow_ShouldReturnValueIfSuccessful()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Act
        var result = instance.GetValueOrRethrow();

        // Assert
        result.ShouldBe(42);
    }

    [Fact]
    public void GetValueOrRethrow_ShouldThrowIfFaulted()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var result = Result<int>.FromException(exception);

        // Assert
        Should.Throw<InvalidOperationException>(() => result.GetValueOrRethrow());
    }

    [Fact]
    public void IfFaulted_ShouldReturnDefaultValueIfFaulted()
    {
        // Arrange
        var exception = new Exception("Test");
        var instance = Result<int>.FromException(exception);

        // Act
        var result = instance.IfFaulted(99);

        // Assert
        result.ShouldBe(99);
    }

    [Fact]
    public void IfFaulted_ShouldThrowWhenGivenNullCallback()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Assert
        Should.Throw<ArgumentNullException>(() => instance.IfFaulted(null!));
    }

    [Fact]
    public void IfFaulted_ShouldInvokeCallbackIfFaulted()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");
        var result = Result<int>.FromException(exception);

        // Act
        var value = result.IfFaulted(ex => ex.Message.Length);

        // Assert
        value.ShouldBe("Test".Length);
    }

    [Fact]
    public void IfFaulted_ShouldReturnOriginalValueIfNotFaulted()
    {
        // Arrange
        var result = Result<int>.FromValue(42);

        // Act
        var value = result.IfFaulted(99);

        // Assert
        value.ShouldBe(42);
    }

    [Fact]
    public void Match_ShouldThrowWhenGivenNullOnSuccessCallback()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Assert
        Should.Throw<ArgumentNullException>(() => instance.Match<double>(null!, null!));
    }

    [Fact]
    public void Match_ShouldThrowWhenGivenNullOnFailureCallback()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Assert
        Should.Throw<ArgumentNullException>(() => instance.Match(_ => 0d, null!));
    }

    [Fact]
    public void Match_ShouldInvokeOnSuccessCallbackIfSuccessful()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Act
        var result = instance.Match(value => value * 2, _ => 0d);

        // Assert
        result.ShouldBe(84d);
    }

    [Fact]
    public void Match_ShouldInvokeOnFailureCallbackIfFaulted()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");
        var instance = Result<int>.FromException(exception);

        // Act
        var result = instance.Match(_ => 0d, ex => ex.Message.Length);

        // Assert
        result.ShouldBe(exception.Message.Length);
    }

    [Fact]
    public void Map_ShouldThrowWhenGivenNullCallback()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Assert
        Should.Throw<ArgumentNullException>(() => instance.Map<double>(null!));
    }

    [Fact]
    public void Map_ShouldInvokeCallbackIfSuccessful()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Act
        var result = instance.Map(value => value * 2);

        // Assert
        result.ShouldBe(84);
    }

    [Fact]
    public void Map_ShouldReturnFaultedResultIfFaulted()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");
        var instance = Result<int>.FromException(exception);

        // Act
        var result = instance.Map(value => value * 2);

        // Assert
        result.IsFaulted.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public void Map_ShouldReturnFaultedResultIfCallbackThrows()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");
        var instance = Result<int>.FromValue(42);

        // Act
        var result = instance.Map<double>(_ => throw exception);

        // Assert
        result.IsFaulted.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public async Task MapAsync_ShouldThrowWhenGivenNullCallback()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Assert
        await Should.ThrowAsync<ArgumentNullException>(() => instance.MapAsync<double>(null!).AsTask());
    }

    [Fact]
    public async Task MapAsync_ShouldInvokeCallbackIfSuccessful()
    {
        // Arrange
        var instance = Result<int>.FromValue(42);

        // Act
        var result = await instance.MapAsync(value => Task.FromResult<double>(value * 2));

        // Assert
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MapAsync_ShouldReturnFaultedResultIfFaulted()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");
        var instance = Result<int>.FromException(exception);

        // Act
        var result = await instance.MapAsync(value => Task.FromResult<double>(value * 2));

        // Assert
        result.IsFaulted.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public async Task MapAsync_ShouldReturnFaultedResultIfCallbackThrows()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");
        var instance = Result<int>.FromValue(42);

        // Act
        var result = await instance.MapAsync<double>((_, _) => throw exception);

        // Assert
        result.IsFaulted.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public void GetHashCode_ShouldBeDifferentForDifferentValues()
    {
        // Arrange
        var result1 = Result<int>.FromValue(42);
        var result2 = Result<int>.FromValue(41);

        // Assert
        result1.GetHashCode().ShouldNotBe(result2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeSameForSameValues()
    {
        // Arrange
        var result1 = Result<int>.FromValue(42);
        var result2 = Result<int>.FromValue(42);

        // Assert
        result1.GetHashCode().ShouldBe(result2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeDifferentForDifferentStates()
    {
        // Arrange
        var result1 = Result<int>.FromValue(42);
        var result2 = Result<int>.FromException(new Exception("Test"));

        // Assert
        result1.GetHashCode().ShouldNotBe(result2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeSameForSameFailedStates()
    {
        // Arrange
        var ex = ExceptionDispatchInfo.Capture(new Exception("Test"));
        var result1 = Result<int>.FromException(ex);
        var result2 = Result<int>.FromException(ex);

        // Assert
        result1.GetHashCode().ShouldBe(result2.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalseWhenGivenDifferentType()
    {
        // Arrange
        var obj = Result<int>.FromValue(42);

        // Act
        var result = obj.Equals(new object());

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrueForEqualResults()
    {
        // Arrange
        var result1 = Result<int>.FromValue(42);
        var result2 = Result<int>.FromValue(42);

        // Assert
        result1.ShouldBe(result2);
    }

    [Fact]
    public void Equals_ShouldReturnFalseForDifferentResults()
    {
        // Arrange
        var result1 = Result<int>.FromValue(42);
        var result2 = Result<int>.FromValue(99);

        // Assert
        result1.ShouldNotBe(result2);
    }

    [Fact]
    public void Equals_ShouldReturnFalseForDifferentStates()
    {
        // Arrange
        var result1 = Result<int>.FromValue(42);
        var result2 = Result<int>.FromException(new Exception("Test"));
        var result3 = Result<int>.FromException(ExceptionDispatchInfo.Capture(new Exception("Test")));

        // Assert
        result1.ShouldNotBe(result2);
        result2.Exception.ShouldNotBeNull();
        result1.ShouldNotBe(result3);
        result3.Exception.ShouldNotBeNull();
    }

    [Fact]
    public void ToString_ShouldReturnValueIfSuccessful()
    {
        // Arrange
        var result = Result<int>.FromValue(42);

        // Assert
        result.ToString().ShouldBe("42");
    }

    [Fact]
    public void ToString_ShouldReturnExceptionMessageIfFaulted()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var result = Result<int>.FromException(exception);

        // Assert
        result.ToString().ShouldContain("Test exception");
    }

    [Fact]
    public void ThrowIfException_ShouldThrowWhenGivenFaultedResult()
    {
        // Arrange
        var exception = new Exception("Test");
        var result = Result<int>.FromException(exception);

        // Assert
        Should.Throw<Exception>(() => result.ThrowIfException());
    }

    [Fact]
    public void ThrowIfException_ShouldNotThrowWhenGivenSuccessfulResult()
    {
        // Arrange
        var result = Result<int>.FromValue(42);

        // Act
        result.ThrowIfException();
    }
}