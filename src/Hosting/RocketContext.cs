using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;
using LoggingBuilder = Microsoft.Extensions.DependencyInjection.LoggingBuilder;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Class RocketContext.
/// </summary>
internal class RocketContext
{
    private readonly IHostBuilder _hostBuilder;
    private readonly Func<IConventionContext> getContext;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RocketContext" /> class.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="conventionContextBuilder"></param>
    public RocketContext(IHostBuilder hostBuilder, ConventionContextBuilder conventionContextBuilder)
    {
        _hostBuilder = hostBuilder;
        IConventionContext? context = null;
        getContext = () =>
        {
            context ??= ConventionContext.From(conventionContextBuilder);
            return context;
        };
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RocketContext" /> class.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="context"></param>
    public RocketContext(IHostBuilder hostBuilder, IConventionContext context)
    {
        _hostBuilder = hostBuilder;
        getContext = () => context;
    }

    /// <summary>
    ///     Construct and compose hosting conventions
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void ComposeHostingConvention(IConfigurationBuilder configurationBuilder)
    {
        _hostBuilder.ApplyConventions(getContext());
    }

    /// <summary>
    ///     Captures the arguments.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder.</param>
    public void CaptureArguments(IConfigurationBuilder configurationBuilder)
    {
        if (configurationBuilder == null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }

        var commandLineSource = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>()
                                                    .FirstOrDefault();
        if (commandLineSource != null)
        {
            var args = commandLineSource.Args;
            var app = new CommandApp<CommandLineArgumentsExtractorCommand>();
            if (!_hostBuilder.Properties.TryGetValue(typeof(HostingResult), out var r))
            {
                var hr = new HostingResult();
                r = hr;
                app.Configure(
                    z => { z.Settings.Registrar.RegisterInstance(hr); }
                );
                app.Run(args);
            }

            if (r is HostingResult result)
            {
                commandLineSource.Args = result.Arguments?.Raw ?? args;

                if (result.Configuration is not null)
                {
                    configurationBuilder.AddInMemoryCollection(result.Configuration);
                }
            }
        }
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

        getContext().Properties.AddIfMissing(context.HostingEnvironment);
        configurationBuilder.UseLocalConfiguration(
            getContext().GetOrAdd(() => new ConfigOptions()).UseEnvironment(context.HostingEnvironment.EnvironmentName)
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

        var cb = new ConfigurationBuilder().ApplyConventions(getContext(), configurationBuilder.Build());

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

        getContext().Properties.AddIfMissing(context.Configuration);

        services.ApplyConventions(getContext());
        new LoggingBuilder(services).ApplyConventions(getContext());
    }
}
