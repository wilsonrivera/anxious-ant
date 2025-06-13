namespace AnxiousAnt.Text;

public class Porter2StemmerExtensionsTests
{
    [Fact]
    public void Stem_ShouldThrowWhenGivenNullStemmer()
    {
        // Arrange
        Action act = () => Porter2StemmerExtensions.Stem(null!, "test");

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Stem_ShouldReturnEmptyStringWhenGivenNullOrEmptyString(string? input)
    {
        // Act
        var result = Porter2StemmerExtensions.Stem(EnglishPorter2Stemmer.Instance, input);

        // Assert
        result.ShouldBeEmpty();
    }
}