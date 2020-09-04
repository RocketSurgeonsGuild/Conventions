using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

// [assembly: Convention(typeof(Program))]

namespace Diagnostics
{
    [PublicAPI]
    public static class Program
    {
        public static Task<int> Main(string[] args) => CreateHostBuilder(args).RunCli();

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
           .LaunchWith(RocketBooster.For(DependencyContext.Default))
           .ConfigureRocketSurgery(
                builder => builder.ConfigureServices(x => { }).ConfigureCommandLine((a, cl) => cl.OnRun(_ => 1))
            );
    }
}