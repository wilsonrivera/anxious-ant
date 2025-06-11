using System.Net;
using System.Text.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AnxiousAnt.Json.Converters;

public class JsonIpAddressConverterTests
{
    [Fact]
    public void Read_ShouldReturnNullWhenReadingNull()
    {
        // Arrange
        const string json = "null";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonIpAddressConverter());

        // Act
        var ip = JsonSerializer.Deserialize<IPAddress>(json, options);

        // Assert
        ip.ShouldBeNull();
    }

    [Fact]
    public void Read_ShouldThrowWhenReadingInvalidIpAddress()
    {
        // Arrange
        const string json = "\"not an ip address\"";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonIpAddressConverter());

        // Act
        Action act = () => JsonSerializer.Deserialize<IPAddress>(json, options);

        // Assert
        act.ShouldThrow<FormatException>();
    }

    [Fact]
    public void Read_ShouldReturnIpAddressWhenReadingValidIpAddress()
    {
        // Arrange
        const string json = "\"127.0.0.1\"";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonIpAddressConverter());

        // Act
        var result = JsonSerializer.Deserialize<IPAddress>(json, options);

        // Assert
        result.ShouldBe(IPAddress.Parse("127.0.0.1"));
    }

    [Fact]
    public void Write_ShouldWriteNullWhenProvidedNull()
    {
        // Arrange
        IPAddress? ip = null;
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonIpAddressConverter());

        // Act
        var result = JsonSerializer.Serialize(ip, options);

        // Assert
        result.ShouldBe("null");
    }

    [Fact]
    public void Write_ShouldWriteIpAddressWhenProvidedAnIpAddress()
    {
        // Arrange
        var ip = IPAddress.Parse("127.0.0.1");
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonIpAddressConverter());

        // Act
        var result = JsonSerializer.Serialize(ip, options);

        // Assert
        result.ShouldBe("\"127.0.0.1\"");
    }
}