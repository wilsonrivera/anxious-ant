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
    public void GetRandomName_GeneratedNameShouldHaveExpectedFormat()
    {
        // Arrange
        var name = RandomUtils.GetRandomName();

        // Assert
        name.ShouldNotBe("_");
        name.ShouldNotStartWith("_");
        name.ShouldNotEndWith("_");
        name.Length.ShouldBeGreaterThan(5);
        name.ShouldContain('_', "Generated name does not contain an underscore");
        name.Any(char.IsDigit).ShouldBeFalse("Generated name contains numbers");
    }

    [Fact]
    public void GetRandomName_GeneratedNameShouldHaveDigit()
    {
        // Arrange
        var name = RandomUtils.GetRandomName(true);

        // Assert
        name.ShouldContain('_', "Generated name does not contain an underscore");
        name.Any(char.IsDigit).ShouldBeTrue("Generated name does not contain a digit");
    }
}