using Sample.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Sample;

public class DefaultCommand(IService service) : Command<AppSettings>
{
    public override int Execute(CommandContext context, AppSettings settings)
    {
        AnsiConsole.WriteLine(service.GetString());
        return 1;
    }
}