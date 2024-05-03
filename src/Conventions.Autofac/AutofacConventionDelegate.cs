using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

/// <summary>
///     Delegate AutofacConvention
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="services"></param>
/// <param name="container"></param>
public delegate void AutofacConvention(
    IConventionContext context,
    IConfiguration configuration,
    IServiceCollection services,
    ContainerBuilder container
);
