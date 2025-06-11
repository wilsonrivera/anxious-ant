using System.Reflection;

namespace AnxiousAnt;

public partial class EmailAddressTests
{
    [Fact]
    public void ShouldBeEmptyWhenDefault()
    {
        // Act
        var email1 = default(EmailAddress);
        var email2 = new EmailAddress();

        // Assert
        email1.IsValid.ShouldBeFalse();
        email1.Address.ShouldBeNull();
        email1.DisplayName.ShouldBeNull();
        email1.User.ShouldBeNull();
        email1.Host.ShouldBeNull();

        email2.IsValid.ShouldBeFalse();
        email2.Address.ShouldBeNull();
        email2.DisplayName.ShouldBeNull();
        email2.User.ShouldBeNull();
        email2.Host.ShouldBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Parse_ShouldThrowWhenGivenNullOrEmpty(string? input)
    {
        // Act
        Action act = () => EmailAddress.Parse(input);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void Parse_ShouldThrowWhenGivenInvalidEmail()
    {
        // Act
        Action act = () => EmailAddress.Parse("not a valid email");

        // Assert
        act.ShouldThrow<FormatException>();
    }

    [Fact]
    public void Parse_ShouldReturnValidEmail()
    {
        // Arrange
        const string input = "\"Test\" <email@test.com>";

        // Act
        var email = EmailAddress.Parse(input);

        // Assert
        email.IsValid.ShouldBeTrue();
        email.Address.ShouldBe("email@test.com");
        email.DisplayName.ShouldBe("Test");
        email.User.ShouldBe("email");
        email.Host.ShouldBe("test.com");
        email.ToString().ShouldBe(input);
    }

    [Fact]
    public void TryParse_ParseDelegateShouldNotBeNull()
    {
        // Arrange
        var delegateField = typeof(EmailAddress).GetField(
            "TryParseDelegate",
            BindingFlags.Static | BindingFlags.NonPublic
        );

        // Assert
        delegateField.ShouldNotBeNull();
        delegateField.GetValue(null).ShouldNotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void TryParse_ShouldReturnFalseWhenGivenNullOrEmpty(string? input)
    {
        // Assert
        EmailAddress.TryParse(input, out _).ShouldBeFalse();
    }

    [Fact]
    public void TryParse_ShouldReturnFalseWhenGivenInvalidEmail()
    {
        // Assert
        EmailAddress.TryParse("not a valid email", out _).ShouldBeFalse();
    }

    [Fact]
    public void TryParse_ShouldReturnValidEmail()
    {
        // Arrange
        const string input = "\"Test\" <email@test.com>";

        // Act
        var result = EmailAddress.TryParse(input, out var email);

        // Assert
        result.ShouldBeTrue();
        email.IsValid.ShouldBeTrue();
        email.Address.ShouldBe("email@test.com");
        email.DisplayName.ShouldBe("Test");
        email.User.ShouldBe("email");
        email.Host.ShouldBe("test.com");
        email.ToString().ShouldBe(input);
    }

    // List obtained from https://en.wikipedia.org/wiki/Email_address

    [Theory]
    [InlineData("simple@example.com")]
    [InlineData("very.common@example.com")]
    [InlineData("FirstName.LastName@EasierReading.org")]
    [InlineData("x@example.com")]
    [InlineData("long.email-address-with-hyphens@and.subdomains.example.com")]
    [InlineData("user.name+tag+sorting@example.com")]
    [InlineData("name/surname@example.com")]
    [InlineData("admin@example")]
    [InlineData("example@s.example")]
    [InlineData("\" \"@example.org")]
    [InlineData("\"john..doe\"@example.org")]
    [InlineData("mailhost!username@example.org")]
    [InlineData("user%example.com@example.org")]
    [InlineData("user-@example.org")]
    [InlineData("postmaster@[123.123.123.123]")]
    [InlineData("postmaster@[IPv6:2001:0db8:85a3:0000:0000:8a2e:0370:7334]")]
    [InlineData("_test@[IPv6:2001:0db8:85a3:0000:0000:8a2e:0370:7334]")]
    [InlineData("I\u2764\ufe0fCHOCOLATE@example.com")]
    [InlineData("éléonore@example.com")]
    [InlineData("δοκιμή@παράδειγμα.δοκιμή")]
    [InlineData("我買@屋企.香港")]
    [InlineData("二ノ宮@黒川.日本")]
    [InlineData("медведь@с-балалайкой.рф")]
    [InlineData("स\u0902पर\u094dक@ड\u093eट\u093eम\u0947ल.भ\u093eरत")]
    public void TryParse_ShouldReturnTrueForValidEmailAddress(string input)
    {
        // Act
        var result = EmailAddress.TryParse(input, out _);

        // Assert
        result.ShouldBeTrue();
    }

    // List obtained from https://en.wikipedia.org/wiki/Email_address

    [Theory]
    [InlineData("abc.example.com")]
    [InlineData("a@b@c@example.com")]
    [InlineData("a\"b(c)d,e:f;g<h>i[j\\k]l@example.com")]
    [InlineData("just\"not\"right@example.com")]
    [InlineData("this is\"not\\allowed@example.com")]
    [InlineData("this\\ still\\\"not\\\\allowed@example.com")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234+x@example.com")]
    [InlineData("i.like.underscores@but_they_are_not_allowed_in_this_part")]
    // This one is supposed to be valid, however, the MailAddress parser, considers it invalid
    [InlineData("\"very.(),:;<>[]\\\".VERY.\\\"very@\\ \\\"very\\\".unusual\"@strange.example.com")]
    public void TryParse_ShouldReturnFalseForInvalidEmailAddress(string input)
    {
        // Act
        var result = EmailAddress.TryParse(input, out _);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldReturnZeroWhenGivenNullOrEmpty()
    {
        // Act
        var email1 = default(EmailAddress);
        var email2 = new EmailAddress();

        // Assert
        email1.IsValid.ShouldBeFalse();
        email1.GetHashCode().ShouldBe(0);

        email2.IsValid.ShouldBeFalse();
        email2.GetHashCode().ShouldBe(0);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueWhenGivenSameEmail()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("test@test.com");

        // Act
        var hc1 = email1.GetHashCode();
        var hc2 = email2.GetHashCode();

        // Assert
        hc2.ShouldBe(hc1);
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentValueWhenGivenDifferentEmails()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("test2@test.com");

        // Act
        var hc1 = email1.GetHashCode();
        var hc2 = email2.GetHashCode();

        // Assert
        hc2.ShouldNotBe(hc1);
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentValueWhenGivenDifferentDisplayName()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("Test <test@test.com>");

        // Act
        var hc1 = email1.GetHashCode();
        var hc2 = email2.GetHashCode();

        // Assert
        hc2.ShouldNotBe(hc1);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueWhenGivenDifferentCasing()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("TEST@test.com");

        // Act
        var hc1 = email1.GetHashCode();
        var hc2 = email2.GetHashCode();

        // Assert
        hc2.ShouldBe(hc1);
    }

    [Fact]
    public void Equals_ReturnFalseWhenGivenDefault()
    {
        // Arrange
        var email = EmailAddress.Parse("test@test.com");

        // Assert
        email.Equals(default).ShouldBeFalse();
        default(EmailAddress).Equals(email).ShouldBeFalse();
        email.Equals(new EmailAddress()).ShouldBeFalse();
        new EmailAddress().Equals(email).ShouldBeFalse();
    }

    [Fact]
    public void Equals_ReturnFalseWhenGivenNull()
    {
        // Arrange
        var email = EmailAddress.Parse("test@test.com");

        // Act
        var result = email.Equals(null);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ReturnsFalseWhenGivenObjectOtherThanEmailAddress()
    {
        // Arrange
        var email = EmailAddress.Parse("test@test.com");

        // Act
        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = email.Equals(123);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ReturnsFalseWhenGivenInvalidEmailAddressString()
    {
        // Arrange
        var email = EmailAddress.Parse("test@test.com");

        // Act
        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = email.Equals("not a valid email");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ReturnsTrueWhenGivenSameEmailAddressAsString()
    {
        // Arrange
        const string input = "test@test.com";
        var email = EmailAddress.Parse(input);

        // Act
        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = email.Equals(input);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("test@test.com")]
    [InlineData("Test <test@test.com>")]
    public void Equals_ReturnsTrueWhenGivenSameEmailAddress(string input)
    {
        // Arrange
        var email1 = EmailAddress.Parse(input);
        var email2 = EmailAddress.Parse(input);

        // Assert
        email1.Equals(email2).ShouldBeTrue();
        email2.Equals(email1).ShouldBeTrue();
    }

    [Fact]
    public void Equals_ReturnsTrueWhenGivenSameEmailAddressWithDifferentCasing()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("TEST@test.com");

        // Assert
        email1.Equals(email2).ShouldBeTrue();
        email2.Equals(email1).ShouldBeTrue();
    }

    [Fact]
    public void Equals_ReturnsTrueWhenGivenSameAddressAndDisplayName()
    {
        // Arrange
        var email1 = EmailAddress.Parse("Test <test@test.com>");
        var email2 = EmailAddress.Parse("Test <test@test.com>");

        // Assert
        email1.Equals(email2).ShouldBeTrue();
        email2.Equals(email1).ShouldBeTrue();
    }

    [Fact]
    public void Equals_ReturnsFalseWhenGivenSameAddressAndDisplayNameWithDifferentCasing()
    {
        // Arrange
        var email1 = EmailAddress.Parse("Test <test@test.com>");
        var email2 = EmailAddress.Parse("TEST <test@test.com>");

        // Assert
        email1.Equals(email2).ShouldBeFalse();
        email2.Equals(email1).ShouldBeFalse();
    }

    [Fact]
    public void Equals_ReturnsFalseWhenGivenSameAddressWithDifferentDisplayName()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("Test <test@test.com>");

        // Assert
        email1.Equals(email2).ShouldBeFalse();
        email2.Equals(email1).ShouldBeFalse();
    }

    [Fact]
    public void AddressEquals_ReturnsFalseWhenGivenDefaultValue()
    {
        // Arrange
        var email = EmailAddress.Parse("test@test.com");

        // Assert
        email.AddressEquals(default).ShouldBeFalse();
        email.AddressEquals(new EmailAddress()).ShouldBeFalse();
        default(EmailAddress).AddressEquals(email).ShouldBeFalse();
        new EmailAddress().AddressEquals(email).ShouldBeFalse();
    }

    [Fact]
    public void AddressEquals_ReturnsTrueWhenGivenSameAddress()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("test@test.com");

        // Act
        var result = email1.AddressEquals(email2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void AddressEquals_ReturnsTrueWhenGivenSameAddressWithDifferentCasing()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("TEST@test.com");

        // Act
        var result = email1.AddressEquals(email2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void WithDisplayName_ShouldThrowWhenEmailAddressIsDefault()
    {
        // Assert
        Should.Throw<InvalidOperationException>(() => default(EmailAddress).WithDisplayName("Test"));
        Should.Throw<InvalidOperationException>(() => new EmailAddress().WithDisplayName("Test"));
    }

    [Fact]
    public void WithDisplayName_ShouldCreateNewInstanceWithGivenDisplayName()
    {
        // Arrange
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = email1.WithDisplayName("Test");
        var email3 = email1.WithDisplayName(null);

        // Assert
        email1.ToString().ShouldBe("test@test.com");
        email2.Equals(email1).ShouldBeFalse();
        email2.ToString().ShouldBe("\"Test\" <test@test.com>");
        email3.Equals(email1).ShouldBeTrue();
        email3.Equals(email2).ShouldBeFalse();
        email3.ToString().ShouldBe("test@test.com");
    }

    [Fact]
    public void GetStringRepresentation_ShouldReturnOnlyAddressWhenDisplayNameIsMissing()
    {
        // Act
        var result = EmailAddress.GetStringRepresentation("test@test.com", null);

        // Assert
        result.ShouldBe("test@test.com");
    }

    [Fact]
    public void GetStringRepresentation_ShouldReturnDisplayNameWhenDisplayNameIsPresent()
    {
        // Act
        var result = EmailAddress.GetStringRepresentation("test@test.com", "Test");

        // Assert
        result.ShouldBe("\"Test\" <test@test.com>");
    }

    [Fact]
    public void GetStringRepresentation_ShouldEscapeDisplayNameWhenItHasQuote()
    {
        // Act
        var result = EmailAddress.GetStringRepresentation("test@test.com", "Test \"Test\" Test");

        // Assert
        result.ShouldBe("\"Test \\\"Test\\\" Test\" <test@test.com>");
    }
}