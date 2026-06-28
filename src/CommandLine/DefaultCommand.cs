using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

internal class DefaultCommand : Command<AppSettings>
{
    protected override int Execute(CommandContext context, AppSettings settings, CancellationToken token) => CommandLineConstants.WaitCode;
}
