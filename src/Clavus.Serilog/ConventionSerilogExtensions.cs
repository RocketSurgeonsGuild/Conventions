using Microsoft.Extensions.Logging;

using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Clavus;

/// <summary>
///     serilog
/// </summary>
public static class ConventionSerilogExtensions
{
    /// <summary>
    ///     Provide a serilog logger
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ClavusContextBuilder UseLogger(this ClavusContextBuilder builder, ILogger logger)
    {
        builder.Set(logger);
#pragma warning disable CA2000
        ILoggerFactory factory = new SerilogLoggerFactory(logger);
#pragma warning restore CA2000
        builder.Set(factory);
        builder.Set<Microsoft.Extensions.Logging.ILogger>(factory.CreateLogger<IClavusContext>());
        return builder;
    }
}
