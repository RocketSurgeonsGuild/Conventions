using Microsoft.Extensions.DependencyInjection;

var builder = await Host
                   .CreateApplicationBuilder(args)
                   .LaunchWith(
                        RocketBooster.For(Imports.Instance),
                        z => z
                            //                   .SetDefaultCommand<DefaultCommand>()
                           .ConfigureCommandLine(configurator => configurator.AddCommand<Dump>("dump"))
                    );
builder.Services.Configure<ConsoleLifetimeOptions>(z => z.SuppressStatusMessages = true);
await builder.RunAsync();
