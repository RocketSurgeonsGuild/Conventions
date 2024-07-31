using Microsoft.Extensions.DependencyInjection;

var builder = Host
   .CreateApplicationBuilder(args);
builder.Services.Configure<ConsoleLifetimeOptions>(z => z.SuppressStatusMessages = true);
await builder
     .LaunchWith(
          RocketBooster.For(Imports.Instance),
          z => z.ConfigureCommandLine(configurator => configurator.AddCommand<Dump>("dump"))
      )
     .RunConsoleAppAsync();

public partial class Program;