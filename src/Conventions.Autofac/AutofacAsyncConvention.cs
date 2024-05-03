using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Autofac;

/// <summary>
///     Delegate AutofacAsyncConvention
/// </summary>
/// <param name="context"></param>
/// <param name="configuration"></param>
/// <param name="services"></param>
/// <param name="container"></param>
/// <param name="cancellationToken"></param>
public delegate ValueTask AutofacAsyncConvention(
    IConventionContext context,
    IConfiguration configuration,
    IServiceCollection services,
    ContainerBuilder container,
    CancellationToken cancellationToken
);