using Microsoft.Extensions.Logging;

using Rocket.Surgery.Clavus.Logging;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1848
namespace Rocket.Surgery.Clavus;

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
        IClavusContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        loggingBuilder.AddConfiguration(context.Configuration.GetSection(nameof(Logging)));
        var logLevel = context.GetOrAdd(() => new ClavusLoggingOptions()).GetLogLevel(context.Configuration);
        if (logLevel.HasValue) loggingBuilder.SetMinimumLevel(logLevel.Value);

        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<ILoggingPart>(convention => convention.Register(context, context.Configuration, loggingBuilder))
                      .AddHandler<ILoggingAsyncPart>(convention => convention.Register(context, context.Configuration, loggingBuilder, cancellationToken))
                      .AddHandler<LoggingPart>(convention => convention(context, context.Configuration, loggingBuilder))
                      .AddHandler<LoggingAsyncPart>(convention => convention(context, context.Configuration, loggingBuilder, cancellationToken))
              )
             .ConfigureAwait(false);
        return loggingBuilder;
    }
}
