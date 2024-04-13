using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Convention Context build extensions.
/// </summary>
[PublicAPI]
public static class ConventionContextBuilderExtensions
{

    /// <summary>
    ///     Use the given app domain for resolving assemblies
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="appDomain"></param>
    /// <returns></returns>
    public static ConventionContextBuilder UseAppDomain(this ConventionContextBuilder builder, AppDomain appDomain) => builder.WithConventionsFrom(new AppDomainConventionFactory(appDomain));

    /// <summary>
    ///     Use the given set of assemblies
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static ConventionContextBuilder UseAssemblies(this ConventionContextBuilder builder, IEnumerable<Assembly> assemblies) => builder.WithConventionsFrom(new AssemblyConventionFactory(assemblies));

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
        configuration = ( await cb.ApplyConventionsAsync(context, configuration, cancellationToken) ).Build();
        context.Set(configuration);
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        await services.ApplyConventionsAsync(context, cancellationToken);
        await new LoggingBuilder(services).ApplyConventionsAsync(context, cancellationToken);

        return services.BuildServiceProvider(context.GetOrAdd(() => new ServiceProviderOptions()));
    }

    /// <summary>
    ///     Allows creation of a service provider from the convention builder.  This will apply configuration
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IServiceProvider> CreateServiceProvider(
        this ConventionContextBuilder builder,
        IConfiguration? configuration = null,
        CancellationToken cancellationToken = default
    )
    {
        return await CreateServiceProvider(await ConventionContext.FromAsync(builder, cancellationToken), configuration, cancellationToken);
    }
}
