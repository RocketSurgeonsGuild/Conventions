using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Hosting;

#pragma warning disable CA1822
#pragma warning disable CA1724
#pragma warning disable CA1707

namespace Hosting.Benchmarks;

internal static class Program
{
    private static void Main()
    {
        var config = ManualConfig
           .Create(DefaultConfig.Instance);
        config.AddJob(
            Job.ShortRun.WithToolchain(
                InProcessEmitToolchain
                   .Instance // Run in-process so you don't need a solution file. If this is part of a solution, replace with CsProjCoreToolchain.NetCoreApp21.
            )
        );

        BenchmarkRunner.Run<Benchmarks>(config);
    }
}

/// <summary>
///     The benchmarks to run
/// </summary>
public class Benchmarks
{
    [Benchmark]
    public async Task Default_Hosting()
    {
        using var host = Host
                        .CreateDefaultBuilder()
                        .ConfigureLogging(x => x.ClearProviders())
                        .Build();
        await host.StartAsync().ConfigureAwait(false);
        await host.StopAsync().ConfigureAwait(false);
    }

    [Benchmark(Baseline = true)]
    public async Task Default_Hosting_Application()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        var host = builder.Build();
        await host.StartAsync().ConfigureAwait(false);
        await host.StopAsync().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task Rocket_Surgery_Hosting_Application()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        var host = await builder.ConfigureRocketSurgery();
        await host.StartAsync().ConfigureAwait(false);
        await host.StopAsync().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task Default_Hosting_With_Service()
    {
        using var host = Host
                        .CreateDefaultBuilder()
                        .ConfigureLogging(x => x.ClearProviders())
                        .ConfigureServices(x => x.AddHostedService<HostedService>())
                        .Build();
        await host.StartAsync().ConfigureAwait(false);
        await host.StopAsync().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task Rocket_Surgery_Hosting_Application_With_Service()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddHostedService<HostedService>();
        builder.Logging.ClearProviders();
        var host = await builder.ConfigureRocketSurgery();
        await host.StartAsync().ConfigureAwait(false);
        await host.StopAsync().ConfigureAwait(false);
    }

    private class HostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(TimeSpan.FromTicks(1), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
