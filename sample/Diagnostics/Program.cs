using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.CommandLine;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

// [assembly: Convention(typeof(Program))]

namespace Diagnostics;

public static partial class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await ( await CreateHostBuilder(args) ).RunConsoleAppAsync();
    }

    public static async Task<IHost> CreateHostBuilder(string[] args)
    {
        return await Host.CreateApplicationBuilder(args).LaunchWith(RocketBooster.For(Imports.Instance));
    }
}

[ExportConvention]
internal class Convention : ICommandLineConvention, IServiceConvention
{
    public void Register(IConventionContext context, IConfigurator app)
    {
        app.AddDelegate(
            "test",
            _ => 1
        );
        app.AddCommand<MyCommand>("dump");
    }

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services) { }
}

internal class MyCommand : AsyncCommand<AppSettings>
{
    private readonly IConfiguration _configuration;
    private readonly IAnsiConsole _console;

    public MyCommand(IConfiguration configuration, IAnsiConsole console)
    {
        _configuration = configuration;
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, AppSettings settings)
    {
        if (_configuration is IConfigurationRoot root) _console.WriteLine(root.GetDebugView());

        return 0;
    }
}