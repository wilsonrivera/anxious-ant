namespace AnxiousAnt;

public class MaybeTests
{
    [Fact]
    public void HasValue_ShouldBeFalse_WhenNone()
    {
        // Arrange & Act
        var maybe = Maybe<string>.None;

        // Assert
        maybe.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Value_ShouldThrowInvalidOperationException_WhenNone()
    {
        // Arrange
        var maybe = Maybe<string>.None;

        // Act
        Func<string> act = () => maybe.Value;

        // Assert
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void FromValue_ShouldCreateMaybe_WhenValueGiven()
    {
        // Arrange
        const string value = "TestValue";

        // Act
        var maybe = Maybe<string>.FromValue(value);

        // Assert
        maybe.HasValue.ShouldBeTrue();
        maybe.Value.ShouldBe(value);
    }

    [Fact]
    public void ImplicitConversion_ShouldCreateMaybe_FromValue()
    {
        // Arrange
        const string value = "TestValue";

        // Act
        Maybe<string> maybe = value;

        // Assert
        maybe.HasValue.ShouldBeTrue();
        maybe.Value.ShouldBe(value);
    }

    [Fact]
    public void ImplicitConversion_ShouldExtractValue_WhenMaybeHasValue()
    {
        // Arrange
        var maybe = Maybe<int>.FromValue(42);

        // Act
        int value = maybe;

        // Assert
        value.ShouldBe(42);
    }

    [Fact]
    public void GetHashCode_ShouldReturnExpectedHashCodeWhenNone()
    {
        // Arrange
        var expected = HashCode.Combine(false, (string?)null);

        // Act
        var maybe = Maybe<string>.None;

        // Assert
        maybe.GetHashCode().ShouldBe(expected);
    }

    [Fact]
    public void GetHashCode_ShouldReturnExpectedHashCodeWhenHasValue()
    {
        // Arrange
        var expected = HashCode.Combine(true, "abc123");

        // Act
        var maybe = Maybe<string>.FromValue("abc123");

        // Assert
        maybe.GetHashCode().ShouldBe(expected);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenBothAreNone()
    {
        // Arrange
        var maybe1 = Maybe<string>.None;
        var maybe2 = Maybe<string>.None;

        // Act
        var areEqual = maybe1.Equals(maybe2);

        // Assert
        areEqual.ShouldBeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOneIsNoneAndAnotherHasValue()
    {
        // Arrange
        var maybeWithValue = Maybe<int>.FromValue(5);
        var maybeNone = Maybe<int>.None;

        // Act
        var areEqual = maybeNone.Equals(maybeWithValue);

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenValuesAreDifferent()
    {
        // Arrange
        var maybe1 = Maybe<int>.FromValue(10);
        var maybe2 = Maybe<int>.FromValue(20);

        // Act
        var areEqual = maybe1.Equals(maybe2);

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenGivenObjectIsNotMaybe()
    {
        // Arrange
        var maybe1 = Maybe<int>.FromValue(10);

        // Act
        // ReSharper disable once SuspiciousTypeConversion.Global
        var areEqual = maybe1.Equals((object?)10);

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenValuesAreSame()
    {
        // Arrange
        var maybe1 = Maybe<string>.FromValue("Hello");
        var maybe2 = Maybe<string>.FromValue("Hello");

        // Act
        var areEqual = maybe1 == maybe2;

        // Assert
        areEqual.ShouldBeTrue();
    }

    [Fact]
    public void InequalityOperator_ShouldReturnTrue_WhenValuesAreDifferent()
    {
        // Arrange
        var maybe1 = Maybe<string>.FromValue("Hello");
        var maybe2 = Maybe<string>.FromValue("World");

        // Act
        var areNotEqual = maybe1 != maybe2;

        // Assert
        areNotEqual.ShouldBeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnEmptyString_WhenNone()
    {
        // Arrange
        var maybe = Maybe<double>.None;

        // Act
        var result = maybe.ToString();

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void ToString_ShouldReturnValueString_WhenHasValue()
    {
        // Arrange
        var maybe = Maybe<int>.FromValue(100);

        // Act
        var result = maybe.ToString();

        // Assert
        result.ShouldBe("100");
    }
}