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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<WebApplicationBuilder> ApplyConventionsAsync(this WebApplicationBuilder webApplicationBuilder, IConventionContext conventionContext, CancellationToken cancellationToken = default)
    {
        foreach (var item in conventionContext.Conventions.Get<IHostingConvention, HostingConvention, IHostingAsyncConvention, HostingAsyncConvention>())
        {
            switch (item)
            {
                case IHostingConvention convention:
                    convention.Register(conventionContext, webApplicationBuilder.Host);
                    break;
                case HostingConvention @delegate:
                    @delegate(conventionContext, webApplicationBuilder.Host);
                    break;
                case IHostingAsyncConvention convention:
                    await convention.Register(conventionContext, webApplicationBuilder.Host, cancellationToken).ConfigureAwait(false);
                    break;
                case HostingAsyncConvention @delegate:
                    await @delegate(conventionContext, webApplicationBuilder.Host, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return webApplicationBuilder;
    }
}
