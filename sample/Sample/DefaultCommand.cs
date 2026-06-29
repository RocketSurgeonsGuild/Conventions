using Sample.Core;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Sample;

public class DefaultCommand(IService service) : Command<AppSettings>
{
    protected override int Execute(CommandContext context, AppSettings settings, CancellationToken token)
    {
        AnsiConsole.WriteLine(service.GetString());
        return 1;
    }
}
