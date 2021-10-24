using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Extensions to support dependency context
/// </summary>
public static class DependencyContextConventionContextBuilderExtensions
{
    internal static IAssemblyCandidateFinder dependencyContextAssemblyCandidateFinderFactory(object? source, ILogger? logger)
    {
        return source switch
        {
            DependencyContext dependencyContext => new DependencyContextAssemblyCandidateFinder(dependencyContext, logger),
            AppDomain appDomain                 => new AppDomainAssemblyCandidateFinder(appDomain, logger),
            IEnumerable<Assembly> assemblies    => new DefaultAssemblyCandidateFinder(assemblies, logger),
            _                                   => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder")
        };
    }

    internal static IAssemblyProvider dependencyContextAssemblyProviderFactory(object? source, ILogger? logger)
    {
        return source switch
        {
            DependencyContext dependencyContext => new DependencyContextAssemblyProvider(dependencyContext, logger),
            AppDomain appDomain                 => new AppDomainAssemblyProvider(appDomain, logger),
            IEnumerable<Assembly> assemblies    => new DefaultAssemblyProvider(assemblies, logger),
            _                                   => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder")
        };
    }

    /// <summary>
    ///     Use the given dependency context for resolving assemblies
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="dependencyContext"></param>
    /// <returns></returns>
    public static ConventionContextBuilder UseDependencyContext(this ConventionContextBuilder builder, DependencyContext dependencyContext)
    {
        builder._source = dependencyContext;
        builder._assemblyProviderFactory = dependencyContextAssemblyProviderFactory;
        builder._assemblyCandidateFinderFactory = dependencyContextAssemblyCandidateFinderFactory;
        return builder;
    }
}
