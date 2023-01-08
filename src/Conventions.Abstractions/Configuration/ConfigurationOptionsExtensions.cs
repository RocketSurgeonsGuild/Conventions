using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Extensions for use with configuration conventions.
/// </summary>
public static class ConfigurationOptionsExtensions
{
    /// <summary>
    ///     Append an application configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    [Obsolete("Use AppendApplicationConfiguration instead")]
    public static IConventionContext AddApplicationConfiguration(this IConventionContext context, ConfigurationBuilderApplicationDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new());
        delegates.Add(@delegate);
        return context;
    }

    /// <summary>
    ///     Append an environment configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    [Obsolete("Use AppendEnvironmentConfiguration instead")]
    public static IConventionContext AddEnvironmentConfiguration(this IConventionContext context, ConfigurationBuilderEnvironmentDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new());
        delegates.Add(@delegate);
        return context;
    }

    /// <summary>
    ///     Append an application configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public static IConventionContext AppendApplicationConfiguration(this IConventionContext context, ConfigurationBuilderApplicationDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new());
        delegates.Add(@delegate);
        return context;
    }

    /// <summary>
    ///     Append an environment configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public static IConventionContext AppendEnvironmentConfiguration(this IConventionContext context, ConfigurationBuilderEnvironmentDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new());
        delegates.Add(@delegate);
        return context;
    }

    /// <summary>
    ///     Prepend an application configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public static IConventionContext PrependApplicationConfiguration(this IConventionContext context, ConfigurationBuilderApplicationDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new());
        delegates.Insert(0, @delegate);
        return context;
    }

    /// <summary>
    ///     Prepend an environment configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public static IConventionContext PrependEnvironmentConfiguration(this IConventionContext context, ConfigurationBuilderEnvironmentDelegate @delegate)
    {
        var delegates = context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new());
        delegates.Insert(0, @delegate);
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
