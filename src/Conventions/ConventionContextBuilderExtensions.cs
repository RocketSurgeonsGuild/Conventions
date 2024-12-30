using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Convention Context build extensions.
/// </summary>
[PublicAPI]
public static class ConventionContextBuilderExtensions
{
    /// <summary>
    ///     Allows creation of a service provider from the convention context.  This will apply configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceProvider> CreateServiceProvider(
        this IConventionContext context,
        IConfiguration? configuration = null,
        CancellationToken cancellationToken = default
    )
    {
        var cb = new ConfigurationBuilder();
        configuration ??= context.Get<IConfiguration>();
        if (configuration is { })
            cb.AddConfiguration(configuration);
        configuration = ( await cb.ApplyConventionsAsync(context, configuration, cancellationToken).ConfigureAwait(false) ).Build();
        context.Set(configuration);
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        await services.ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);
        await new LoggingBuilder(services).ApplyConventionsAsync(context, cancellationToken).ConfigureAwait(false);

        if (context.Get<ServiceProviderFactoryAdapter>() is not { } factory)
            return services.BuildServiceProvider(context.GetOrAdd(() => new ServiceProviderOptions()));

        var adapter = await factory(context, services, cancellationToken).ConfigureAwait(false);
        var builder = adapter.CreateBuilder(services);
        return adapter.CreateServiceProvider(builder);
    }
}
