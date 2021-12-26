namespace Rocket.Surgery.Conventions.CommandLine;

class FallbackServiceProvider : IServiceProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProvider _fallbackServiceProvider;

    public FallbackServiceProvider(IServiceProvider serviceProvider, IServiceProvider fallbackServiceProvider)
    {
        _serviceProvider = serviceProvider;
        _fallbackServiceProvider = fallbackServiceProvider;
    }

    public object? GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType) ?? _fallbackServiceProvider.GetService(serviceType);
    }
}