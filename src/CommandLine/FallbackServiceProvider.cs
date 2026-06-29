namespace Rocket.Surgery.CommandLine;

internal class FallbackServiceProvider(IServiceProvider serviceProvider, IServiceProvider fallbackServiceProvider) : IServiceProvider
{
    public object? GetService(Type serviceType) => serviceProvider.GetService(serviceType) ?? fallbackServiceProvider.GetService(serviceType);
}
