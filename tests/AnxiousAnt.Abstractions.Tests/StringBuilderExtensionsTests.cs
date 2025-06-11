namespace AnxiousAnt;

public class StringBuilderExtensionsTests
{
    [Fact]
    public void AppendIfNotEmpty_ShouldNotAppendString_WhenBuilderIsEmpty()
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendIfNotEmpty("!!");

        // Assert
        sb.ToString().ShouldBeEmpty();
    }

    [Fact]
    public void AppendIfNotEmpty_ShouldAppendString_WhenBuilderNotEmpty()
    {
        // Arrange
        var sb = new StringBuilder("test");

        // Act
        sb.AppendIfNotEmpty("!!");

        // Assert
        sb.ToString().ShouldBe("test!!");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AppendIfNotEmpty_ShouldNotAppendString_WhenGivenStringIsNullOrEmpty(string? input)
    {
        // Arrange
        var sb = new StringBuilder("test");

        // Act
        sb.AppendIfNotEmpty(input);

        // Assert
        sb.ToString().ShouldBe("test");
    }

    [Fact]
    public void AppendIfNotEmpty_ShouldNotAppendChar_WhenBuilderIsEmpty()
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendIfNotEmpty('!');

        // Assert
        sb.ToString().ShouldBeEmpty();
    }
    [Fact]
    public void AppendIfNotEmpty_ShouldAppendChar_WhenBuilderNotEmpty()
    {
        // Arrange
        var sb = new StringBuilder("test");

        // Act
        sb.AppendIfNotEmpty('!');

        // Assert
        sb.ToString().ShouldBe("test!");
    }
}