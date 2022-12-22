using Sample.Core;
using Spectre.Console.Cli;

namespace Sample;

public class DefaultCommand : Command<AppSettings>
{
    private readonly IService _service;

    public DefaultCommand(IService service)
    {
        _service = service;
    }

    public override int Execute(CommandContext context, AppSettings settings)
    {
        Console.WriteLine(_service.GetString());
        return 1;
    }
}
