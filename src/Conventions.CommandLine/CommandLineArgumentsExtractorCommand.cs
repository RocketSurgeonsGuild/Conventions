using System.Globalization;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

class CommandLineArgumentsExtractorCommand : Command<AppSettings>
{
    private readonly HostingResult _result;

    public CommandLineArgumentsExtractorCommand(HostingResult result) => _result = result;

    public override int Execute(CommandContext context, AppSettings settings)
    {
        PopulateResult(_result, context, settings);
        return 0;
    }

    public static void PopulateResult(HostingResult result, CommandContext context, AppSettings settings)
    {
        result.Configuration = new Dictionary<string, string>
        {
            [nameof(AppSettings.Trace)] = settings.Trace.ToString(CultureInfo.InvariantCulture),
            [nameof(AppSettings.Verbose)] = settings.Verbose.ToString(CultureInfo.InvariantCulture),
            [nameof(AppSettings.LogLevel)] = settings.LogLevel.HasValue ? settings.LogLevel.Value.ToString() : ""
        }.ToDictionary(z => $"{nameof(AppSettings)}:{z.Key}", z => z.Value);
        result.Arguments = context.Remaining;
    }
}