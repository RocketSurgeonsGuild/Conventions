using Microsoft.Extensions.DependencyInjection;

var builder = Host
   .CreateApplicationBuilder(args);
builder.Services.Configure<ConsoleLifetimeOptions>(z => z.SuppressStatusMessages = true);
var host = await builder
   .LaunchWith(
        RocketBooster.For(Imports.Instance),
        z => z.ConfigureCommandLine(configurator => configurator.AddCommand<Dump>("dump"))
    );
await host.RunAsync();