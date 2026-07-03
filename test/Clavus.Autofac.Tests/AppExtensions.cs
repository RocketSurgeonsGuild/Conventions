using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Clavus.Autofac.Tests;

internal static class AppExtensions
{
    public static ILifetimeScope GetLifetimeScope(this IHost host) => host.Services.GetRequiredService<ILifetimeScope>();
}
