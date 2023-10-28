using Sample.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Sample;

public class DefaultCommand(IService service) : Command<AppSettings>
{
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override int Execute(CommandContext context, AppSettings settings)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    {
        AnsiConsole.WriteLine(service.GetString());
        return 1;
    }
}
