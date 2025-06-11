using System.ComponentModel;
using System.Net.Mail;

namespace AnxiousAnt;

partial class UrlTests
{
    [Fact]
    public void TypeConverter_ShouldProvideConverter()
    {
        // Arrange
        var expectedType = typeof(Url.UrlTypeConverter);
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Assert
        expectedType.ShouldNotBeNull();
        converter.GetType().ShouldBe(expectedType);
    }

    [Fact]
    public void TypeConverter_CanConvertFrom_ShouldReturnFalseWhenSourceTypeIsNotStringOrMailAddress()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Assert
        converter.CanConvertFrom(typeof(object)).ShouldBeFalse();
    }

    [Fact]
    public void TypeConverter_CanConvertFrom_String_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Assert
        converter.CanConvertFrom(typeof(string)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_CanConvertFrom_Uri_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Assert
        converter.CanConvertFrom(typeof(Uri)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_CanConvertTo_ShouldReturnFalseWhenDestinationTypeIsNotStringOrUri()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Assert
        converter.CanConvertTo(typeof(object)).ShouldBeFalse();
    }

    [Fact]
    public void TypeConverter_CanConvertTo_String_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Assert
        converter.CanConvertTo(typeof(string)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_CanConvertTo_Uri_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Assert
        converter.CanConvertTo(typeof(Uri)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_ConvertFrom_ShouldThrowWhenSourceTypeIsNotStringOrUri()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Act
        Action act = () => converter.ConvertFrom(new object());

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }

    [Fact]
    public void TypeConverter_ConvertFrom_String_ShouldReturnValidEmail()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));

        // Act
        var result = converter.ConvertFrom("https://example.com");

        // Assert
        result.ShouldBeOfType<Url>();
        ((Url)result).Scheme.ShouldBe("https");
        ((Url)result).Host.ShouldBe("example.com");
    }

    [Fact]
    public void TypeConverter_ConvertFrom_Uri_ShouldReturnValidUrl()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));
        var uri = new Uri("https://example.com");

        // Act
        var result = converter.ConvertFrom(uri);

        // Assert
        result.ShouldBeOfType<Url>();
        ((Url)result).Scheme.ShouldBe("https");
        ((Url)result).Host.ShouldBe("example.com");
    }

    [Fact]
    public void TypeConverter_ConvertTo_ShouldThrowWhenDestinationTypeIsNotStringOrUri()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));
        var uri = new Uri("https://example.com");

        // Act
        Action act = () => converter.ConvertTo(uri, typeof(object));

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }

    [Fact]
    public void TypeConverter_ConvertTo_String_ShouldReturnUriWhenGivenValidUrl()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var url = Url.Parse("https://example.com");

        // Act
        var result = converter.ConvertTo(url, typeof(string));

        // Assert
        result.ShouldBeOfType<string>();
        result.ShouldBe("https://example.com");
    }

    [Fact]
    public void TypeConverter_ConvertTo_Uri_ShouldReturnUriWhenGivenValidUrl()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(Url));
        var url = Url.Parse("https://example.com");

        // Act
        var result = converter.ConvertTo(url, typeof(Uri));

        // Assert
        result.ShouldBeOfType<Uri>();
        result.ShouldBeEquivalentTo(new Uri("https://example.com"));
    }
}