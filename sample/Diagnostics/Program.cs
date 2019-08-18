using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Diag;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.CommandLine;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Logging;
using Rocket.Surgery.Hosting;

// [assembly: Convention(typeof(Program))]

namespace Diag
{
    [PublicAPI]
    public static class Program
    {
        public static Task<int> Main(string[] args) => CreateHostBuilder(args).RunCli();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .LaunchWith(RocketBooster.For(DependencyContext.Default))
                .ConfigureRocketSurgery(builder => builder.ConfigureServices(x => { }).ConfigureCommandLine(cl => cl.OnRun(_ => 1)));
    }
}
