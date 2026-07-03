using Clavus.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA2000

namespace Clavus;

/// <summary>
///     Convention Context build extensions.
/// </summary>
[PublicAPI]
public static class ClavusContextBuilderExtensions
{
    /// <summary>
    ///     Allows creation of a service provider from the convention context.  This will apply configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceProvider> CreateServiceProvider(this IClavusContext context, CancellationToken cancellationToken = default)
    {
        var cb = new ConfigurationManager();
        await cb.ApplyPartsAsync(context, cancellationToken).ConfigureAwait(false);
        context.Set(cb).Set<IConfigurationRoot>(cb).Set<IConfiguration>(cb);
        var services = new ServiceCollection();
        services.AddSingleton<IConfigurationRoot>(cb).AddSingleton(cb).AddSingleton<IConfiguration>(cb);
        await services.ApplyPartsAsync(context, cancellationToken).ConfigureAwait(false);
        await new LoggingBuilder(services).ApplyPartsAsync(context, cancellationToken).ConfigureAwait(false);

        if (context.Get<ServiceProviderFactoryAdapter>() is not { } factory)
            return services.BuildServiceProvider(context.GetOrAdd(() => new ServiceProviderOptions()));

        var adapter = await factory(context, services, cancellationToken).ConfigureAwait(false);
        var builder = adapter.CreateBuilder(services);
        return adapter.CreateServiceProvider(builder);
    }
}
