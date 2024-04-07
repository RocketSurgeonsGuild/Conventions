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
    public static async ValueTask<IServiceProvider> CreateServiceProvider(this IConventionContext context, IConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        var cb = new ConfigurationBuilder();
        configuration ??= context.Get<IConfiguration>();
        if (configuration is { })
            cb.AddConfiguration(configuration);
        configuration = (await cb.ApplyConventionsAsync(context, configuration, cancellationToken)).Build();
        context.Set(configuration);
        var services = new ServiceCollection();
        services.AddSingleton(configuration);

        var factory = await ConventionServiceProviderFactory.FromAsync(context, cancellationToken);
        return await factory.CreateServiceProvider(factory.CreateBuilder(services));
    }

    /// <summary>
    ///     Allows creation of a service provider from the convention builder.  This will apply configuration
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceProvider> CreateServiceProvider(this ConventionContextBuilder builder, IConfiguration? configuration = null)
    {
        return await CreateServiceProvider(await ConventionContext.FromAsync(builder), configuration);
    }

    /// <summary>
    ///     Allows creation of a service provider from the convention context.  This will apply configuration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceProvider> CreateServiceProviderAsync(
        this IConventionContext context,
        IConfiguration? configuration = null,
        CancellationToken cancellationToken = default
    )
    {
        var cb = new ConfigurationBuilder();
        configuration ??= context.Get<IConfiguration>();
        if (configuration is { })
            cb.AddConfiguration(configuration);
        configuration = ( await cb.ApplyConventionsAsync(context, configuration, cancellationToken) ).Build();
        context.Set(configuration);
        var services = new ServiceCollection();
        services.AddSingleton(configuration);

        var factory = await ConventionServiceProviderFactory.FromAsync(context, cancellationToken);
        return await factory.CreateServiceProvider(factory.CreateBuilder(services));
    }

    /// <summary>
    ///     Allows creation of a service provider from the convention builder.  This will apply configuration
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceProvider> CreateServiceProviderAsync(
        this ConventionContextBuilder builder,
        IConfiguration? configuration = null,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateServiceProviderAsync(await ConventionContext.FromAsync(builder, cancellationToken), configuration, cancellationToken);
    }
}
