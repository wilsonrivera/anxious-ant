namespace AnxiousAnt.Collections;

public class ReadOnlyKeyValueCollectionTests
{
    [Fact]
    public void ShouldBeEmpty()
    {
        // Arrange
        var collection = ReadOnlyKeyValueCollection<int>.Empty;

        // Assert
        collection.Count.ShouldBe(0);
        collection.ShouldBeEmpty();
    }
}