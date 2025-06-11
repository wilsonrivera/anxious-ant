using System.Collections.Specialized;
using System.Text.Json;

namespace AnxiousAnt.Text.Json.Converters;

public class NameValueCollectionConverterTests
{
    [Fact]
    public void Read_ShouldReturnNullWhenReadingNull()
    {
        // Arrange
        const string json = "null";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NameValueCollectionConverter());

        // Act
        var nvc = JsonSerializer.Deserialize<NameValueCollection>(json, options);

        // Assert
        nvc.ShouldBeNull();
    }

    [Fact]
    public void Read_ShouldReturnNullWhenReadingInvalidCollection()
    {
        // Arrange
        const string json = "\"not a name value collection\"";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NameValueCollectionConverter());

        // Act
        var nvc = JsonSerializer.Deserialize<NameValueCollection>(json, options);

        // Assert
        nvc.ShouldBeNull();
    }

    [Fact]
    public void Read_ShouldReturnIpAddressWhenReadingValidIpAddress()
    {
        // Arrange
        const string json = "{\"key\":[\"value1\",\"value2\"]}";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NameValueCollectionConverter());

        var expected = new NameValueCollection { { "key", "value1" }, { "key", "value2" } };

        // Act
        var result = JsonSerializer.Deserialize<NameValueCollection>(json, options);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Write_ShouldWriteNullWhenProvidedNull()
    {
        // Arrange
        NameValueCollection? nvc = null;
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NameValueCollectionConverter());

        // Act
        var result = JsonSerializer.Serialize(nvc, options);

        // Assert
        result.ShouldBe("null");
    }

    [Fact]
    public void Write_ShouldWriteIpAddressWhenProvidedAnIpAddress()
    {
        // Arrange
        var nvc = new NameValueCollection { { "key", "value1" }, { "key", "value2" } };
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NameValueCollectionConverter());

        // Act
        var result = JsonSerializer.Serialize(nvc, options);

        // Assert
        result.ShouldBe("{\"key\":[\"value1\",\"value2\"]}");
    }
}