using System.Reflection;

namespace AnxiousAnt.Reflection;

/// <summary>
/// Provides access to the types loaded in the current application domain.
/// </summary>
[ExcludeFromCodeCoverage]
[RequiresUnreferencedCode("Types might be removed")]
public static class AppDomainTypes
{
    private static readonly Lazy<IReadOnlyCollection<Type>> LazyCurrent = new(static () => AppDomain.CurrentDomain
        .GetAssemblies()
        .SelectMany(static assembly =>
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // If some types fail to load, we still return the ones that did load
                return ex.Types.Where(static t => t != null);
            }
        })
        .Select(static type => type!)
        .ToArray()
    );

    /// <summary>
    /// Gets the types loaded in the current application domain.
    /// </summary>
    public static IReadOnlyCollection<Type> Current => LazyCurrent.Value;
}