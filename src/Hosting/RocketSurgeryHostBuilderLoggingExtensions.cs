using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
public static class RocketSurgeryHostBuilderLoggingExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <returns></returns>
    public static IHostBuilder ApplyConventions(this IHostBuilder hostBuilder, IConventionContext conventionContext)
    {
        foreach (var item in conventionContext.Conventions.Get<IHostingConvention, HostingConvention>())
        {
            if (item is IHostingConvention convention)
            {
                convention.Register(conventionContext, hostBuilder);
            }
            else if (item is HostingConvention @delegate)
            {
                @delegate(conventionContext, hostBuilder);
            }
        }

        return hostBuilder;
    }

    #if NET8_0_OR_GREATER
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder ApplyConventions(this IHostApplicationBuilder hostBuilder, IConventionContext conventionContext)
    {
        foreach (var item in conventionContext.Conventions.Get<IHostApplicationConvention, HostApplicationConvention>())
        {
            if (item is IHostApplicationConvention convention)
            {
                convention.Register(conventionContext, hostBuilder);
            }
            else if (item is HostApplicationConvention @delegate)
            {
                @delegate(conventionContext, hostBuilder);
            }
        }

        return hostBuilder;
    }
    #endif
}
