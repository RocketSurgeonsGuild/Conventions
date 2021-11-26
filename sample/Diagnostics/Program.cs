using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Hosting;
using Spectre.Console.Cli;

// [assembly: Convention(typeof(Program))]

namespace Diagnostics
{
    [ImportConventions]
    public static partial class Program
    {
        public static Task Main(string[] args)
        {
            return CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .LaunchWith(RocketBooster.ForDependencyContext(DependencyContext.Default))
                       .ConfigureRocketSurgery(
                            builder => builder
                                      .ConfigureServices(_ => { })
                                      .ConfigureCommandLine((_, cl) => cl.OnRun(_ => 1))
                        );
        }
    }
}
