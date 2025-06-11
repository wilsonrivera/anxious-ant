namespace AnxiousAnt;

partial class UrlTests
{

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void Parse_ShouldThrowWhenGivenNullEmptyOrWhiteSpace(string? input)
    {
        // Act
        var act = () => Url.Parse(input);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void TryParse_ShouldReturnFalseWhenGivenNullEmptyOrWhiteSpace(string? input)
    {
        // Act
        var result = Url.TryParse(input, out var url);

        // Assert
        result.ShouldBeFalse();
        url.ShouldBeNull();
    }

    [Theory]
    [InlineData("http://www.trailing-slash.com/", "/")]
    [InlineData("http://www.trailing-slash.com/a/b/", "/a/b/")]
    [InlineData("http://www.trailing-slash.com/a/b/?x=y", "/a/b/")]
    [InlineData("http://www.no-trailing-slash.com", "")]
    [InlineData("http://www.no-trailing-slash.com/a/b", "/a/b")]
    [InlineData("http://www.no-trailing-slash.com/a/b?x=y", "/a/b")]
    public void TryParse_PathRetainsTrailingSlash(string input, string expected)
    {
        // Act
        var result = Url.TryParse(input, out var url);

        // Assert
        result.ShouldBeTrue();
        url!.ToString().ShouldBe(input);
        url.Path.ShouldBe(expected);
    }

    [Theory]
    // // relative
    [InlineData("//relative/with/authority", "", "", "relative", null, "/with/authority", "", "")]
    [InlineData("/relative/without/authority", "", "", "", null, "/relative/without/authority", "", "")]
    [InlineData("relative/without/path/anchor", "", "", "", null, "relative/without/path/anchor", "", "")]
    // // absolute
    [InlineData(
        "http://www.mysite.com/with/path?x=1",
        "http",
        "",
        "www.mysite.com",
        null,
        "/with/path",
        "?x=1",
        ""
    )]
    [InlineData(
        "https://www.mysite.com/with/path?x=1#foo",
        "https",
        "",
        "www.mysite.com",
        null,
        "/with/path",
        "?x=1",
        "foo"
    )]
    [InlineData(
        "http://user:pass@www.mysite.com:8080/with/path?x=1?y=2",
        "http",
        "user:pass",
        "www.mysite.com",
        8080,
        "/with/path",
        "?x=1?y=2",
        ""
    )]
    [InlineData(
        "http://www.mysite.com/#with/path?x=1?y=2",
        "http",
        "",
        "www.mysite.com",
        null,
        "/",
        "",
        "with/path?x=1?y=2"
    )]
    // from https://en.wikipedia.org/wiki/Uniform_Resource_Identifier#Examples
    [InlineData(
        "https://john.doe@www.example.com:123/forum/questions/?tag=networking&order=newest#top",
        "https",
        "john.doe",
        "www.example.com",
        123, "/forum/questions/",
        "?tag=networking&order=newest",
        "top"
    )]
    [InlineData(
        "ldap://[2001:db8::7]/c=GB?objectClass?one",
        "ldap",
        "",
        "[2001:db8::7]",
        null,
        "/c=GB",
        "?objectClass?one",
        ""
    )]
    [InlineData("mailto:John.Doe@example.com", "mailto", "", "", null, "John.Doe@example.com", "", "")]
    [InlineData(
        "news:comp.infosystems.www.servers.unix",
        "news",
        "",
        "",
        null,
        "comp.infosystems.www.servers.unix",
        "",
        ""
    )]
    [InlineData("tel:+1-816-555-1212", "tel", "", "", null, "+1-816-555-1212", "", "")]
    [InlineData("telnet://192.0.2.16:80/", "telnet", "", "192.0.2.16", 80, "/", "", "")]
    [InlineData(
        "urn:oasis:names:specification:docbook:dtd:xml:4.1.2",
        "urn",
        "",
        "",
        null,
        "oasis:names:specification:docbook:dtd:xml:4.1.2", "", ""
    )]
    public void TryParse_ShouldParseUrlParts(
        string input,
        string scheme,
        string userInfo,
        string host,
        int? port,
        string path,
        string query,
        string fragment)
    {
        // Act
        var result = Url.TryParse(input, out var url);

        // Assert
        result.ShouldBeTrue();
        url.ShouldNotBeNull();

        url.Scheme.ShouldBe(scheme);
        url.UserInfo.ShouldBe(userInfo);
        url.Host.ShouldBe(host);
        url.Port.ShouldBe(port);
        url.Path.ShouldBe(path);
        url.QueryParams.ToString().ShouldBe(query);
        url.Fragment.ShouldBe(fragment);
    }
}