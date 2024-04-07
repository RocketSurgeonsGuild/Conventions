using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Class RocketContext.
/// </summary>
internal class RocketContext
{
    private readonly IHostBuilder _hostBuilder;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RocketContext" /> class.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    public RocketContext(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder;
    }

    public IConventionContext Context => (IConventionContext)_hostBuilder.Properties[typeof(IConventionContext)];

    /// <summary>
    ///     Construct and compose hosting conventions
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void ComposeHostingConvention(IConfigurationBuilder configurationBuilder)
    {
        if (!_hostBuilder.Properties.TryGetValue(typeof(ConventionContextBuilder), out var conventionContextBuilderObject))
            throw new KeyNotFoundException($"Could not find {nameof(ConventionContextBuilder)}");

        var conventionContextBuilder = (ConventionContextBuilder)conventionContextBuilderObject;
        var contextObject = ConventionContext.From(conventionContextBuilder);
        _hostBuilder.Properties[typeof(IConventionContext)] = contextObject;
        _hostBuilder.ApplyConventions(Context);
    }

    /// <summary>
    ///     Configures the application configuration.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configurationBuilder">The configuration builder.</param>
    public void ConfigureAppConfiguration(
        HostBuilderContext context,
        IConfigurationBuilder configurationBuilder
    )
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (configurationBuilder == null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }

        Context.Properties.AddIfMissing(context.HostingEnvironment);
        Context.Properties.AddIfMissing(context.HostingEnvironment.GetType().FullName!, context.HostingEnvironment);
        RocketInternalsShared.SharedHostConfiguration(Context, configurationBuilder, context.Configuration, context.HostingEnvironment);
    }

    /// <summary>
    ///     Configures the services.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The services.</param>
    public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        Context.Properties.AddIfMissing(context.Configuration);

        services.ApplyConventions(Context);
        new LoggingBuilder(services).ApplyConventions(Context);
    }

    public IServiceProviderFactory<object> UseServiceProviderFactory(HostBuilderContext context)
    {
        return ConventionServiceProviderFactory.Wrap(Context, false);
    }
}
