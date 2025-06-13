using System.Net.Sockets;

namespace AnxiousAnt.Reflection;

public class ReflectionExtensionsTests
{
    [Fact]
    public void IsNullable_ThrowsWhenGivenTypeIsNull()
    {
        // Arrange
        Action act = () => ReflectionExtensions.IsNullable(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(List<string>))]
    public void IsNullable_ShouldReturnTrueWhenGivenTypeIsReferenceType(Type type)
    {
        // Assert
        type.IsNullable().ShouldBeTrue();
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(SocketFlags))]
    public void IsNullable_ShouldReturnFalseWhenGivenTypeIsValueType(Type type)
    {
        // Assert
        type.IsNullable().ShouldBeFalse();
    }

    [Theory]
    [InlineData(typeof(int?))]
    [InlineData(typeof(SocketFlags?))]
    public void IsNullable_ShouldReturnTrueWhenGivenTypeIsNullableValueType(Type type)
    {
        // Assert
        type.IsNullable().ShouldBeTrue();
    }
}