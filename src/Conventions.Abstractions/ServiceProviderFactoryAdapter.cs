using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions;

internal delegate ValueTask<IServiceProviderFactory<object>> ServiceProviderFactoryAdapter(
    IConventionContext context,
    IServiceCollection services,
    CancellationToken cancellationToken
);

/// <summary>
///     A factory that provides a list of conventions
/// </summary>
public delegate IEnumerable<IConventionWithDependencies> ConventionProviderFactory(IServiceProviderDictionary context);