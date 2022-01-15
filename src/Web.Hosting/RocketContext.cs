#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Configuration;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Web.Hosting;

/// <summary>
///     Class RocketContext.
/// </summary>
internal class RocketContext
{
    private readonly WebApplicationBuilder _webApplicationBuilder;
    private readonly IConventionContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RocketContext" /> class.
    /// </summary>
    /// <param name="webApplicationBuilder">The host builder.</param>
    /// <param name="context"></param>
    public RocketContext(WebApplicationBuilder webApplicationBuilder, IConventionContext context)
    {
        _webApplicationBuilder = webApplicationBuilder;
        _context = context;
    }

    /// <summary>
    ///     Construct and compose hosting conventions
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void ComposeHostingConvention()
    {
        _webApplicationBuilder.ApplyConventions(_context);
    }

    /// <summary>
    ///     Configures the application configuration.
    /// </summary>
    public void ConfigureAppConfiguration()
    {
        _context.Properties.AddIfMissing(_webApplicationBuilder.Environment);
        _context.Properties.AddIfMissing<IHostEnvironment>(_webApplicationBuilder.Environment);
        _webApplicationBuilder.Configuration.UseLocalConfiguration(
            _context.GetOrAdd(() => new ConfigOptions()).UseEnvironment(_webApplicationBuilder.Environment.EnvironmentName)
        );

        // Insert after all the normal configuration but before the environment specific configuration

        IConfigurationSource? source = null;
        var configurationBuilder = (IConfigurationBuilder)_webApplicationBuilder.Configuration;
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

        var cb = new ConfigurationBuilder().ApplyConventions(_context, _webApplicationBuilder.Configuration);

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
    public void ConfigureServices()
    {
        _context.Properties.AddIfMissing(_webApplicationBuilder.Configuration);

        _webApplicationBuilder.Services.ApplyConventions(_context);
        _webApplicationBuilder.Logging.ApplyConventions(_context);
    }
}
#endif
