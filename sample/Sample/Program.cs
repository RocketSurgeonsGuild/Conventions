using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using System.Threading.Tasks;

namespace Sample
{
    [ImportConventions]
    public static partial class Program
    {
        public static Task<int> Main(string[] args) => CreateHostBuilder(args).RunCli();

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
           .LaunchWith(RocketBooster.For(GetConventions))
           .ConfigureRocketSurgery(builder => builder.ConfigureCommandLine(context => context.OnRun<DefaultCommand>().AddCommand<Dump>()));
    }
}