using System.ComponentModel;
using System.Net.Mail;

namespace AnxiousAnt;

partial class EmailAddressTests
{
    [Fact]
    public void TypeConverter_ShouldProvideConverter()
    {
        // Arrange
        var expectedType = typeof(EmailAddress.EmailAddressTypeConverter);
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Assert
        expectedType.ShouldNotBeNull();
        converter.GetType().ShouldBe(expectedType);
    }

    [Fact]
    public void TypeConverter_CanConvertFrom_ShouldReturnFalseWhenSourceTypeIsNotStringOrMailAddress()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Assert
        converter.CanConvertFrom(typeof(object)).ShouldBeFalse();
    }

    [Fact]
    public void TypeConverter_CanConvertFrom_String_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Assert
        converter.CanConvertFrom(typeof(string)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_CanConvertFrom_MailAddress_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Assert
        converter.CanConvertFrom(typeof(MailAddress)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_CanConvertTo_ShouldReturnFalseWhenDestinationTypeIsNotStringOrMailAddress()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Assert
        converter.CanConvertTo(typeof(object)).ShouldBeFalse();
    }

    [Fact]
    public void TypeConverter_CanConvertTo_String_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Assert
        converter.CanConvertTo(typeof(string)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_CanConvertTo_MailAddress_ShouldReturnTrue()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Assert
        converter.CanConvertTo(typeof(MailAddress)).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_ConvertFrom_ShouldThrowWhenSourceTypeIsNotStringOrMailAddress()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Act
        Action act = () => converter.ConvertFrom(new object());

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    public void TypeConverter_ConvertFrom_String_ShouldReturnDefaultWhenGivenValueIsNullOfEmpty(string input)
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Act
        var result = converter.ConvertFrom(input);

        // Assert
        result.ShouldBeOfType<EmailAddress>();
        ((EmailAddress)result).Equals(default).ShouldBeTrue();
    }

    [Fact]
    public void TypeConverter_ConvertFrom_String_ShouldThrowWhenGivenInvalidEmail()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Act
        Action act = () => converter.ConvertFrom("not a valid email");

        // Assert
        act.ShouldThrow<FormatException>();
    }

    [Fact]
    public void TypeConverter_ConvertFrom_String_ShouldReturnValidEmail()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));

        // Act
        var result = converter.ConvertFrom("test@test.com");

        // Assert
        result.ShouldBeOfType<EmailAddress>();
        ((EmailAddress)result).Address.ShouldBe("test@test.com");
    }

    [Fact]
    public void TypeConverter_ConvertFrom_MailAddress_ShouldReturnValidEmail()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var mailAddress = new MailAddress("test@test.com");

        // Act
        var result = converter.ConvertFrom(mailAddress);

        // Assert
        result.ShouldBeOfType<EmailAddress>();
        ((EmailAddress)result).Address.ShouldBe("test@test.com");
    }

    [Fact]
    public void TypeConverter_ConvertTo_ShouldThrowWhenDestinationTypeIsNotStringOrMailAddress()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var email = EmailAddress.Parse("test@test.com");

        // Act
        Action act = () => converter.ConvertTo(email, typeof(object));

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }

    [Fact]
    public void TypeConverter_ConvertTo_String_ShouldReturnNullWhenGivenDefault()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var email = default(EmailAddress);

        // Act
        var result = converter.ConvertTo(email, typeof(string));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void TypeConverter_ConvertTo_String_ShouldReturnAddressWhenGivenValidEmail()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var email = EmailAddress.Parse("test@test.com");

        // Act
        var result = converter.ConvertTo(email, typeof(string));

        // Assert
        result.ShouldBeOfType<string>();
        result.ShouldBe("test@test.com");
    }

    [Fact]
    public void TypeConverter_ConvertTo_MailAddress_ShouldReturnNullWhenGivenDefault()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var email = default(EmailAddress);

        // Act
        var result = converter.ConvertTo(email, typeof(MailAddress));

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void TypeConverter_ConvertTo_MailAddress_ShouldReturnMailAddressWhenGivenValidEmail()
    {
        // Arrange
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var email = EmailAddress.Parse("test@test.com");

        // Act
        var result = converter.ConvertTo(email, typeof(MailAddress));

        // Assert
        result.ShouldBeOfType<MailAddress>();
        result.ShouldBeEquivalentTo(new MailAddress("test@test.com"));
    }
}