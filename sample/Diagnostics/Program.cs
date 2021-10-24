using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

// [assembly: Convention(typeof(Program))]

namespace Diagnostics
{
    [ImportConventions]
    public static class Program
    {
        public static Task<int> Main(string[] args)
        {
            return CreateHostBuilder(args).RunCli();
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
