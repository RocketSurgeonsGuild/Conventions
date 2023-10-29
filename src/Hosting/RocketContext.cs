using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;

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

        // This code is duplicated per host (web host, generic host, and wasm host)
        configurationBuilder.InsertConfigurationSourceAfter(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(x.Path, $"appsettings.{context.HostingEnvironment.EnvironmentName}.json", StringComparison.OrdinalIgnoreCase)
                       ),
            new IConfigurationSource[]
            {
                new JsonConfigurationSource
                {
                    FileProvider = configurationBuilder.GetFileProvider(),
                    Path = "appsettings.local.json",
                    Optional = true,
                    ReloadOnChange = true
                }
            }
        );

        configurationBuilder.ReplaceConfigurationSourceAt(
            sources => sources.OfType<FileConfigurationSource>().FirstOrDefault(
                x => string.Equals(x.Path, "appsettings.json", StringComparison.OrdinalIgnoreCase)
            ),
            Context.GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new())
                   .SelectMany(z => z.Invoke(configurationBuilder))
                   .Select(z => z.Factory(null))
        );

        if (!string.IsNullOrEmpty(context.HostingEnvironment.EnvironmentName))
        {
            configurationBuilder.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(
                               x => string.Equals(x.Path, $"appsettings.{context.HostingEnvironment.EnvironmentName}.json", StringComparison.OrdinalIgnoreCase)
                           ),
                Context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                       .SelectMany(z => z.Invoke(configurationBuilder, context.HostingEnvironment.EnvironmentName))
                       .Select(z => z.Factory(null))
            );
        }

        configurationBuilder.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(x => string.Equals(x.Path, "appsettings.local.json", StringComparison.OrdinalIgnoreCase)),
            Context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                   .SelectMany(z => z.Invoke(configurationBuilder, "local"))
                   .Select(z => z.Factory(null))
        );

        // Insert after all the normal configuration but before the environment specific configuration

        IConfigurationSource? source = null;
        foreach (var item in configurationBuilder.Sources.Reverse())
        {
            if (item is CommandLineConfigurationSource ||
                ( item is EnvironmentVariablesConfigurationSource env && ( string.IsNullOrWhiteSpace(env.Prefix) ||
                                                                           string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) ) ||
                ( item is FileConfigurationSource a && string.Equals(a.Path, "secrets.json", StringComparison.OrdinalIgnoreCase) ))
            {
                continue;
            }

            source = item;
            break;
        }

        var index = source == null
            ? configurationBuilder.Sources.Count - 1
            : configurationBuilder.Sources.IndexOf(source);

        var cb = new ConfigurationBuilder().ApplyConventions(Context, context.Configuration);
        if (cb.Sources is { Count: > 0 })
        {
            configurationBuilder.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true
                }
            );
        }
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
