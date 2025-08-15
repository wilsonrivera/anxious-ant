namespace AnxiousAnt;

/// <summary>
/// Provides extensions for the <see cref="Nullable{T}"/> struct.
/// </summary>
public static class NullableExtensions
{
    /// <summary>
    /// Attempts to get value from nullable container.
    /// </summary>
    /// <typeparam name="T">The underlying value type of the nullable type.</typeparam>
    /// <param name="nullable">Nullable value.</param>
    /// <param name="value">Underlying value.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="nullable"/> is not <c>null</c>; otherwise, <c>false</c>.</returns>
    public static bool TryGetValue<T>(ref readonly this T? nullable, out T value)
        where T : struct
    {
        value = Nullable.GetValueRefOrDefaultRef(in nullable);
        return nullable.HasValue;
    }
}