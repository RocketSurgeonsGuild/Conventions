using Serilog;

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
        return builder;
    }
}
