using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Web.Hosting;

/// <summary>
///     Class RocketContext.
/// </summary>
internal sealed class RocketContext
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
        _context.Properties.AddIfMissing(_webApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing<IConfiguration>(_webApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing(_webApplicationBuilder.Environment);
        _context.Properties.AddIfMissing<IHostEnvironment>(_webApplicationBuilder.Environment);
        RocketInternalsShared.SharedHostConfiguration(
            _context,
            _webApplicationBuilder.Configuration,
            _webApplicationBuilder.Configuration,
            _webApplicationBuilder.Environment
        );
    }

    /// <summary>
    ///     Configures the services.
    /// </summary>
    public void ConfigureServices()
    {
        _webApplicationBuilder.Services.ApplyConventions(_context);
        _webApplicationBuilder.Logging.ApplyConventions(_context);
    }

    public IServiceProviderFactory<object> UseServiceProviderFactory()
    {
        return ConventionServiceProviderFactory.Wrap(_context, false);
    }
}