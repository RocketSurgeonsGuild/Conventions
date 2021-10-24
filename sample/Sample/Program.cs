using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Sample;

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
                   .LaunchWith(RocketBooster.For(GetConventions))
                   .ConfigureRocketSurgery(builder => builder.ConfigureCommandLine(context => context.OnRun<DefaultCommand>().AddCommand<Dump>()));
    }
}
