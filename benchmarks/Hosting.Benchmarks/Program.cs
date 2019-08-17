using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Rocket.Surgery.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyModel;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Hosting.Benchmarks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var config = ManualConfig
                .Create(DefaultConfig.Instance);
            config.Add(
                Job.ShortRun.With(
                    InProcessEmitToolchain.Instance  // Run in-process so you don't need a solution file. If this is part of a solution, replace with CsProjCoreToolchain.NetCoreApp21.
                )
            );

            BenchmarkRunner.Run<Benchmarks>(config);
        }
    }

    /// <summary>
    /// The benchmarks to run
    /// </summary>
    public class Benchmarks
    {
        [Benchmark(Baseline = true)]
        public async Task Default_Hosting()
        {
            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureLogging(x => x.ClearProviders())
                .Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        [Benchmark]
        public async Task Rocket_Surgery_Hosting()
        {
            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .LaunchWith(RocketBooster.For(DependencyContext.Default))
                .ConfigureLogging(x => x.ClearProviders())
                .Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        class HostedService : IHostedService
        {
            public Task StartAsync(CancellationToken cancellationToken) => Task.Delay(TimeSpan.FromMilliseconds(1));
            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        }

        [Benchmark]
        public async Task Default_Hosting_With_Service()
        {
            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureLogging(x => x.ClearProviders())
                .ConfigureServices(x => x.AddHostedService<HostedService>())
                .Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        [Benchmark]
        public async Task Rocket_Surgery_Hosting_With_Service()
        {
            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .LaunchWith(RocketBooster.For(DependencyContext.Default))
                .ConfigureLogging(x => x.ClearProviders())
                .ConfigureServices(x => x.AddHostedService<HostedService>())
                .Build();
            await host.StartAsync();
            await host.StopAsync();
        }
    }
}
