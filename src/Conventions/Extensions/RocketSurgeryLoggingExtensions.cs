using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
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
        var configuration = context.Get<IConfiguration>();
        if (configuration is null)
        {
            configuration = new ConfigurationBuilder().Build();
            context.Logger.LogWarning("Configuration was not found in context");
        }

        loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
        var logLevel = context.GetOrAdd(() => new RocketLoggingOptions()).GetLogLevel(configuration);
        if (logLevel.HasValue) loggingBuilder.SetMinimumLevel(logLevel.Value);

        foreach (var item in context.Conventions.Get<
                     ILoggingConvention,
                     LoggingConvention,
                     ILoggingAsyncConvention,
                     LoggingAsyncConvention
                 >())
        {
            switch (item)
            {
                case ILoggingConvention convention:
                    convention.Register(context, configuration, loggingBuilder);
                    break;
                case LoggingConvention @delegate:
                    @delegate(context, configuration, loggingBuilder);
                    break;
                case ILoggingAsyncConvention convention:
                    await convention.Register(context, configuration, loggingBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case LoggingAsyncConvention @delegate:
                    await @delegate(context, configuration, loggingBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return loggingBuilder;
    }
}
