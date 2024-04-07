using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Logging;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1848
namespace Microsoft.Extensions.Logging;

/// <summary>
///     Extension method to apply logging conventions
/// </summary>
public static class RocketSurgeryLoggingExtensions
{
    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="loggingBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <returns></returns>
    [Obsolete("Use ApplyConventionsAsync instead, this method does not support async conventions")]
    public static ILoggingBuilder ApplyConventions(this ILoggingBuilder loggingBuilder, IConventionContext conventionContext)
    {
        var configuration = conventionContext.Get<IConfiguration>();
        if (configuration is null)
        {
            configuration = new ConfigurationBuilder().Build();
            conventionContext.Logger.LogWarning("Configuration was not found in context");
        }

        loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
        var logLevel = conventionContext.GetOrAdd(() => new RocketLoggingOptions()).GetLogLevel(configuration);
        if (logLevel.HasValue)
        {
            loggingBuilder.SetMinimumLevel(logLevel.Value);
        }

        foreach (var item in conventionContext.Conventions.Get<ILoggingConvention, LoggingConvention>())
        {
            switch (item)
            {
                case ILoggingConvention convention:
                    convention.Register(conventionContext, configuration, loggingBuilder);
                    break;
                case LoggingConvention @delegate:
                    @delegate(conventionContext, configuration, loggingBuilder);
                    break;
            }
        }

        return loggingBuilder;
    }

    /// <summary>
    ///     Apply logging conventions
    /// </summary>
    /// <param name="loggingBuilder"></param>
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<ILoggingBuilder> ApplyConventionsAsync(this ILoggingBuilder loggingBuilder, IConventionContext conventionContext, CancellationToken cancellationToken = default)
    {
        var configuration = conventionContext.Get<IConfiguration>();
        if (configuration is null)
        {
            configuration = new ConfigurationBuilder().Build();
            conventionContext.Logger.LogWarning("Configuration was not found in context");
        }

        loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
        var logLevel = conventionContext.GetOrAdd(() => new RocketLoggingOptions()).GetLogLevel(configuration);
        if (logLevel.HasValue)
        {
            loggingBuilder.SetMinimumLevel(logLevel.Value);
        }

        foreach (var item in conventionContext.Conventions.Get<
                     ILoggingConvention,
                     LoggingConvention,
                     ILoggingAsyncConvention,
                     LoggingAsyncConvention
                 >())
        {
            switch (item)
            {
                case ILoggingConvention convention:
                    convention.Register(conventionContext, configuration, loggingBuilder);
                    break;
                case LoggingConvention @delegate:
                    @delegate(conventionContext, configuration, loggingBuilder);
                    break;
                case ILoggingAsyncConvention convention:
                    await convention.Register(conventionContext, configuration, loggingBuilder, cancellationToken).ConfigureAwait(false);
                    break;
                case LoggingAsyncConvention @delegate:
                    await @delegate(conventionContext, configuration, loggingBuilder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return loggingBuilder;
    }
}
