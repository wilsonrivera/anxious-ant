namespace AnxiousAnt.Text;

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
    public void ToStringAndReturn_ShouldThrowWhenGivenStringBuilderIsNull()
    {
        // Arrange
        Action act = () => StringBuilderPool.ToStringAndReturn(null!);

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

    [Fact]
    public void CopyToAndReturn_ShouldThrowWhenGivenStringBuilderIsNull()
    {
        // Arrange
        Action act = () => StringBuilderPool.CopyToAndReturn(null!, default);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CopyToAndReturn_ShouldNotCopyAnythingWhenGivenAnEmptyStringBuilder()
    {
        // Arrange
        var sb = StringBuilderPool.Rent();
        var array = new char[10];

        // Act
        var charsCopied = StringBuilderPool.CopyToAndReturn(sb, array.AsSpan());

        // Assert
        charsCopied.ShouldBe(0);
        array.ShouldAllBe(c => c == '\0');
    }

    [Fact]
    public void CopyToAndReturn_ShouldCopyAllCharactersWhenGivenAStringBuilderWithCharacters()
    {
        // Arrange
        var sb = StringBuilderPool.Rent().Append("hello world");
        var array = new char[15];

        // Act
        var charsCopied = StringBuilderPool.CopyToAndReturn(sb, array.AsSpan());

        // Assert
        charsCopied.ShouldBe(11);
        array.Take(11).ShouldBe("hello world");
    }

    [Fact]
    public void CopyToAndReturn_ShouldThrowWhenDestinationIsTooSmall()
    {
        // Arrange
        var sb = StringBuilderPool.Rent().Append("hello world");
        var array = new char[10];
        Action act = () => StringBuilderPool.CopyToAndReturn(sb, array.AsSpan());

        // Assert
        act.ShouldThrow<ArgumentException>();
    }
}