using Rocket.Surgery.Conventions.CommandLine;
using Sample.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Sample;

public class DefaultCommand : Command
{
    private readonly IService _service;

    public DefaultCommand(IService service)
    {
        _service = service;
    }

    public override int Execute(CommandContext context)
    {
        Console.WriteLine(_service.GetString());
        return 1;
    }
}
