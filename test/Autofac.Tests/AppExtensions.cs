using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

internal static class AppExtensions
{
    public static ILifetimeScope GetLifetimeScope(this IHostBuilder builder)
    {
        var host = builder.Build();
        return host.Services.GetRequiredService<ILifetimeScope>();
    }
}
