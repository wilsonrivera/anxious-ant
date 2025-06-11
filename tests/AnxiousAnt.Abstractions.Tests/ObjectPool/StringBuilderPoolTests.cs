namespace AnxiousAnt.ObjectPool;

public class StringBuilderPoolTests
{
    [Fact]
    public void Return_ShouldThrowWhenGivenStringBuilderIsNull()
    {
        // Arrange
        Action act = () => StringBuilderPool.Return(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void ToStringAndReturn_ShouldReturnTheStringBuilderString()
    {
        // Arrange
        var sb = StringBuilderPool.Rent();

        // Act
        sb.Append("Hello world");
        var result = StringBuilderPool.ToStringAndReturn(sb);

        // Assert
        result.ShouldBe("Hello world");
    }

    [Fact]
    public void ToStringAndReturn_ShouldReturnEmptyStringWhenGivenAnEmptyStringBuilder()
    {
        // Arrange
        var sb = StringBuilderPool.Rent();

        // Act
        var result = StringBuilderPool.ToStringAndReturn(sb);

        // Assert
        result.ShouldBeEmpty();
    }
}