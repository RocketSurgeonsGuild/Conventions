using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
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
        return await ( await CreateHostBuilder(args) ).RunAsync();
    }

    public static async Task<HostApplicationBuilder> CreateHostBuilder(string[] args)
    {
        return await ( await Host
                            .CreateApplicationBuilder(args)
                            .LaunchWith(RocketBooster.For(Imports.GetConventions)) )
           .ConfigureRocketSurgery(
                builder => builder
                   .ConfigureServices(_ => { })
            );
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
    private readonly IHostBuilder _hostBuilder;
    private readonly IAnsiConsole _console;

    public MyCommand(IHostBuilder hostBuilder, IAnsiConsole console)
    {
        _hostBuilder = hostBuilder;
        _console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, AppSettings settings)
    {
        using var host = _hostBuilder.Build();
        await host.StartAsync();
        if (host.Services.GetRequiredService<IConfiguration>() is IConfigurationRoot root)
        {
            _console.WriteLine(root.GetDebugView());
        }

        return 0;
    }
}