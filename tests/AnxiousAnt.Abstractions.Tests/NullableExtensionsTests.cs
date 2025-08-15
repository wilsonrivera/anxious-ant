namespace AnxiousAnt;

public class NullableExtensionsTests
{
    [Fact]
    public void TryGetValue_ShouldReturnFalseWhenGivenNull()
    {
        // Arrange
        int? nullable = null;

        // Assert
        nullable.TryGetValue(out _).ShouldBeFalse();
    }

    [Fact]
    public void TryGetValue_ShouldReturnTrueWhenGivenValue()
    {
        // Arrange
        int? nullable = 42;

        // Assert
        nullable.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(42);
    }
}