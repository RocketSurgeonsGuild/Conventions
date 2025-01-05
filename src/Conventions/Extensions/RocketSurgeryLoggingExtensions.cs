using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Rocket.Surgery.Conventions.Logging;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1848
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
public static class RocketSurgeryLoggingExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="loggingBuilder"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<ILoggingBuilder> ApplyConventionsAsync(
        this ILoggingBuilder loggingBuilder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        loggingBuilder.AddConfiguration(context.Configuration.GetSection(nameof(Logging)));
        var logLevel = context.GetOrAdd(() => new RocketLoggingOptions()).GetLogLevel(context.Configuration);
        if (logLevel.HasValue) loggingBuilder.SetMinimumLevel(logLevel.Value);

        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<ILoggingConvention>(convention => convention.Register(context, context.Configuration, loggingBuilder))
                      .AddHandler<ILoggingAsyncConvention>(convention => convention.Register(context, context.Configuration, loggingBuilder, cancellationToken))
                      .AddHandler<LoggingConvention>(convention => convention(context, context.Configuration, loggingBuilder))
                      .AddHandler<LoggingAsyncConvention>(convention => convention(context, context.Configuration, loggingBuilder, cancellationToken))
              )
             .ConfigureAwait(false);
        return loggingBuilder;
    }
}
