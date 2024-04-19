using System.Collections;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     In-memory implementation of <see cref="IConfigurationProvider" />
/// </summary>
internal class AppSettingsConfigurationProvider : ConfigurationProvider, IEnumerable<KeyValuePair<string, string?>>
{
    /// <summary>
    ///     Initialize a new instance from the source.
    /// </summary>
    public AppSettingsConfigurationProvider(
    ) { }

    public void Update(CommandContext commandContext, AppSettings appSettings)
    {
        Load();
        var additionalData = new Dictionary<string, string>
                             {
                                 [nameof(AppSettings.Trace)] = appSettings.Trace.ToString(CultureInfo.InvariantCulture),
                                 [nameof(AppSettings.Verbose)] = appSettings.Verbose.ToString(CultureInfo.InvariantCulture),
                                 [nameof(AppSettings.LogLevel)] = appSettings.LogLevel.HasValue ? appSettings.LogLevel.Value.ToString() : "",
                             }
                            .Select(z => new KeyValuePair<string, string>($"{nameof(AppSettings)}:{z.Key}", z.Value))
                             // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                            .Concat(commandContext.Remaining.Parsed.Select(z => new KeyValuePair<string, string>(z.Key, z.Last()!)))
                            .ToDictionary(z => z.Key, z => z.Value);
        if (appSettings.LogLevel.HasValue)
        {
            additionalData.Add("Logging:LogLevel:Default", appSettings.LogLevel.Value.ToString());
            additionalData.Add("Logging:Debug:LogLevel:Default", appSettings.LogLevel.Value.ToString());
            additionalData.Add("Logging:Console:LogLevel:Default", appSettings.LogLevel.Value.ToString());

            var serilogStringValue = appSettings.LogLevel.Value switch
                                     {
                                         LogLevel.Trace    => "Verbose",
                                         LogLevel.Critical => "Fatal",
                                         LogLevel.None     => null,
                                         _                 => appSettings.LogLevel.Value.ToString(),
                                     };
            if (serilogStringValue is { }) additionalData.Add("Serilog:MinimumLevel:Default", serilogStringValue);
        }

        foreach (var item in additionalData)
        {
            Data[item.Key] = item.Value;
        }

        OnReload();
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}