using System.Text.Json;

namespace AnxiousAnt;

partial class EmailAddressTests
{
    [Fact]
    public void JsonConverter_ShouldProvideConverter()
    {
        // Arrange
        var expectedType = typeof(EmailAddress.EmailAddressJsonConverter);

        // Act
        var converter = JsonSerializerOptions.Default.GetConverter(typeof(EmailAddress));

        // Assert
        expectedType.ShouldNotBeNull();
        converter.GetType().ShouldBe(expectedType);
    }

    [Fact]
    public void JsonConverter_ShouldWriteNullWhenGivenDefaultInstance()
    {
        // Arrange
        EmailAddress email = default;

        // Act
        var json = JsonSerializer.Serialize(email);

        // Assert
        json.ShouldBe("null");
    }

    [Fact]
    public void JsonConverter_ShouldReadEmptyWhenValueIsNull()
    {
        // Arrange
        const string json = "null";

        // Act
        var email = JsonSerializer.Deserialize<EmailAddress>(json);

        // Assert
        email.ShouldBe(EmailAddress.Empty);
    }

    [Fact]
    public void JsonConverter_ShouldThrowWhenGivenInvalidValue()
    {
        // Arrange
        const string json = "93480";
        Action act = () => JsonSerializer.Deserialize<EmailAddress>(json);

        // Assert
        act.ShouldThrow<JsonException>();
    }

    [Fact]
    public void JsonConverter_Roundtrip()
    {
        // Arrange
        var email = EmailAddress.Parse("test@test.com");
        var dto = new Dto(email);

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<Dto>(json);

        // Assert
        json.ShouldBe("{\"Email\":\"test@test.com\"}");
        deserialized.ShouldNotBeNull();
        deserialized.Email.Equals(email).ShouldBeTrue();
    }

    private sealed record Dto(EmailAddress Email);
}