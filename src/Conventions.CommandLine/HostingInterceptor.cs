using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

class HostingInterceptor : ICommandInterceptor
{
    private readonly IHostBuilder _hostBuilder;
    private readonly ICommandInterceptor? _commandInterceptor;

    public HostingInterceptor(IHostBuilder hostBuilder, ICommandInterceptor? commandInterceptor)
    {
        _hostBuilder = hostBuilder;
        _commandInterceptor = commandInterceptor;
    }

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        _commandInterceptor?.Intercept(context, settings);
        if (settings is not AppSettings appSettings) appSettings = new AppSettings();
        var result = new HostingResult();
        CommandLineArgumentsExtractorCommand.PopulateResult(result, context, appSettings);
        _hostBuilder.Properties.Add(typeof(HostingResult), result);
    }
}