namespace AnxiousAnt;

public class OptionalOfTTests
{
    [Fact]
    public void DefaultCtor_ShouldNotBeInitialized()
    {
        // Arrange
        Optional<int> optional = default;
        Action act = () => _ = optional.Value;
        Action act2 = () => _ = optional.ValueRef;

        // Assert
        optional.ValueKind.ShouldBe(Optional.UninitializedValueKind);
        optional.IsDefault.ShouldBeTrue();
        optional.IsEmpty.ShouldBeTrue();
        optional.IsDefaultOrEmpty.ShouldBeTrue();
        optional.ToString().ShouldBe("Uninitialized");
        optional.ValueOr(42).ShouldBe(42);

        act.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Ctor_WithoutParameters_ShouldBeEmpty()
    {
        // Arrange
        Optional<int> optional = new();
        Action act = () => _ = optional.Value;
        Action act2 = () => _ = optional.ValueRef;

        // Assert
        optional.ValueKind.ShouldBe(Optional.EmptyValueKind);
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
        optional.IsDefaultOrEmpty.ShouldBeTrue();
        optional.ToString().ShouldBe("None");
        optional.ValueOr(42).ShouldBe(42);

        act.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Ctor_WithNull_ShouldBeEmpty()
    {
        // Arrange
        Optional<string> optional = new(null);
        Action act = () => _ = optional.Value;
        Action act2 = () => _ = optional.ValueRef;

        // Assert
        optional.ValueKind.ShouldBe(Optional.NullValueKind);
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
        optional.IsDefaultOrEmpty.ShouldBeTrue();
        optional.ToString().ShouldBe("None");
        optional.TryGetValue(out _).ShouldBeFalse();
        optional.ValueOr("test").ShouldBe("test");

        act.ShouldThrow<InvalidOperationException>();
        act2.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void Ctor_WithValue_ShouldNotBeEmptyWhenGivenValue()
    {
        // Arrange
        Optional<string> optional = new("test");

        // Assert
        optional.ValueKind.ShouldBe(Optional.NotEmptyValueKind);
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeFalse();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.ToString().ShouldBe("Some(test)");
        optional.ValueOr("test2").ShouldBe("test");
        optional.Value.ShouldBe("test");
        optional.ValueRef.ShouldBe("test");
        optional.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe("test");
    }

    [Fact]
    public void Ctor_WithOptional_ShouldBeSameValueKind()
    {
        Optional<Optional<int>> optional = default;
        new Optional<int>(in optional).ValueKind.ShouldBe(Optional.UninitializedValueKind);

        optional = new Optional<Optional<int>>();
        new Optional<int>(in optional).ValueKind.ShouldBe(Optional.EmptyValueKind);

        optional = new Optional<Optional<int>>(default);
        new Optional<int>(in optional).ValueKind.ShouldBe(Optional.UninitializedValueKind);

        optional = new Optional<Optional<int>>(new Optional<int>());
        new Optional<int>(in optional).ValueKind.ShouldBe(Optional.EmptyValueKind);

        Optional<Optional<string>> optional2 = new(new Optional<string>(null));
        new Optional<string>(in optional2).ValueKind.ShouldBe(Optional.NullValueKind);
    }

    [Fact]
    public void Ctor_WithOptional_ShouldNotBeEmptyWhenGivenNonEmptyOptional()
    {
        // Arrange
        Optional<Optional<int>> input = new(new Optional<int>(42));

        // Act
        var optional = new Optional<int>(in input);

        // Assert
        optional.ValueKind.ShouldBe(Optional.NotEmptyValueKind);
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeFalse();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void GetHashCode_SmokeTest()
    {
        // Arrange
        const string input = "hello world";

        // Assert
        default(Optional<int>).GetHashCode().ShouldBe(Optional.UninitializedValueKind);
        new Optional<int>().GetHashCode().ShouldBe(Optional.EmptyValueKind);
        new Optional<string>(null).GetHashCode().ShouldBe(Optional.NullValueKind);
        new Optional<string>(input).GetHashCode().ShouldBe(input.GetHashCode());
    }

    [Fact]
    public void Equals_Object_ShouldReturnFalseWhenGivenUnsupportedType()
    {
        var optional = new Optional<int>(42);

        // ReSharper disable once SuspiciousTypeConversion.Global
        optional.Equals("different").ShouldBeFalse();
    }

    [Fact]
    public void Equals_Object_WithNull_ShouldReturnTrueWhenValueKindIsNull()
    {
        // Arrange
        var optional = new Optional<string>(null);

        // Assert
        optional.Equals((object?)null).ShouldBeTrue();
    }


    [Fact]
    public void Equals_Object_WithNull_ShouldReturnFalseWhenValueKindIsNotNull()
    {
        default(Optional<string>).Equals((object?)null).ShouldBeFalse();
        new Optional<string>().Equals((object?)null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Object_WithOptional_ShouldReturnFalseWhenGivenDifferentKind()
    {
        default(Optional<int>).Equals((object)new Optional<int>()).ShouldBeFalse();
        default(Optional<string>).Equals((object)new Optional<string>(null)).ShouldBeFalse();
        default(Optional<int>).Equals((object)new Optional<int>(42)).ShouldBeFalse();
        new Optional<int>().Equals((object)new Optional<int>(42)).ShouldBeFalse();
        new Optional<string>().Equals((object)new Optional<string>(null)).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Object_WithOptional_ShouldReturnTrueWhenGivenSameKindAndValue()
    {
        default(Optional<int>).Equals((object)default(Optional<int>)).ShouldBeTrue();
        new Optional<int>().Equals((object)new Optional<int>()).ShouldBeTrue();
        new Optional<string>(null).Equals((object)new Optional<string>(null)).ShouldBeTrue();
        new Optional<int>(42).Equals((object)new Optional<int>(42)).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Object_WithOptional_ShouldReturnFalseWhenGivenSameKindAndDifferentValue()
    {
        // Arrange
        var left = new Optional<int>(42);
        var right = new Optional<int>(100);

        // Assert
        left.Equals((object)right).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Object_WithValue_ShouldReturnTrueWhenGivenSameValue()
    {
        // Arrange
        var optional = new Optional<int>(42);

        // Assert
        // ReSharper disable once SuspiciousTypeConversion.Global
        optional.Equals((object)42).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Object_WithValue_ShouldReturnFalseWhenGivenDifferentValue()
    {
        // Arrange
        var optional = new Optional<int>(42);

        // Assert
        // ReSharper disable once SuspiciousTypeConversion.Global
        optional.Equals((object)100).ShouldBeFalse();
    }

    [Fact]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public void Equals_Object_WithValue_ShouldReturnFalseWhenEmpty()
    {
        default(Optional<int>).Equals((object)42).ShouldBeFalse();
        new Optional<int>().Equals((object)42).ShouldBeFalse();
        new Optional<string>(null).Equals((object)"test").ShouldBeFalse();
    }

    [Fact]
    public void Equals_Optional_ShouldReturnFalseWhenGivenDifferentKind()
    {
        default(Optional<int>).Equals(new Optional<int>()).ShouldBeFalse();
        default(Optional<string>).Equals(new Optional<string>(null)).ShouldBeFalse();
        default(Optional<int>).Equals(new Optional<int>(42)).ShouldBeFalse();
        new Optional<int>().Equals(new Optional<int>(42)).ShouldBeFalse();
        new Optional<string>().Equals(new Optional<string>(null)).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Optional_ShouldReturnTrueWhenGivenSameKindAndValue()
    {
        default(Optional<int>).Equals(default(Optional<int>)).ShouldBeTrue();
        new Optional<int>().Equals(new Optional<int>()).ShouldBeTrue();
        new Optional<string>(null).Equals(new Optional<string>(null)).ShouldBeTrue();
        new Optional<int>(42).Equals(new Optional<int>(42)).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Optional_ShouldReturnFalseWhenGivenSameKindAndDifferentValue()
    {
        // Arrange
        var left = new Optional<int>(42);
        var right = new Optional<int>(100);

        // Assert
        left.Equals(right).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Value_WithNull_ShouldReturnTrueWhenValueKindIsNull()
    {
        // Arrange
        var optional = new Optional<string>(null);

        // Assert
        optional.Equals(null).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Value_WithNull_ShouldReturnFalseWhenValueKindIsNotNull()
    {
        // Arrange
        var optional = new Optional<string>("test");

        // Assert
        optional.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Value_ShouldReturnTrueWhenGivenSameValue()
    {
        // Arrange
        var optional = new Optional<int>(42);

        // Assert
        optional.Equals(42).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Value_ShouldReturnFalseWhenGivenDifferentValue()
    {
        // Arrange
        var optional = new Optional<int>(42);

        // Assert
        optional.Equals(100).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Value_ShouldReturnFalseWhenEmpty()
    {
        default(Optional<int>).Equals(42).ShouldBeFalse();
        new Optional<int>().Equals(42).ShouldBeFalse();
        new Optional<string>(null).Equals("test").ShouldBeFalse();
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalseWhenDefaultOrEmpty()
    {
        default(Optional<int>).TryGetValue(out _).ShouldBeFalse();
        new Optional<int>().TryGetValue(out _).ShouldBeFalse();
        new Optional<string>(null).TryGetValue(out _).ShouldBeFalse();
    }

    [Fact]
    public void Or_WithAlternative_ShouldReturnAlternativeWhenUninitialized()
    {
        // Arrange
        Optional<int> input = default;

        // Act
        var optional = input.Or(42);

        // Assert
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithAlternative_ShouldReturnAlternativeWhenEmpty()
    {
        // Arrange
        Optional<int> input = new();

        // Act
        var optional = input.Or(42);

        // Assert
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithAlternative_ShouldReturnAlternativeWhenNull()
    {
        // Arrange
        Optional<string> input = new(null);

        // Act
        var optional = input.Or("test");

        // Assert
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public void Or_WithAlternative_ShouldReturnValueWhenNotEmpty()
    {
        // Arrange
        Optional<int> input = new(42);

        // Act
        var optional = input.Or(100);

        // Assert
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithFactory_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        Optional<int> optional = default;
        Action act = () => _ = optional.Or(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Or_WithFactory_ShouldReturnAlternativeWhenUninitialized()
    {
        // Arrange
        Optional<int> input = default;
        var fakeFactory = A.Fake<Func<int>>();
        A.CallTo(() => fakeFactory()).Returns(42);

        // Act
        var optional = input.Or(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithFactory_ShouldReturnAlternativeWhenEmpty()
    {
        // Arrange
        Optional<int> input = new();
        var fakeFactory = A.Fake<Func<int>>();
        A.CallTo(() => fakeFactory()).Returns(42);

        // Act
        var optional = input.Or(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Or_WithFactory_ShouldReturnAlternativeWhenNull()
    {
        // Arrange
        Optional<string> input = new();
        var fakeFactory = A.Fake<Func<string>>();
        A.CallTo(() => fakeFactory()).Returns("test");

        // Act
        var optional = input.Or(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public void Or_WithFactory_ShouldNotCallFactoryWhenNotEmpty()
    {
        // Arrange
        Optional<int> input = new(42);
        var fakeFactory = A.Fake<Func<int>>();

        // Act
        var optional = input.Or(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustNotHaveHappened();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
    }

    [Fact]
    public async Task OrAsync_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        Optional<int> optional = default;
        Func<Task> act = () => optional.OrAsync(null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrAsync_ShouldReturnAlternativeWhenUninitialized()
    {
        // Arrange
        Optional<int> input = default;
        var fakeFactory = A.Fake<Func<CancellationToken, Task<int>>>();
        A.CallTo(() => fakeFactory(A<CancellationToken>._)).Returns(42);

        // Act
        var optional = await input.OrAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrAsync_ShouldReturnAlternativeWhenEmpty()
    {
        // Arrange
        Optional<int> input = new();
        var fakeFactory = A.Fake<Func<CancellationToken, Task<int>>>();
        A.CallTo(() => fakeFactory(A<CancellationToken>._)).Returns(42);

        // Act
        var optional = await input.OrAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrAsync_ShouldReturnAlternativeWhenNull()
    {
        // Arrange
        Optional<string> input = new();
        var fakeFactory = A.Fake<Func<CancellationToken, Task<string?>>>();
        A.CallTo(() => fakeFactory(A<CancellationToken>._)).Returns("test");

        // Act
        var optional = await input.OrAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public async Task OrAsync_ShouldNotCallFactoryWhenNotEmpty()
    {
        // Arrange
        Optional<int> input = new(42);
        var fakeFactory = A.Fake<Func<CancellationToken, Task<int>>>();

        // Act
        var optional = await input.OrAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustNotHaveHappened();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
    }

    [Fact]
    public void OrElse_WithAlternative_ShouldReturnAlternativeWhenUninitialized()
    {
        // Arrange
        Optional<int> optional = default;

        // Assert
        optional.OrElse(new Optional<int>(42)).Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithAlternative_ShouldReturnAlternativeWhenEmpty()
    {
        // Arrange
        Optional<int> optional = new();

        // Assert
        optional.OrElse(new Optional<int>(42)).Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithAlternative_ShouldReturnAlternativeWhenNull()
    {
        // Arrange
        Optional<string> optional = new(null);

        // Assert
        optional.OrElse(new Optional<string>("test")).Value.ShouldBe("test");
    }

    [Fact]
    public void OrElse_WithAlternative_ShouldReturnValueWhenNotEmpty()
    {
        // Arrange
        Optional<int> optional = new(42);

        // Assert
        optional.OrElse(new Optional<int>(100)).Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithFactory_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        Optional<int> optional = default;
        Action act = () => _ = optional.OrElse(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void OrElse_WithFactory_ShouldReturnAlternativeWhenUninitialized()
    {
        // Arrange
        Optional<int> input = default;
        var fakeFactory = A.Fake<Func<Optional<int>>>();
        A.CallTo(() => fakeFactory()).Returns(new Optional<int>(42));

        // Act
        var optional = input.OrElse(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithFactory_ShouldReturnAlternativeWhenEmpty()
    {
        // Arrange
        Optional<int> input = new();
        var fakeFactory = A.Fake<Func<Optional<int>>>();
        A.CallTo(() => fakeFactory()).Returns(new Optional<int>(42));

        // Act
        var optional = input.OrElse(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void OrElse_WithFactory_ShouldReturnAlternativeWhenNull()
    {
        // Arrange
        Optional<string> input = new();
        var fakeFactory = A.Fake<Func<Optional<string>>>();
        A.CallTo(() => fakeFactory()).Returns(new Optional<string>("test"));

        // Act
        var optional = input.OrElse(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public void OrElse_WithFactory_ShouldNotCallFactoryWhenNotEmpty()
    {
        // Arrange
        Optional<int> input = new(42);
        var fakeFactory = A.Fake<Func<Optional<int>>>();

        // Act
        var optional = input.OrElse(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustNotHaveHappened();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
    }

    [Fact]
    public async Task OrElseAsync_ShouldThrowWhenGivenNullFactory()
    {
        // Arrange
        Optional<int> optional = default;
        Func<Task> act = () => optional.OrElseAsync(null!).AsTask();

        // Assert
        await act.ShouldThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task OrElseAsync_ShouldReturnAlternativeWhenUninitialized()
    {
        // Arrange
        Optional<int> input = default;
        var fakeFactory = A.Fake<Func<CancellationToken, Task<Optional<int>>>>();
        A.CallTo(() => fakeFactory(A<CancellationToken>._)).Returns(new Optional<int>(42));

        // Act
        var optional = await input.OrElseAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrElseAsync_ShouldReturnAlternativeWhenEmpty()
    {
        // Arrange
        Optional<int> input = new();
        var fakeFactory = A.Fake<Func<CancellationToken, Task<Optional<int>>>>();
        A.CallTo(() => fakeFactory(A<CancellationToken>._)).Returns(new Optional<int>(42));

        // Act
        var optional = await input.OrElseAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task OrElseAsync_ShouldReturnAlternativeWhenNull()
    {
        // Arrange
        Optional<string> input = new();
        var fakeFactory = A.Fake<Func<CancellationToken, Task<Optional<string>>>>();
        A.CallTo(() => fakeFactory(A<CancellationToken>._)).Returns(new Optional<string>("test"));

        // Act
        var optional = await input.OrElseAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public async Task OrElseAsync_ShouldNotCallFactoryWhenNotEmpty()
    {
        // Arrange
        Optional<int> input = new(42);
        var fakeFactory = A.Fake<Func<CancellationToken, Task<Optional<int>>>>();

        // Act
        var optional = await input.OrElseAsync(fakeFactory);

        // Assert
        A.CallTo(fakeFactory).MustNotHaveHappened();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
    }

    [Fact]
    public void Filter_WithCondition_ShouldReturnValueWhenTrue()
    {
        // Arrange
        var input = new Optional<int>(42);

        // Act
        var optional = input.If(true);

        // Assert
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Filter_WithCondition_ShouldReturnEmptyWhenFalse()
    {
        // Arrange
        var input = new Optional<int>(42);

        // Act
        var optional = input.If(false);

        // Assert
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Filter_WithPredicate_ShouldThrowWhenGivenNullPredicate()
    {
        // Arrange
        Optional<int> optional = default;
        Action act = () => _ = optional.If(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Filter_WithPredicate_ShouldReturnValueWhenTrue()
    {
        // Arrange
        var input = new Optional<int>(42);
        var fakePredicate = A.Fake<Func<int, bool>>();
        A.CallTo(() => fakePredicate(A<int>._)).Returns(true);

        // Act
        var optional = input.If(fakePredicate);

        // Assert
        A.CallTo(fakePredicate).MustHaveHappenedOnceExactly();
        optional.IsDefaultOrEmpty.ShouldBeFalse();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Filter_WithPredicate_ShouldReturnEmptyWhenPredicateReturnsFalse()
    {
        // Arrange
        var input = new Optional<int>(42);
        var fakePredicate = A.Fake<Func<int, bool>>();
        A.CallTo(() => fakePredicate(A<int>._)).Returns(false);

        // Act
        var optional = input.If(fakePredicate);

        // Assert
        A.CallTo(fakePredicate).MustHaveHappenedOnceExactly();
        optional.IsDefault.ShouldBeFalse();
        optional.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void LogicalNotOperator_ShouldReturnTrueWhenDefaultOrEmpty()
    {
        (!default(Optional<int>)).ShouldBeTrue();
        (!new Optional<int>()).ShouldBeTrue();
        (!new Optional<string>(null)).ShouldBeTrue();
    }

    [Fact]
    public void LogicalNotOperator_ShouldReturnFalseWhenNotEmpty()
    {
        (!new Optional<int>(42)).ShouldBeFalse();
    }

    [Fact]
    public void TrueOperator_ShouldReturnFalseWhenDefaultOrEmpty()
    {
        if (default(Optional<int>))
        {
            throw new Exception("Default optional should not be true");
        }

        if (new Optional<int>())
        {
            throw new Exception("Empty optional should not be true");
        }

        if (new Optional<string>(null))
        {
            throw new Exception("Null optional should not be true");
        }
    }

    [Fact]
    public void TrueOperator_ShouldReturnTrueWhenNotEmpty()
    {
        if (new Optional<int>(42))
        {
            // ignore
        }
        else
        {
            throw new Exception("Non-empty optional should be true");
        }
    }

    [Fact]
    public void BitwiseAndOperator_ShouldReturnFalseWhenEitherIsEmptyOrDefault()
    {
        (default(Optional<int>) & new Optional<int>(42)).ShouldBeFalse();
        (new Optional<int>(42) & default(Optional<int>)).ShouldBeFalse();
        (new Optional<int>() & new Optional<int>(42)).ShouldBeFalse();
        (new Optional<int>(42) & new Optional<int>()).ShouldBeFalse();
        (new Optional<string>("hello") & new Optional<string>(null)).ShouldBeFalse();
        (new Optional<string>(null) & new Optional<string>("hello")).ShouldBeFalse();
    }

    [Fact]
    public void BitwiseAndOperator_ShouldReturnTrueWhenNeitherIsEmpty()
    {
        (new Optional<int>(42) & new Optional<int>(52)).ShouldBeTrue();
    }

    [Fact]
    public void BitwiseOrOperator_ShouldReturnSelfWhenNotDefaultOrEmpty()
    {
        // Arrange
        var input = new Optional<int>(42);

        // Act
        var result = input | new Optional<int>(52);

        // Assert
        result.IsDefaultOrEmpty.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void BitwiseOrOperator_ShouldReturnAlternativeWhenDefaultOrEmpty()
    {
        (default(Optional<int>) | new Optional<int>(42)).Value.ShouldBe(42);
        (new Optional<int>() | new Optional<int>(42)).Value.ShouldBe(42);
        (new Optional<string>(null) | new Optional<string>("hello")).Value.ShouldBe("hello");
    }
}