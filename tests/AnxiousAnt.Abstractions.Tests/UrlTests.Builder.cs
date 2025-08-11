namespace AnxiousAnt;

partial class UrlTests
{
    #region Scheme

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void WithScheme_ShouldSetEmptyStringWhenGivenNullOrWhiteSpace(string? input)
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithScheme(input!);

        // Assert
        url.Scheme.ShouldBeEmpty();
    }

    [Fact]
    public void WithScheme_ShouldSetScheme()
    {
        // Arrange
        var url = new Url();

        // Act
        _ = url.WithScheme("http");

        // Assert
        url.Scheme.ShouldBe("http");
    }

    [Fact]
    public void WithScheme_ShouldChangeScheme()
    {
        // Arrange
        var url = Url.Parse("http://example.com");

        // Act
        url.WithScheme("https");

        // Assert
        url.Scheme.ShouldBe("https");
        url.ToString().ShouldBe("https://example.com");
    }

    [Theory]
    [InlineData("https")]
    [InlineData("fake-scheme")]
    public void WithScheme_ShouldSetSchemeWhenGivenValidScheme(string scheme)
    {
        // Arrange
        var url = new Url();

        // Act
        url.WithScheme(scheme);

        // Assert
        url.Scheme.ShouldBe(scheme);
    }

    [Theory]
    [InlineData("1http")]
    [InlineData("http!")]
    [InlineData("ht*tp")]
    [InlineData("h tt p")]
    [InlineData("http://")]
    [InlineData(":scheme")]
    [InlineData("_http")]
    [InlineData("http_")]
    [InlineData("http/")]
    public void WithScheme_ShouldThrowWhenSchemeIsInvalid(string scheme)
    {
        // Arrange
        Action act = () => new Url().WithScheme(scheme);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    #endregion

    #region UserInfo

    [Theory]
    [InlineData(null, "", "https://example.com")]
    [InlineData("", "", "https://example.com")]
    [InlineData("      ", "", "https://example.com")]
    [InlineData("test", "test", "https://test@example.com")]
    public void WithUsername_ShouldUpdateUsernamePart(string? username, string expectedUsername, string expectedUrl)
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithUsername(username);

        // Assert
        url.Username.ShouldBe(expectedUsername);
        url.Password.ShouldBeEmpty();
        url.UserInfo.ShouldBe(expectedUsername);
        url.ToString().ShouldBe(expectedUrl);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("      ", "")]
    [InlineData("test", "test")]
    public void WithPassword_ShouldUpdatePasswordPartWhenUsernameIsPresent(string? password, string expectedPassword)
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithPassword(password);

        // Assert
        url.Username.ShouldBeEmpty();
        url.Password.ShouldBe(expectedPassword);
        url.UserInfo.ShouldBeEmpty();
        url.ToString().ShouldBe("https://example.com");
    }

    [Theory]
    [InlineData(null, null, "", "", "", "https://example.com")]
    [InlineData("", "", "", "", "", "https://example.com")]
    [InlineData("     ", "", "", "", "", "https://example.com")]
    [InlineData("", "       ", "", "", "", "https://example.com")]
    [InlineData("test", null, "test", "", "test", "https://test@example.com")]
    [InlineData(null, "test", "", "test", "", "https://example.com")]
    [InlineData("test", "test", "test", "test", "test:test", "https://test:test@example.com")]
    public void WithUserInfo_ShouldUpdateUsernameAndPassword(
        string? username,
        string? password,
        string expectedUsername,
        string expectedPassword,
        string expectedUserInfo,
        string expectedUrl)
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithUsername(username).WithPassword(password);

        // Assert
        url.Username.ShouldBe(expectedUsername);
        url.Password.ShouldBe(expectedPassword);
        url.UserInfo.ShouldBe(expectedUserInfo);
        url.ToString().ShouldBe(expectedUrl);
    }

    [Fact]
    public void ClearUserInfo_ShouldRemoveUsernameAndPassword()
    {
        // Arrange
        var url = Url.Parse("https://test:test@example.com");

        // Act
        _ = url.ClearUserInfo();

        // Assert
        url.Username.ShouldBeEmpty();
        url.Password.ShouldBeEmpty();
        url.UserInfo.ShouldBeEmpty();
        url.ToString().ShouldBe("https://example.com");
    }

    #endregion

    #region Host

    [Fact]
    public void WithHost_ShouldUpdateHost()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithHost("example.org");

        // Assert
        url.Host.ShouldBe("example.org");
        url.ToString().ShouldBe("https://example.org");
    }

    [Fact]
    public void WithHost_ShouldThrowWhenGivenHostIsInvalid()
    {
        // Arrange
        Action act = () => new Url().WithHost("exa_mple.com");

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData("example.com")]
    public void WithAsciiHost_ShouldReturnSameInstanceWhenHostIsNotAnIdn(string? host)
    {
        // Arrange
        var url = new Url().WithHost(host!);

        // Act
        var updated = url.WithAsciiHost();

        // Assert
        updated.ShouldBeSameAs(url);
    }

    [Theory]
    [InlineData("bücher.com", "xn--bcher-kva.com")]
    [InlineData("мойдомен.рф", "xn--d1acklchcc.xn--p1ai")]
    [InlineData("παράδειγμα.δοκιμή", "xn--hxajbheg2az3al.xn--jxalpdlp")]
    public void WithAsciiHost_ShouldReturnAsciiHostForIdnHost(string input, string expected)
    {
        // Arrange
        var url = new Url().WithHost(input);

        // Act
        var updated = url.WithAsciiHost();

        // Assert
        updated.Host.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData("example.com")]
    public void WithUnicodeHost_ShouldReturnSameInstanceWhenHostIsNotAnIdn(string? host)
    {
        // Arrange
        var url = new Url().WithHost(host!);

        // Act
        var updated = url.WithUnicodeHost();

        // Assert
        updated.ShouldBeSameAs(url);
    }

    [Theory]
    [InlineData("xn--bcher-kva.com", "bücher.com")]
    [InlineData("xn--d1acklchcc.xn--p1ai", "мойдомен.рф")]
    [InlineData("xn--hxajbheg2az3al.xn--jxalpdlp", "παράδειγμα.δοκιμή")]
    public void WithUnicodeHost_ShouldReturnAsciiHostForIdnHost(string input, string expected)
    {
        // Arrange
        var url = new Url().WithHost(input);

        // Act
        var updated = url.WithUnicodeHost();

        // Assert
        updated.Host.ShouldBe(expected);
    }

    #endregion

    #region Port

    [Fact]
    public void WithPort_ShouldUpdatePort()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithPort(8080);

        // Assert
        url.Port.ShouldBe(8080);
        url.ToString().ShouldBe("https://example.com:8080");
    }

    [Fact]
    public void ClearPort_ShouldRemovePort()
    {
        // Arrange
        var url = Url.Parse("https://example.com:80");

        // Act
        _ = url.ClearPort();

        // Assert
        url.Port.ShouldBeNull();
        url.ToString().ShouldBe("https://example.com");
    }

    #endregion

    #region Path

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void AddPathSegment_ShouldNotModifyTheUrlWhenGivenNullOrEmptyPath(string? path)
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegment(path!);

        // Assert
        url.PathSegments.Count.ShouldBe(0);
        url.ToString().ShouldBe("https://example.com");
    }

    [Fact]
    public void AddPathSegment_ShouldAddPathSegment()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegment("test");

        // Assert
        url.PathSegments.Count.ShouldBe(1);
        url.PathSegments[0].ShouldBe("test");
        url.ToString().ShouldBe("https://example.com/test");
    }

    [Fact]
    public void AddPathSegment_ShouldAddMultiplePathSegments()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegment("test/segment");

        // Assert
        url.PathSegments.Count.ShouldBe(2);
        url.PathSegments[0].ShouldBe("test");
        url.PathSegments[1].ShouldBe("segment");
        url.ToString().ShouldBe("https://example.com/test/segment");
    }

    [Fact]
    public void AddPathSegment_ShouldEncodePathSegment()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegment("test segment");

        // Assert
        url.PathSegments.Count.ShouldBe(1);
        url.PathSegments[0].ShouldBe("test%20segment");
        url.ToString().ShouldBe("https://example.com/test%20segment");
    }

    [Fact]
    public void AddPathSegments_ShouldNotModifyTheUrlWhenGivenEmptySegments()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegments(Array.Empty<string>());

        // Assert
        url.PathSegments.Count.ShouldBe(0);
        url.ToString().ShouldBe("https://example.com");
    }

    [Fact]
    public void AddPathSegments_ShouldAddAllSegments()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddPathSegments("test", "segment");

        // Assert
        url.PathSegments.Count.ShouldBe(2);
        url.PathSegments[0].ShouldBe("test");
        url.PathSegments[1].ShouldBe("segment");
        url.ToString().ShouldBe("https://example.com/test/segment");
    }

    [Fact]
    public void PopPathSegment_ShouldReturnNullWhenPathIsEmpty()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        var result = url.PopPathSegment();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void PopPathSegment_ShouldReturnAndRemoveLastSegment()
    {
        // Arrange
        var url = Url.Parse("https://example.com/test/segment");

        // Act
        var result = url.PopPathSegment();

        // Assert
        result.ShouldBe("segment");
        url.PathSegments.Count.ShouldBe(1);
        url.PathSegments[0].ShouldBe("test");
        url.ToString().ShouldBe("https://example.com/test");
    }

    [Fact]
    public void ClearPath_ShouldRemoveAllSegments()
    {
        // Arrange
        var url = Url.Parse("https://example.com/test/segment");

        // Act
        _ = url.ClearPath();

        // Assert
        url.PathSegments.Count.ShouldBe(0);
        url.ToString().ShouldBe("https://example.com");
    }

    #endregion

    #region Query

    [Fact]
    public void AddQueryParam_ShouldUpdateQueryParams()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.AddQueryParam("test", "true").AddQueryParam("tester");

        // Assert
        url.QueryParams.Count.ShouldBe(2);
        url.QueryParams["test"].ShouldBe("true");
        url.ToString().ShouldBe("https://example.com?test=true&tester");
    }

    [Fact]
    public void SetQueryParam_ShouldAddParamIfDoesNotExist()
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.SetQueryParam("test", "true").SetQueryParam("tester");

        // Assert
        url.QueryParams.Count.ShouldBe(2);
        url.QueryParams["test"].ShouldBe("true");
        url.ToString().ShouldBe("https://example.com?test=true&tester");
    }

    [Fact]
    public void SetQueryParam_ShouldReplaceExistingParam()
    {
        // Arrange
        var url = Url.Parse("https://example.com?test=false");

        // Act
        _ = url.SetQueryParam("test", "true");

        // Assert
        url.QueryParams.Count.ShouldBe(1);
        url.QueryParams["test"].ShouldBe("true");
        url.ToString().ShouldBe("https://example.com?test=true");
    }

    [Fact]
    public void RemoveQueryParam_ShouldRemoveExistingParam()
    {
        // Arrange
        var url = Url.Parse("https://example.com?test=false&tester");

        // Act
        _ = url.RemoveQueryParam("test");

        // Assert
        url.QueryParams.Count.ShouldBe(1);
        url.ToString().ShouldBe("https://example.com?tester");
    }

    [Fact]
    public void RemoveQueryParams_ShouldRemoveAllExistingQueryParams()
    {
        // Arrange
        var url = Url.Parse("https://example.com?test=false&tester");

        // Act
        _ = url.RemoveQueryParams("test", "tester", "notexisting");

        // Assert
        url.QueryParams.Count.ShouldBe(0);
        url.ToString().ShouldBe("https://example.com");
    }

    [Fact]
    public void ClearQuery_ShouldRemoveAllQueryParams()
    {
        // Arrange
        var url = Url.Parse("https://example.com?test=false&tester");

        // Act
        _ = url.ClearQuery();

        // Assert
        url.QueryParams.Count.ShouldBe(0);
        url.ToString().ShouldBe("https://example.com");
    }

    #endregion

    #region Fragment

    [Theory]
    [InlineData(null, "", "https://example.com")]
    [InlineData("", "", "https://example.com")]
    [InlineData("      ", "", "https://example.com")]
    [InlineData("test", "test", "https://example.com#test")]
    public void WithFragment_ShouldUpdateFragment(string? input, string expectedFragment, string expectedUrl)
    {
        // Arrange
        var url = Url.Parse("https://example.com");

        // Act
        _ = url.WithFragment(input);

        // Assert
        url.Fragment.ShouldBe(expectedFragment);
        url.ToString().ShouldBe(expectedUrl);
    }

    #endregion
}