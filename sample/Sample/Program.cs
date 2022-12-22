await Host.CreateDefaultBuilder(args)
          .LaunchWith(
               RocketBooster.For(Imports.GetConventions),
               z => z
//                   .SetDefaultCommand<DefaultCommand>()
                  .ConfigureCommandLine(configurator => configurator.AddCommand<Dump>("dump"))
           )
          .RunAsync();
