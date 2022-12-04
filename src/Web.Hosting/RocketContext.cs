using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;

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
        _context.Properties.AddIfMissing<IConfiguration>(_webApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing(_webApplicationBuilder.Environment);
        _context.Properties.AddIfMissing<IHostEnvironment>(_webApplicationBuilder.Environment);

        // This code is duplicated per host (web host, generic host, and wasm host)
        _webApplicationBuilder.Configuration.InsertConfigurationSourceAfter(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(
                           x => string.Equals(
                               x.Path, $"appsettings.{_webApplicationBuilder.Environment.EnvironmentName}.json", StringComparison.OrdinalIgnoreCase
                           )
                       ),
            new IConfigurationSource[]
            {
                new JsonConfigurationSource
                {
                    FileProvider = _webApplicationBuilder.Configuration.GetFileProvider(),
                    Path = "appsettings.local.json",
                    Optional = true,
                    ReloadOnChange = true
                }
            }
        );

        _webApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
            sources => sources.OfType<FileConfigurationSource>().FirstOrDefault(
                x => string.Equals(x.Path, "appsettings.json", StringComparison.OrdinalIgnoreCase)
            ),
            _context.GetOrAdd<List<ConfigurationBuilderApplicationDelegate>>(() => new())
                    .SelectMany(z => z.Invoke(_webApplicationBuilder.Configuration))
                    .Select(z => z.Factory(null))
        );

        if (!string.IsNullOrEmpty(_webApplicationBuilder.Environment.EnvironmentName))
        {
            _webApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
                sources => sources
                          .OfType<FileConfigurationSource>()
                          .FirstOrDefault(
                               x => string.Equals(
                                   x.Path, $"appsettings.{_webApplicationBuilder.Environment.EnvironmentName}.json", StringComparison.OrdinalIgnoreCase
                               )
                           ),
                _context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                        .SelectMany(z => z.Invoke(_webApplicationBuilder.Configuration, _webApplicationBuilder.Environment.EnvironmentName))
                        .Select(z => z.Factory(null))
            );
        }

        _webApplicationBuilder.Configuration.ReplaceConfigurationSourceAt(
            sources => sources
                      .OfType<FileConfigurationSource>()
                      .FirstOrDefault(x => string.Equals(x.Path, "appsettings.local.json", StringComparison.OrdinalIgnoreCase)),
            _context.GetOrAdd<List<ConfigurationBuilderEnvironmentDelegate>>(() => new())
                    .SelectMany(z => z.Invoke(_webApplicationBuilder.Configuration, "local"))
                    .Select(z => z.Factory(null))
        );

        // Insert after all the normal configuration but before the environment specific configuration

        IConfigurationSource? source = null;
        var configurationBuilder = (IConfigurationBuilder)_webApplicationBuilder.Configuration;
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

        var cb = new ConfigurationBuilder().ApplyConventions(_context, _webApplicationBuilder.Configuration);
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
    public void ConfigureServices()
    {
        _context.Properties.AddIfMissing(_webApplicationBuilder.Configuration);

        _webApplicationBuilder.Services.ApplyConventions(_context);
        _webApplicationBuilder.Logging.ApplyConventions(_context);
    }

    public IServiceProviderFactory<object> UseServiceProviderFactory()
    {
        return ConventionServiceProviderFactory.Wrap(_context, false);
    }
}
