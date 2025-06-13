namespace AnxiousAnt.Text;

public class Porter2StemmerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Stem_ShouldReturnEmptyStringWhenGivenNullOrEmptyString(string? input)
    {
        // Act
        var result = Porter2Stemmer.Stem(input);

        // Assert
        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("         ")]
    [InlineData("at")]
    [InlineData("a")]
    public void Stem_ShouldReturnSameStringWhenGivenShortOrWhiteSpaceString(string input)
    {
        // Act
        var result = Porter2Stemmer.Stem(input);

        // Assert
        result.ShouldBe(input);
    }

    [Fact]
    public void Stem_SmokeTest()
    {
        // Arrange
        var data = GetData().ToArray();

        // Assert
        foreach ((string input, string expected) in data)
        {
            Porter2Stemmer.Stem(input).ShouldBe(expected);
        }
    }

    private static IEnumerable<(string, string)> GetData()
    {
        using var stream = File.OpenRead("./data/stemmer/data.csv");
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            yield return (parts[0], parts[1]);
        }
    }
}