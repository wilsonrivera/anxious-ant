namespace AnxiousAnt.Reflection;

/// <summary>
/// Provides extension methods related to reflection operations.
/// </summary>
public static class ReflectionExtensions
{
    private static readonly Type NullableType = typeof(Nullable<>);

    /// <summary>
    /// Determines whether the specified type is nullable.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns>
    /// <c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullable(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return type is { IsValueType: false, IsEnum: false } ||
               type.IsGenericType && type.GetGenericTypeDefinition() == NullableType;
    }
}