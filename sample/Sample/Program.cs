await Host.CreateDefaultBuilder(args)
          .UseConsoleLifetime(z => z.SuppressStatusMessages = true)
          .LaunchWith(
               RocketBooster.For(Imports.GetConventions),
               z => z
//                   .SetDefaultCommand<DefaultCommand>()
                  .ConfigureCommandLine(configurator => configurator.AddCommand<Dump>("dump"))
           )
          .RunAsync();
