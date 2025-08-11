namespace AnxiousAnt;

public partial class UrlTests
{
    #region User Info

    [Fact]
    public void UserInfo_ShouldReturnEmptyWhenUsernameAndPasswordAreNotSet()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Assert
        url.UserInfo.ShouldBeEmpty();
    }

    [Fact]
    public void UserInfo_ShouldReturnEmptyWhenOnlyPasswordIsSet()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithPassword("test");

        // Assert
        url.UserInfo.ShouldBeEmpty();
    }

    [Fact]
    public void UserInfo_ShouldReturnUsernameWhenOnlyUsernameIsSet()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithUsername("test");

        // Assert
        url.UserInfo.ShouldBe("test");
    }

    [Fact]
    public void UserInfo_ShouldReturnUsernameAndPasswordWhenBothAreSet()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithUsername("test").WithPassword("test");

        // Assert
        url.UserInfo.ShouldBe("test:test");
    }

    [Fact]
    public void UserInfo_ShouldReturnSameInstanceWhenCalledMultipleTimes()
    {
        // Arrange
        var url = Url.Parse("https://test:test@example.com/a/b/c");

        // Act
        var result1 = url.UserInfo;
        var result2 = url.UserInfo;

        // Assert
        result2.ShouldBeSameAs(result1);
    }

    #endregion

    #region Authority

    [Fact]
    public void Authority_ShouldReturnEmptyWhenHostIsNotSet()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithUsername("test").WithPort(80);

        // Assert
        url.Authority.ShouldBeEmpty();
    }

    [Fact]
    public void Authority_ShouldReturnHostWhenOnlyHostIsSet()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithHost("example.com");

        // Assert
        url.Authority.ShouldBe("example.com");
    }

    [Fact]
    public void Authority_ShouldIncludeUserInfoWhenSet()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithHost("example.com").WithUsername("test");

        // Assert
        url.Authority.ShouldBe("test@example.com");
    }

    [Fact]
    public void Authority_ShouldIncludePortWhenSet()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithHost("example.com").WithPort(80);

        // Assert
        url.Authority.ShouldBe("example.com:80");
    }

    [Fact]
    public void Authority_ShouldReturnSameInstanceWhenCalledMultipleTimes()
    {
        // Arrange
        var url = Url.Parse("https://test:test@example.com/a/b/c");

        // Act
        var result1 = url.Authority;
        var result2 = url.Authority;

        // Assert
        result2.ShouldBeSameAs(result1);
    }

    #endregion

    #region Root

    [Fact]
    public void Root_ShouldReturnEmptyWhenSchemeAndAuthorityAreEmpty()
    {
        // Arrange
        var url = new Url();

        // Assert
        url.Root.ShouldBeEmpty();
    }

    [Fact]
    public void Root_ShouldReturnSchemeWhenOnlySchemeIsSet()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithScheme("https");

        // Assert
        url.Root.ShouldBe("https");
    }

    [Fact]
    public void Root_ShouldReturnAuthorityWhenOnlyAuthorityIsSet()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithHost("example.com");

        // Assert
        url.Root.ShouldBe("example.com");
    }

    [Fact]
    public void Root_ShouldReturnSchemeAndAuthorityWhenBothAreSet()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithScheme("https").WithHost("example.com");

        // Assert
        url.Root.ShouldBe("https://example.com");
    }

    [Fact]
    public void Root_ShouldReturnSameInstanceWhenCalledMultipleTimes()
    {
        // Arrange
        var url = Url.Parse("http://example.com/a/b/c");

        // Act
        var result1 = url.Root;
        var result2 = url.Root;

        // Assert
        result2.ShouldBeSameAs(result1);
    }

    #endregion

    #region Path

    [Fact]
    public void Path_ShouldReturnEmptyWhenPathIsEmpty()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Assert
        url.Path.ShouldBeEmpty();
    }

    [Fact]
    public void Path_ShouldReturnSlashWhenPathHasLeadingSlash()
    {
        // Arrange
        var url = Url.Parse("https://example.com/");

        // Assert
        url.Path.ShouldBe("/");
    }

    [Fact]
    public void Path_ShouldReturnAllSegments()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegment("/a/b/c");

        // Assert
        url.Path.ShouldBe("/a/b/c");
    }

    [Fact]
    public void Path_ShouldAppendTrailingSlash()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegment("/a/b/c/");

        // Assert
        url.Path.ShouldBe("/a/b/c/");
    }

    [Fact]
    public void Path_ShouldReturnSameInstanceWhenCalledMultipleTimes()
    {
        // Arrange
        var url = Url.Parse("http://example.com/a/b/c");

        // Act
        var result1 = url.Path;
        var result2 = url.Path;

        // Assert
        result2.ShouldBeSameAs(result1);
    }

    #endregion

    [Fact]
    public void IsDataUrl_ShouldReturnTrueForDataUrl()
    {
        // Arrange
        var url = Url.Parse("data:text/plain;base64,SGVsbG8gV29ybGQh");

        // Assert
        url.IsDataUrl.ShouldBeTrue();
    }

    [Fact]
    public void IsDataUrl_ShouldReturnFalseForNonDataUrl()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Assert
        url.IsDataUrl.ShouldBeFalse();
    }

    [Theory]
    [InlineData("http://example.com", false)]
    [InlineData("https://example.com", true)]
    [InlineData("ws://example.com", false)]
    [InlineData("wss://example.com", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("sftp://example.com", true)]
    public void IsSecureScheme_ShouldReturnExpectedValue(string input, bool expected)
    {
        // Arrange
        var url = Url.Parse(input);

        // Act
        var result = url.IsSecureScheme;

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ToString_ShouldReturnExpectedValue()
    {
        // Arrange
        var url = Url.Parse("https://example.com/a/b/c?x=y#fragment");

        // Assert
        url.ToString().ShouldBe("https://example.com/a/b/c?x=y#fragment");
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueAsUriGetHashCode()
    {
        // Arrange
        var uri = new Uri("https://example.com/a/b/c?x=y#fragment");
        var url = Url.Parse("https://example.com/a/b/c?x=y#fragment");

        // Act
        var hc = url.GetHashCode();

        // Assert
        hc.ShouldBe(uri.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueForDifferentCasingHost()
    {
        // Arrange
        var url1 = Url.Parse("https://EXAMPLE.com");
        var url2 = Url.Parse("https://example.com");

        // Assert
        url1.GetHashCode().ShouldBe(url2.GetHashCode());
        url2.GetHashCode().ShouldBe(url1.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueForDifferentCasingSchema()
    {
        // Arrange
        var url1 = Url.Parse("HTTPS://example.com");
        var url2 = Url.Parse("https://example.com");

        // Assert
        url1.GetHashCode().ShouldBe(url2.GetHashCode());
        url2.GetHashCode().ShouldBe(url1.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalseWhenGivenNull()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        var result = url.Equals(null);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalseWhenGivenDifferentType()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = url.Equals(44);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrueWhenGivenSameInstance()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        var result = url.Equals(url);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnTrueWhenGivenUriWithSameValue()
    {
        // Arrange
        var uri = new Uri("https://example.com");
        var url = Url.Parse("https://example.com");

        // Act
        var result = url.Equals(uri);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalseWhenGivenDifferentUrl()
    {
        // Arrange
        var url1 = Url.Parse("https://example.com");
        var url2 = Url.Parse("https://example.com/test");

        // Act
        var result = url2.Equals(url1);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnTrueWhenGivenSameUrl()
    {
        // Arrange
        var url1 = Url.Parse("https://example.com");
        var url2 = Url.Parse("https://example.com");

        // Act
        var result = url2.Equals(url1);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnTrueForCaseInsensitiveSchema()
    {
        // Arrange
        var url1 = Url.Parse("HTTPS://example.com");
        var url2 = Url.Parse("https://example.com");

        // Act
        var result = url2.Equals(url1);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnTrueForCaseInsensitiveHost()
    {
        // Arrange
        var url1 = Url.Parse("https://EXAMPLE.com");
        var url2 = Url.Parse("https://example.com");

        // Act
        var result = url2.Equals(url1);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Clone_ShouldCreateNewInstance()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        var clone = url.Clone();

        // Assert
        clone.ShouldNotBeSameAs(url);
        clone.ToString().ShouldBe(url.ToString());
    }

    [Fact]
    public void Clone_ModifyingCloneShouldNotModifyOriginal()
    {
        // Arrange
        var url = Url.Parse("https://example.com/test?a=1");

        // Act
        var clone = url.Clone();
        clone.AddPathSegment("segment");
        clone.AddQueryParam("b", "yes");

        // Assert
        clone.ShouldNotBeSameAs(url);
        url.ToString().ShouldBe("https://example.com/test?a=1");
        clone.ToString().ShouldBe("https://example.com/test/segment?a=1&b=yes");
    }

    [Fact]
    public void Clone_ShouldPreserveLeadingSlash()
    {
        // Arrange
        var url = Url.Parse("https://example.example.com/a/b/c/d");

        // Act
        var clone = url.Clone();
        clone.SetQueryParam("a", "b");

        // Assert
        clone.ToString().ShouldBe("https://example.example.com/a/b/c/d?a=b");
    }

    [Fact]
    public void Clone_ShouldPreserveTrailingSlash()
    {
        // Arrange
        var url = Url.Parse("https://example.example.com/a/b/c/d/");

        // Act
        var clone = url.Clone();
        clone.SetQueryParam("a", "b");

        // Assert
        clone.ToString().ShouldBe("https://example.example.com/a/b/c/d/?a=b");
    }

    [Fact]
    public void ResetToRoot_ShouldResetUrl()
    {
        // Arrange
        var url = Url.Parse("https://example.com/test?a=1#fragment");

        // Act
        _ = url.ResetToRoot();

        // Assert
        url.ToString().ShouldBe("https://example.com");
    }

    [Fact]
    public void DivisionOp_ShouldThrowWhenGivenNull()
    {
        // Assert
        Should.Throw<ArgumentNullException>(() => ((Url)null!) / "path");
    }

    [Fact]
    public void DivisionOp_ShouldAppendPathSegment()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        var url2 = url / "path";
        url2 /= "segment";

        // Assert
        url2.Path.ShouldBe("/path/segment");
        url2.ToString().ShouldBe("https://example.com/path/segment");
    }
}