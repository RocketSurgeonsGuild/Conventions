using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

internal static class AppExtensions
{
    public static IContainer GetLifetimeScope(this HostApplicationBuilder builder)
    {
        var host = builder.Build();
        return host.Services.GetRequiredService<IContainer>();
    }
}