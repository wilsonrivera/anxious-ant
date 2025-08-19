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
}