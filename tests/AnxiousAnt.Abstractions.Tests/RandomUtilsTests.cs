namespace AnxiousAnt;

public class RandomUtilsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetRandomString_ThrowsWhenGivenInvalidLength(int length)
    {
        // Act
        Action act = () => RandomUtils.GetRandomString(length);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetRandomString_ReturnsRandomStringOfGivenLength()
    {
        // Act
        var result = RandomUtils.GetRandomString(10);

        // Assert
        result.Length.ShouldBe(10);
    }

    [Fact]
    public void GetRandomChars_ShouldDoNothingWhenGivenEmptyDestination()
    {
        // Act
        RandomUtils.GetRandomChars([]);
    }

    [Fact]
    public void GetRandomChars_ShouldFillDestination()
    {
        // Arrange
        char[] destination = new char[10];
        Array.Fill(destination, (char)0);

        // Act
        RandomUtils.GetRandomChars(destination);

        // Assert
        destination.ShouldAllBe(chr => chr != (char)0);
    }

    [Fact]
    public void GetRandomName_GeneratedNameShouldHaveExpectedFormat()
    {
        // Arrange
        var name = RandomUtils.GetRandomName();

        // Assert
        name.ShouldNotBe("-");
        name.ShouldNotStartWith("-");
        name.ShouldNotEndWith("-");
        name.Length.ShouldBeGreaterThan(5);
        name.ShouldContain('-', "Generated name does not contain an underscore");
        name.Any(char.IsDigit).ShouldBeFalse("Generated name contains numbers");
    }

    [Fact]
    public void TryGetRandomName_ShouldReturnFalseWhenDestinationIsEmpty()
    {
        // Assert
        RandomUtils.TryGetRandomName([], out var charsWritten).ShouldBeFalse();
        charsWritten.ShouldBe(0);
    }

    [Fact]
    public void TryGetRandomName_ShouldReturnFalseWhenDestinationIsTooSmall()
    {
        // Arrange
        var destination = new char[1];

        // Assert
        RandomUtils.TryGetRandomName(destination, out var charsWritten).ShouldBeFalse();
        charsWritten.ShouldBe(0);
        destination.ShouldBe([(char)0]);
    }

    [Fact]
    public void TryGetRandomName_ShouldReturnTrueWhenDestinationIsLargeEnough()
    {
        // Arrange
        var destination = new char[RandomUtils.GetMaxPossibleRandomNameLength()];

        // Act
        var result = RandomUtils.TryGetRandomName(destination, out var charsWritten);

        // Assert
        result.ShouldBeTrue();
        charsWritten.ShouldBeGreaterThan(0);

        var name = new string(destination[..charsWritten]);

        name.ShouldNotBe("-");
        name.ShouldNotStartWith("-");
        name.ShouldNotEndWith("-");
        name.Length.ShouldBeGreaterThan(5);
        name.ShouldContain('-', "Generated name does not contain an underscore");
        name.Any(char.IsDigit).ShouldBeFalse("Generated name contains numbers");
    }

    [Fact]
    public void Mock()
    {

    }
}