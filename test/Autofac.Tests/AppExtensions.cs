using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

internal static class AppExtensions
{
    public static ILifetimeScope GetLifetimeScope(this HostApplicationBuilder builder)
    {
        var host = builder.Build();
        return host.Services.GetRequiredService<ILifetimeScope>();
    }

    public static ILifetimeScope GetLifetimeScope(this WebApplicationBuilder builder)
    {
        var host = builder.Build();
        return host.Services.GetRequiredService<ILifetimeScope>();
    }
}