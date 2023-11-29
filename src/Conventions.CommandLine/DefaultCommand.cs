using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class DefaultCommand : Command<AppSettings>
{
    public override int Execute(CommandContext context, AppSettings settings)
    {
        return CommandLineConstants.WaitCode;
    }
}