using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration;

/// <summary>
///     Extensions for use with configuration conventions.
/// </summary>
[PublicAPI]
public static class ConfigurationOptionsExtensions
{
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
            // We add in reverse order to keep the same order going in.
            foreach (var newSource in createSourceFrom.Reverse())
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
        var source = getSource(builder.Sources);
        if (source != null)
        {
            var index = builder.Sources.IndexOf(source);
            builder.Sources.RemoveAt(index);
            // We add in reverse order to keep the same order going in.
            foreach (var newSource in createSourceFrom.Reverse())
            {
                builder.Sources.Insert(index, newSource);
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
}
