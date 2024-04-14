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
    /// <returns></returns>
    public static IServiceProvider CreateServiceProvider(this IConventionContext context, IConfiguration? configuration = null)
    {
        var cb = new ConfigurationBuilder();
        configuration ??= context.Get<IConfiguration>();
        if (configuration is { })
            cb.AddConfiguration(configuration);
        configuration = cb.ApplyConventions(context).Build();
        context.Set(configuration);
        var services = new ServiceCollection();
        services.AddSingleton(configuration);

        var factory = ConventionServiceProviderFactory.From(context);
        return factory.CreateServiceProvider(factory.CreateBuilder(services));
    }

    /// <summary>
    ///     Allows creation of a service provider from the convention builder.  This will apply configuration
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceProvider CreateServiceProvider(this ConventionContextBuilder builder, IConfiguration? configuration = null)
    {
        return CreateServiceProvider(ConventionContext.From(builder), configuration);
    }
}
