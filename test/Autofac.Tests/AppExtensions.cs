using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

internal static class AppExtensions
{
    public static ILifetimeScope GetLifetimeScope(this IHost host)
    {
        return host.Services.GetRequiredService<ILifetimeScope>();
    }
}
