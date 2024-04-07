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
    public async ValueTask ComposeHostingConvention(CancellationToken cancellationToken)
    {
        await _webApplicationBuilder.ApplyConventionsAsync(_context, cancellationToken);
    }

    /// <summary>
    ///     Configures the application configuration.
    /// </summary>
    public ValueTask ConfigureAppConfiguration(CancellationToken cancellationToken)
    {
        _context.Properties.AddIfMissing(_webApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing<IConfiguration>(_webApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing(_webApplicationBuilder.Environment);
        _context.Properties.AddIfMissing<IHostEnvironment>(_webApplicationBuilder.Environment);
        return RocketInternalsShared.SharedHostConfigurationAsync(
            _context,
            _webApplicationBuilder.Configuration,
            _webApplicationBuilder.Configuration,
            _webApplicationBuilder.Environment,
            cancellationToken
        );
    }

    /// <summary>
    ///     Configures the services.
    /// </summary>
    public async Task ConfigureServices(CancellationToken cancellationToken)
    {
        await _webApplicationBuilder.Services.ApplyConventionsAsync(_context, cancellationToken).ConfigureAwait(false);
        await _webApplicationBuilder.Logging.ApplyConventionsAsync(_context, cancellationToken).ConfigureAwait(false);
    }

    public IServiceProviderFactory<object> UseServiceProviderFactory()
    {
        return ConventionServiceProviderFactory.Wrap(_context, false);
    }
}