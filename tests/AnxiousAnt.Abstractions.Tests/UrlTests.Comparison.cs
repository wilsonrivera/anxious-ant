namespace AnxiousAnt;

partial class UrlTests
{
    [Fact]
    public void Comparer_Roundtrip()
    {
        // Arrange
        var comparer = Url.Comparer;
        var url1 = Url.Parse("https://example.com");
        var url2 = Url.Parse("https://example.com/");

        // Assert
        comparer.ShouldNotBeNull();
        comparer.Equals(url1, url2).ShouldBeTrue();
        comparer.GetHashCode(url1).ShouldBe(url1.GetHashCode());
    }
}