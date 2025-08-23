using System.Text.Json;

namespace AnxiousAnt.Text.Json.Converters;

public class OptionalConverterTests
{
    private static readonly OptionalConverter<string> Converter = new();
    private static readonly JsonSerializerOptions Options = new(JsonSerializerOptions.Default)
    {
        Converters = { Converter }
    };

    [Fact]
    public void Read_ShouldThrowWhenGivenNullType()
    {
        // Arrange
        Action act = () =>
        {
            var reader = new Utf8JsonReader([]);
            Converter.Read(ref reader, null!, null!);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Read_ShouldThrowWhenGivenNullOptions()
    {
        // Arrange
        Action act = () =>
        {
            var reader = new Utf8JsonReader([]);
            Converter.Read(ref reader, typeof(string), null!);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Read_ShouldReturnOptionalWithNullValueWhenValueIsNull()
    {
        // Arrange
        var reader = new Utf8JsonReader("null"u8);

        // Act
        var result = Converter.Read(ref reader, typeof(string), JsonSerializerOptions.Default);
        Action act1 = () => _ = result.Value;
        Action act2 = () => _ = result.ValueRef;

        // Assert
        result.IsDefault.ShouldBeFalse();
        result.IsEmpty.ShouldBeTrue();
        act1.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Write_ShouldThrowWhenGivenNullWriter()
    {
        // Arrange
        Action act = () => Converter.Write(null!, default, null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Write_ShouldThrowWhenGivenNullOptions()
    {
        // Arrange
        Action act = () => Converter.Write(new Utf8JsonWriter(Stream.Null), default, null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Write_ShouldThrowWhenGivenUninitializedOptional()
    {
        // Arrange
        var optional = default(Optional<string>);
        Action act = () => Converter.Write(new Utf8JsonWriter(Stream.Null), optional, Options);

        // Arrange
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Write_ShouldWriteNullWhenGivenOptionalNoValue()
    {
        // Assert
        Write(Optional<string>.None).ShouldBe("null");

        static string Write(Optional<string> optional)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            Converter.Write(writer, optional, Options);
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [Fact]
    public void Write_ShouldWriteValueWhenGivenSomeOptional()
    {
        // Arrange
        var optional = Optional.Of("hello world");

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        Converter.Write(writer, optional, Options);
        writer.Flush();

        var json = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        json.ShouldBe("\"hello world\"");
    }
}