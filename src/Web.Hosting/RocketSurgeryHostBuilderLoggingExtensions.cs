#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
public static class RocketSurgeryWebApplicationBuilderLoggingExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <returns></returns>
    public static WebApplicationBuilder ApplyConventions(this WebApplicationBuilder webApplicationBuilder, IConventionContext conventionContext)
    {
        foreach (var item in conventionContext.Conventions.Get<IHostingConvention, HostingConvention>())
        {
            if (item is IHostingConvention convention)
            {
                convention.Register(conventionContext, webApplicationBuilder.Host);
            }
            else if (item is HostingConvention @delegate)
            {
                @delegate(conventionContext, webApplicationBuilder.Host);
            }
        }

        return webApplicationBuilder;
    }
}
#endif
