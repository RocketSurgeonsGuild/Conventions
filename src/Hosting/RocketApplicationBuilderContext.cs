#if NET6_0_OR_GREATER
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
    public ValueTask ComposeHostingConvention(CancellationToken cancellationToken)
    {
        return _hostApplicationBuilder.ApplyConventionsAsync(_context, cancellationToken);
    }

    /// <summary>
    ///     Configures the application configuration.
    /// </summary>
    public async ValueTask ConfigureAppConfiguration(CancellationToken cancellationToken)
    {
        _context.Properties.AddIfMissing(_hostApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing<IConfiguration>(_hostApplicationBuilder.Configuration);
        _context.Properties.AddIfMissing(_hostApplicationBuilder.Environment);
        _context.Properties.AddIfMissing(_hostApplicationBuilder.Environment.GetType().FullName!, _hostApplicationBuilder.Environment);
        await RocketInternalsShared
             .SharedHostConfigurationAsync(
                  _context,
                  _hostApplicationBuilder.Configuration,
                  _hostApplicationBuilder.Configuration,
                  _hostApplicationBuilder.Environment,
                  cancellationToken
              )
             .ConfigureAwait(false);
    }

    /// <summary>
    ///     Configures the services.
    /// </summary>
    public async ValueTask ConfigureServices(CancellationToken cancellationToken)
    {
        await _hostApplicationBuilder.Services.ApplyConventionsAsync(_context, cancellationToken).ConfigureAwait(false);
        await _hostApplicationBuilder.Logging.ApplyConventionsAsync(_context, cancellationToken).ConfigureAwait(false);
    }

    public IServiceProviderFactory<object> UseServiceProviderFactory()
    {
        return ConventionServiceProviderFactory.Wrap(_context, false);
    }
}
#endif