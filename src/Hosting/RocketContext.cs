using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Configuration;

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

    public IConventionContext ConventionContext => (IConventionContext)_hostBuilder.Properties[typeof(IConventionContext)];

    /// <summary>
    ///     Construct and compose hosting conventions
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void ComposeHostingConvention(IConfigurationBuilder configurationBuilder)
    {
        if (!_hostBuilder.Properties.TryGetValue(typeof(ConventionContextBuilder), out var conventionContextBuilderObject))
            throw new KeyNotFoundException($"Could not find {nameof(ConventionContextBuilder)}");

        var conventionContextBuilder = (ConventionContextBuilder)conventionContextBuilderObject!;
        var contextObject = Conventions.ConventionContext.From(conventionContextBuilder);
        _hostBuilder.Properties[typeof(IConventionContext)] = contextObject;
        _hostBuilder.ApplyConventions(ConventionContext);
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

        ConventionContext.Properties.AddIfMissing(context.HostingEnvironment);
        configurationBuilder.UseLocalConfiguration(
            ConventionContext.GetOrAdd(() => new ConfigOptions()).UseEnvironment(context.HostingEnvironment.EnvironmentName)
        );

        // Insert after all the normal configuration but before the environment specific configuration

        IConfigurationSource? source = null;
        foreach (var item in configurationBuilder.Sources.Reverse())
        {
            if (item is CommandLineConfigurationSource ||
                ( item is EnvironmentVariablesConfigurationSource env && ( string.IsNullOrWhiteSpace(env.Prefix) ||
                                                                           string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) ) ||
                ( item is JsonConfigurationSource a && string.Equals(
                    a.Path,
                    "secrets.json",
                    StringComparison.OrdinalIgnoreCase
                ) ))
            {
                continue;
            }

            source = item;
            break;
        }

        var index = source == null
            ? configurationBuilder.Sources.Count - 1
            : configurationBuilder.Sources.IndexOf(source);

        var cb = new ConfigurationBuilder().ApplyConventions(ConventionContext, configurationBuilder.Build());

        configurationBuilder.Sources.Insert(
            index + 1,
            new ChainedConfigurationSource
            {
                Configuration = cb.Build(),
                ShouldDisposeConfiguration = true
            }
        );
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

        ConventionContext.Properties.AddIfMissing(context.Configuration);

        services.ApplyConventions(ConventionContext);
        new LoggingBuilder(services).ApplyConventions(ConventionContext);
    }

    public IServiceProviderFactory<object> UseServiceProviderFactory(HostBuilderContext context)
    {
        return ConventionServiceProviderFactory.Wrap(ConventionContext, false);
    }
}
