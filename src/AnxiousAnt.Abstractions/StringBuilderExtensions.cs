namespace AnxiousAnt;

/// <summary>
/// Provides extension methods for the <see cref="StringBuilder"/> class.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends the specified string to the <see cref="StringBuilder"/> if its length is greater than zero.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> instance to append to. Must not be null.</param>
    /// <param name="value">The string to append if the <see cref="StringBuilder"/> is not empty. Can be null or empty.</param>
    /// <returns>
    /// The original <see cref="StringBuilder"/> instance after the operation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendIfNotEmpty(this StringBuilder builder, string? value)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrEmpty(value))
        {
            return builder;
        }

        if (builder.Length > 0)
        {
            builder.Append(value);
        }

        return builder;
    }

    /// <summary>
    /// Appends the specified character to the <see cref="StringBuilder"/> if its length is greater than zero.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> instance to append to. Must not be null.</param>
    /// <param name="value">The character to append if the <see cref="StringBuilder"/> is not empty.</param>
    /// <returns>
    /// The original <see cref="StringBuilder"/> instance after the operation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendIfNotEmpty(this StringBuilder builder, char value)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (builder.Length > 0)
        {
            builder.Append(value);
        }

        return builder;
    }
}