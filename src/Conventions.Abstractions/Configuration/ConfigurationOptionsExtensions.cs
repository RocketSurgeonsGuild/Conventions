using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Extensions for use with configuration conventions.
/// </summary>
public static class ConfigurationOptionsExtensions
{
    /// <summary>
    ///     Add an application configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public static IConventionContext AddApplicationConfiguration(this IConventionContext context, ConfigurationBuilderApplicationDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new());
        delegates.Add(@delegate);
        return context;
    }

    /// <summary>
    ///     Add an environment configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public static IConventionContext AddEnvironmentConfiguration(this IConventionContext context, ConfigurationBuilderEnvironmentDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new());
        delegates.Add(@delegate);
        return context;
    }

    internal static void InsertConfigurationSourceAfter<T>(
        this IConfigurationBuilder builder,
        Func<IList<IConfigurationSource>, T?> getSource,
        IEnumerable<IConfigurationSource> createSourceFrom
    )
        where T : IConfigurationSource
    {
        var source = getSource(builder.Sources);
        if (source != null)
        {
            var index = builder.Sources.IndexOf(source);
            foreach (var newSource in createSourceFrom)
            {
                builder.Sources.Insert(index + 1, newSource);
            }
        }
        else
        {
            foreach (var newSource in createSourceFrom)
            {
                builder.Sources.Add(newSource);
            }
        }
    }

    internal static void ReplaceConfigurationSourceAt<T>(
        this IConfigurationBuilder builder,
        Func<IList<IConfigurationSource>, T?> getSource,
        IEnumerable<IConfigurationSource> createSourceFrom
    )
        where T : class, IConfigurationSource
    {
        var iConfigurationSources = createSourceFrom as IConfigurationSource[] ?? createSourceFrom.ToArray();
        if (iConfigurationSources.Length == 0)
            return;
        var source = getSource(builder.Sources);
        if (source != null)
        {
            var index = builder.Sources.IndexOf(source);
            builder.Sources.RemoveAt(index);
            foreach (var newSource in iConfigurationSources)
            {
                builder.Sources.Insert(index, newSource);
            }
        }
        else
        {
            foreach (var newSource in iConfigurationSources)
            {
                builder.Sources.Add(newSource);
            }
        }
    }
}
