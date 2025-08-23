using System.Text.Json;

namespace AnxiousAnt.Text.Json.Converters;

public class OptionalConverterFactoryTests
{
    private static readonly OptionalConverterFactory Factory = new();
    private static readonly JsonSerializerOptions Options = new();

    [Fact]
    public void CanConvert_ShouldThrowWhenGivenNullType()
    {
        // Arrange
        Action act = () => Factory.CanConvert(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CanConvert_ShouldReturnFalseWhenGivenNonOptionalType()
    {
        // Assert
        Factory.CanConvert(typeof(string)).ShouldBeFalse();
    }

    [Fact]
    public void CanConvert_ShouldReturnTrueWhenGivenOptionalType()
    {
        // Assert
        Factory.CanConvert(typeof(Optional<string>)).ShouldBeTrue();
    }

    [Fact]
    public void CreateConverter_ShouldThrowWhenGivenNullType()
    {
        // Arrange
        Action act = () => Factory.CreateConverter(null!, null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CreateConverter_ShouldThrowWhenGivenNullOptions()
    {
        // Arrange
        Action act = () => Factory.CreateConverter(typeof(string), null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CreateConverter_ShouldReturnNullWhenGivenNonOptionalType()
    {
        // Assert
        Factory.CreateConverter(typeof(string), Options).ShouldBeNull();
    }

    [Fact]
    public void CreateConverter_ShouldReturnConverterWhenGivenOptionalType()
    {
        // Act
        var converter =Factory.CreateConverter(typeof(Optional<string>), Options);

        // Assert
        converter.ShouldNotBeNull();
        converter.ShouldBeOfType<OptionalConverter<string>>();
    }
}