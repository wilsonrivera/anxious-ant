using System.Text.Json;

namespace AnxiousAnt;

partial class UrlTests
{
    [Fact]
    public void JsonConverter_ShouldProvideConverter()
    {
        // Arrange
        var expectedType = typeof(Url.UrlJsonConverter);

        // Act
        var converter = JsonSerializerOptions.Default.GetConverter(typeof(Url));

        // Assert
        expectedType.ShouldNotBeNull();
        converter.GetType().ShouldBe(expectedType);
    }

    [Fact]
    public void JsonConverter_ShouldWriteNullWhenGivenNull()
    {
        // Arrange
        Url? url = null;

        // Act
        var json = JsonSerializer.Serialize(url);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void JsonConverter_ShouldReadEmptyWhenValueIsNull()
    {
        // Arrange
        const string json = "null";

        // Act
        var url = JsonSerializer.Deserialize<Url>(json);

        // Assert
        url.ShouldBe(null);
    }

    [Fact]
    public void JsonConverter_ShouldThrowWhenGivenInvalidValue()
    {
        // Arrange
        const string json = "93480";
        Action act = () => JsonSerializer.Deserialize<Url>(json);

        // Assert
        act.ShouldThrow<JsonException>();
    }

    [Fact]
    public void JsonConverter_Roundtrip()
    {
        // Arrange
        var url = Url.Parse("https://example.com");
        var dto = new Dto(url);

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<Dto>(json);

        // Assert
        json.ShouldBe("{\"Url\":\"https://example.com\"}");
        deserialized.ShouldNotBeNull();
        deserialized.Url.Equals(url).ShouldBeTrue();
    }

    private sealed record Dto(Url Url);
}