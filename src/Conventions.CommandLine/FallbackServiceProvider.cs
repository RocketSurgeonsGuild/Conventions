namespace Rocket.Surgery.Conventions.CommandLine;

class FallbackServiceProvider(IServiceProvider serviceProvider, IServiceProvider fallbackServiceProvider) : IServiceProvider
{
    public object? GetService(Type serviceType)
    {
        return serviceProvider.GetService(serviceType) ?? fallbackServiceProvider.GetService(serviceType);
    }
}
