using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.CommandLine;

public interface ICommandAppServiceProviderFactory
{
    IServiceProvider CreateServiceProvider(IServiceCollection services, IConventionContext conventionContext);
}