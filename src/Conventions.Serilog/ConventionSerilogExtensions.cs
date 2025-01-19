using Microsoft.Extensions.Logging;
using Serilog.Events;
using ILogger = Serilog.ILogger;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
/// serilog
/// </summary>
public static class ConventionSerilogExtensions
{
    /// <summary>
    ///     Provide a serilog logger
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ConventionContextBuilder UseLogger(this ConventionContextBuilder builder, ILogger logger)
    {
        builder.Set(logger);
        #pragma warning disable CA2000
        ILoggerFactory factory = new global::Serilog.Extensions.Logging.SerilogLoggerFactory(logger);
        #pragma warning restore CA2000
        builder.Set(factory);
        builder.Set<Microsoft.Extensions.Logging.ILogger>(factory.CreateLogger<IConventionContext>());
        return builder;
    }
}
