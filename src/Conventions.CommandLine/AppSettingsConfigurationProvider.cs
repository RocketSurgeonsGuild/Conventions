using System.Collections;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     In-memory implementation of <see cref="IConfigurationProvider" />
/// </summary>
internal class AppSettingsConfigurationProvider : CommandLineConfigurationProvider, IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    ///     Initialize a new instance from the source.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="switchMappings"></param>
    public AppSettingsConfigurationProvider(
        IEnumerable<string> args,
        IDictionary<string, string>? switchMappings = null
    ) : base(args, switchMappings)
    {
    }

    public void Update(CommandContext commandContext, AppSettings appSettings)
    {
        Data = new Dictionary<string, string>
               {
                   [nameof(AppSettings.Trace)] = appSettings.Trace.ToString(CultureInfo.InvariantCulture),
                   [nameof(AppSettings.Verbose)] = appSettings.Verbose.ToString(CultureInfo.InvariantCulture),
                   [nameof(AppSettings.LogLevel)] = appSettings.LogLevel.HasValue ? appSettings.LogLevel.Value.ToString() : ""
               }
              .Select(z => new KeyValuePair<string, string>($"{nameof(AppSettings)}:{z.Key}", z.Value))
              .Concat(commandContext.Remaining.Parsed.Select(z => new KeyValuePair<string, string>(z.Key, z.Last()!)))
              .ToDictionary(z => z.Key, z => z.Value);
        if (appSettings.LogLevel.HasValue)
        {
            Data.Add("Logging:LogLevel:Default", appSettings.LogLevel.Value.ToString());
            Data.Add("Logging:Debug:LogLevel:Default", appSettings.LogLevel.Value.ToString());
            Data.Add("Logging:Console:LogLevel:Default", appSettings.LogLevel.Value.ToString());

            var serilogStringValue = appSettings.LogLevel.Value switch
            {
                LogLevel.Trace    => "Verbose",
                LogLevel.Critical => "Fatal",
                LogLevel.None     => null,
                _                 => appSettings.LogLevel.Value.ToString()
            };
            if (serilogStringValue is not null)
            {
                Data.Add("Serilog:MinimumLevel:Default", serilogStringValue);
            }
        }

        OnReload();
    }

    public void Default()
    {
        base.Load();
        OnReload();
    }

    public override void Load()
    {
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
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
