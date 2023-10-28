using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

internal class DefaultCommand : Command<AppSettings>
{
    private readonly IAnsiConsole _console;

    public DefaultCommand(IAnsiConsole console)
    {
        _console = console;
    }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override int Execute(CommandContext context, AppSettings settings)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    {
        if (settings.Version)
        {
            _console.WriteLine(
                Assembly.GetEntryAssembly()?
                   .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                   .InformationalVersion ?? "?"
            );
            return 0;
        }

        return CommandLineConstants.WaitCode;
    }
}
