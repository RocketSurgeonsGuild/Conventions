using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryHostApplicationExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask ApplyConventionsAsync<TBuilder>(
        this TBuilder hostBuilder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    ) where TBuilder : IHostApplicationBuilder
    {
        foreach (var item in context.Conventions
                                    .Get<
                                         IHostApplicationConvention<TBuilder>,
                                         HostApplicationConvention<TBuilder>,
                                         IHostApplicationAsyncConvention<TBuilder>,
                                         HostApplicationAsyncConvention<TBuilder>
                                     >())
        {
            switch (item)
            {
                case IHostApplicationConvention<TBuilder> convention:
                    convention.Register(context, hostBuilder);
                    break;
                case HostApplicationConvention<TBuilder> @delegate:
                    @delegate(context, hostBuilder);
                    break;
                case IHostApplicationAsyncConvention<TBuilder> convention:
                    await convention.Register(context, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case HostApplicationAsyncConvention<TBuilder> @delegate:
                    await @delegate(context, hostBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
    }
}
