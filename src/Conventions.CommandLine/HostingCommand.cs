using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

class HostingCommand : AsyncCommand<AppSettings>
{
    private readonly IHostBuilder _hostBuilder;

    public HostingCommand(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, AppSettings settings)
    {
        var result = new HostingResult();
        _hostBuilder.Properties.Add(typeof(HostingResult), result);
        CommandLineArgumentsExtractorCommand.PopulateResult(result, context, settings);
        await _hostBuilder.Build().RunAsync();
        return 0;
    }
}