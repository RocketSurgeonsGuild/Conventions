using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests;

internal sealed class Contrib : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services) { }
}