namespace AnxiousAnt;

/// <summary>
/// Provides a generic singleton container for managing shared instances of a type.
/// This class is a simple utility to store and retrieve a single instance of a generic type.
/// </summary>
/// <typeparam name="T">The type of the object to be managed as a singleton instance.</typeparam>
public static class Singleton<T>
{
    private static T? s_instance;

    /// <summary>
    /// Gets a value indicating whether the singleton instance has been set.
    /// </summary>
    /// <value>
    /// <c>true</c> if the singleton instance has been set; otherwise, <c>false</c>.
    /// </value>
    public static bool Exists
    {
        [MethodImpl(MethodImplOptions.Synchronized)] get => s_instance is not null;
    }

    /// <summary>
    /// Gets the singleton instance of the specified type.
    /// </summary>
    /// <value>
    /// The singleton instance if it has been set; otherwise, an exception is thrown if the instance is accessed without being set.
    /// </value>
    /// <exception cref="InvalidOperationException">Thrown when attempting to access the instance before it has been set.</exception>
    public static T Instance
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => s_instance ?? throw new InvalidOperationException("Singleton instance has not been set.");
    }

    /// <summary>
    /// Replaces the current singleton instance with the provided value.
    /// </summary>
    /// <param name="value">The new value to set as the singleton instance. Cannot be null.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void ReplaceWith(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (s_instance is IDisposable disposable)
        {
            disposable.Dispose();
        }

        s_instance = value;
    }

    /// <summary>
    /// Clears the current singleton instance, resetting it to its default value.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Clear()
    {
        s_instance = default;
    }

    /// <summary>
    /// Attempts to retrieve the current singleton instance if it exists.
    /// </summary>
    /// <param name="result">When this method returns, contains the instance if it exists; otherwise, the
    /// default value of the type.</param>
    /// <returns>
    /// <c>true</c> if the singleton instance exists; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static bool TryGet([MaybeNullWhen(false)] out T result)
    {
        result = default;
        if (s_instance is not { } instance)
        {
            return false;
        }

        result = instance;
        return true;
    }

    /// <summary>
    /// Retrieves the current singleton instance if it exists, or sets it to the provided default value and returns it.
    /// </summary>
    /// <param name="defaultValue">The value to set as the singleton instance if it does not already exist.</param>
    /// <returns>
    /// The existing singleton instance if it exists, otherwise the newly set default value.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static T GetOrSet(T defaultValue)
    {
        ArgumentNullException.ThrowIfNull(defaultValue);
        if (s_instance is { } result)
        {
            return result;
        }

        s_instance = defaultValue;
        return defaultValue;
    }

    /// <summary>
    /// Retrieves the singleton instance if it exists, or creates and sets a new instance using the provided
    /// factory method.
    /// </summary>
    /// <param name="factory">The function used to create a new instance if the singleton does not exist.</param>
    /// <returns>
    /// The existing singleton instance if it exists; otherwise, the newly created instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static T GetOrCreate(Func<T> factory) => GetOrCreate(factory, out _);

    /// <summary>
    /// Retrieves the singleton instance if it exists, or creates and sets a new instance using the provided
    /// factory method.
    /// Additionally, indicates whether a new instance was created.
    /// </summary>
    /// <param name="factory">The function used to create a new instance if the singleton does not exist.</param>
    /// <param name="created">A boolean value that is set to <c>true</c> if a new instance was created; otherwise,
    /// <c>false</c>.</param>
    /// <returns>
    /// The existing singleton instance if it exists; otherwise, the newly created instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static T GetOrCreate(Func<T> factory, out bool created)
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (s_instance is { } result)
        {
            created = false;
            return result;
        }

        s_instance = result = factory();
        created = true;
        return result;
    }
}