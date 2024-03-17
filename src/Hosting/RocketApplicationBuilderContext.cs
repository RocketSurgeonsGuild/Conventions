#if NET8_0_OR_GREATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Class RocketContext.
/// </summary>
internal sealed class RocketApplicationBuilderContext
{
    private readonly IHostApplicationBuilder _hostApplicationBuilder;
    private readonly IConventionContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RocketContext" /> class.
    /// </summary>
    /// <param name="hostApplicationBuilder">The host builder.</param>
    /// <param name="context"></param>
    public RocketApplicationBuilderContext(IHostApplicationBuilder hostApplicationBuilder, IConventionContext context)
    {
        _hostApplicationBuilder = hostApplicationBuilder;
        _context = context;
    }

    /// <summary>
    ///     Construct and compose hosting conventions
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void ComposeHostingConvention()
    {
        _hostApplicationBuilder.ApplyConventions(_context);
    }

    /// <summary>
    ///     Configures the application configuration.
    /// </summary>
    public void ConfigureAppConfiguration()
    {
        _context.Properties.AddIfMissing(_hostApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing<IConfiguration>(_hostApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing(_hostApplicationBuilder.Environment);
        RocketInternalsShared.SharedHostConfiguration(
            _context,
            _hostApplicationBuilder.Configuration,
            _hostApplicationBuilder.Configuration,
            _hostApplicationBuilder.Environment
        );
    }

    /// <summary>
    ///     Configures the services.
    /// </summary>
    public void ConfigureServices()
    {
        _hostApplicationBuilder.Services.ApplyConventions(_context);
        _hostApplicationBuilder.Logging.ApplyConventions(_context);
    }

    public IServiceProviderFactory<object> UseServiceProviderFactory()
    {
        return ConventionServiceProviderFactory.Wrap(_context, false);
    }
}
#endif