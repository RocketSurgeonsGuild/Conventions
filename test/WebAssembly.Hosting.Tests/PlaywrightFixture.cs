using System.Reflection;
using System.Runtime.ExceptionServices;
using DryIoc.ImTools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using Rocket.Surgery.WebAssembly.Hosting.Tests.DevServer;
using Sample.BlazorWasm;
using Xunit;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

public sealed class PlaywrightFixture : IAsyncLifetime
{
    private static void RunInBackgroundThread(Action action)
    {
        var isDone = new ManualResetEvent(false);

        ExceptionDispatchInfo? edi = null;
        new Thread(
            () =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    edi = ExceptionDispatchInfo.Capture(ex);
                }

                isDone.Set();
            }
        ).Start();

        if (!isDone.WaitOne(TimeSpan.FromSeconds(10))) throw new TimeoutException("Timed out waiting for: " + action);

        if (edi != null) throw edi.SourceException;
    }

    public string Uri { get; set; } = null!;
    public IBrowser? Browser { get; set; } = null!;

    public IHost Host { get; set; } = null!;
    public string ContentRoot { get; private set; } = null!;
    private IPlaywright PlaywrightInstance { get; set; } = null!;

    private IHost CreateWebHost()
    {
        ContentRoot = typeof(App)
                     .Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                     .FindFirst(x => x.Key == "ContentRootPath")
                    ?.Value
         ?? throw new InvalidOperationException("ContentRootPath not found");

        var host = "127.0.0.1";
//        if (E2ETestOptions.Instance.SauceTest)
//        {
//            host = E2ETestOptions.Instance.Sauce.HostName;
//        }

        var args = new List<string>
        {
            "--urls", $"http://{host}:0",
            "--contentroot", ContentRoot,
            "--applicationpath", typeof(App).Assembly.Location,
        };

        return DevHostServerProgram.BuildWebHost(args.ToArray());
    }

    public async Task InitializeAsync()
    {
        Host = CreateWebHost();
        RunInBackgroundThread(Host.Start);

        Uri = Host
             .Services.GetRequiredService<IServer>()
             .Features
             .Get<IServerAddressesFeature>()!
             .Addresses.Single();
        PlaywrightInstance = await Playwright.CreateAsync();
        Browser = await PlaywrightInstance.Chromium.LaunchAsync(new() { Headless = true, });
    }

    public async Task DisposeAsync()
    {
        await Host.StopAsync();
        Host.Dispose();
        if (Browser is { })
            await Browser.DisposeAsync();
        PlaywrightInstance.Dispose();
    }

    private class StaticSiteStartup
    {
        public string PathBase { get; } = null!;

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (!string.IsNullOrEmpty(PathBase)) app.UsePathBase(PathBase);

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                }
            );

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapFallbackToFile("index.html"));
        }
    }
}
