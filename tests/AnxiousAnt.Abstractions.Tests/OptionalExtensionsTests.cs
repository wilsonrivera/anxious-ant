namespace AnxiousAnt;

public class OptionalExtensionsTests
{
    [Fact]
    public void ValueOrNull_ShouldReturnNullWhenDefaultOrEmpty()
    {
        default(Optional<int>).ValueOrNull().ShouldBeNull();
        new Optional<int>().ValueOrNull().ShouldBeNull();
    }

    [Fact]
    public void ValueOrNull_ShouldReturnValueWhenNotEmpty()
    {
        // Arrange
        var optional = new Optional<int>(42);

        // Assert
        optional.ValueOrNull().ShouldBe(42);
    }

    [Fact]
    public void GetHashCode_SmokeTest()
    {
        // Arrange
        const string input = "hello world";
        var optional = new Optional<string>(input);

        // Assert
        default(Optional<string>).GetHashCode(StringComparison.Ordinal).ShouldBe(Optional.UninitializedValueKind);
        new Optional<string>().GetHashCode(StringComparison.Ordinal).ShouldBe(Optional.EmptyValueKind);
        new Optional<string>(null).GetHashCode(StringComparison.Ordinal).ShouldBe(Optional.NullValueKind);
        optional.GetHashCode(StringComparison.Ordinal).ShouldBe(input.GetHashCode(StringComparison.Ordinal));
        optional.GetHashCode(StringComparison.OrdinalIgnoreCase)
            .ShouldBe(input.GetHashCode(StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AsSpan_ShouldReturnEmptySpanWhenEmpty()
    {
        default(Optional<string>).AsSpan().IsEmpty.ShouldBeTrue();
        new Optional<string>().AsSpan().IsEmpty.ShouldBeTrue();
        new Optional<string>(null).AsSpan().IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void AsSpan_ShouldReturnSpanWithContentWhenNotEmpty()
    {
        // Arrange
        const string input = "test";
        var optional = new Optional<string>(input);

        // Act
        var span = optional.AsSpan();

        // Assert
        span.Length.ShouldBe(input.Length);
        span.ToString().ShouldBe(input);
    }
}