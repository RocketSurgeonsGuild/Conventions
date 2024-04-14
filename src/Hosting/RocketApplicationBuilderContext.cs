using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using ServiceFactoryAdapter =
    System.Func<Rocket.Surgery.Conventions.IConventionContext, Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Microsoft.Extensions.DependencyInjection.IServiceProviderFactory<object>>>;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Class RocketContext.
/// </summary>
internal sealed class RocketApplicationBuilderContext
{
    private readonly IHostApplicationBuilder _hostApplicationBuilder;
    private readonly IConventionContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RocketApplicationBuilderContext" /> class.
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
        _context.Properties.AddIfMissing(_hostApplicationBuilder.Environment.GetType(), _hostApplicationBuilder.Environment);
        await RocketInternalsShared.SharedHostConfigurationAsync(_context, _hostApplicationBuilder, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Configures the services.
    /// </summary>
    public async ValueTask ConfigureServices(CancellationToken cancellationToken)
    {
        await _hostApplicationBuilder.Services.ApplyConventionsAsync(_context, cancellationToken).ConfigureAwait(false);
        await _hostApplicationBuilder.Logging.ApplyConventionsAsync(_context, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<IServiceProviderFactory<object>?> UseServiceProviderFactory(IServiceCollection services, CancellationToken cancellationToken)
    {
        if (_context.Get<ServiceFactoryAdapter>() is not { } factory)
        {
            return null;
        }

        return await factory(_context, services, cancellationToken);
    }
}