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
        result.ValueOrNull().ShouldBeNull();
    }

    [Fact]
    public void ValueOrNull_ShouldReturnNullWhenFailed()
    {
        // Arrange
        var result = new Result<int>(ExceptionDispatchInfo.Capture(new Exception()));

        // Assert
        result.ValueOrNull().ShouldBeNull();
    }

    [Fact]
    public void ValueOrNull_ShouldReturnValueWhenSuccessful()
    {
        // Arrange
        var result = new Result<int>(42);

        // Assert
        result.ValueOrNull().ShouldBe(42);
    }
}