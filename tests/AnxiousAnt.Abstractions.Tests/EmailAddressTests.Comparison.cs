namespace AnxiousAnt;

partial class EmailAddressTests
{
    [Fact]
    public void Comparer_Roundtrip()
    {
        // Arrange
        var comparer = EmailAddress.Comparer;
        var email1 = EmailAddress.Parse("test@test.com");
        var email2 = EmailAddress.Parse("test@test.com");

        // Assert
        comparer.ShouldNotBeNull();
        comparer.Equals(email1, email2).ShouldBeTrue();
        comparer.GetHashCode(email1).ShouldBe(email1.GetHashCode());
    }

    [Fact]
    public void AddressOnlyComparer_Roundtrip()
    {
        // Arrange
        var comparer = EmailAddress.AddressOnlyComparer;
        var email1 = EmailAddress.Parse("Test <test@test.com>");
        var email2 = EmailAddress.Parse("Test2 <TEST@test.com>");
        var expectedHashCode = string.GetHashCode(email1.Address, StringComparison.OrdinalIgnoreCase);

        // Assert
        comparer.ShouldNotBeNull();
        comparer.Equals(email1, email2).ShouldBeTrue();
        comparer.GetHashCode(email1).ShouldBe(expectedHashCode);
    }
}