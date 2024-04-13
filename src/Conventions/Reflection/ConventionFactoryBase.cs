using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
public abstract partial class ConventionFactoryBase : IConventionFactory
{
    /// <inheritdoc />
    public abstract IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder);

    /// <inheritdoc />
    public IEnumerable<IConventionWithDependencies> LoadConventions(ConventionContextBuilder builder)
    {
        return GetAssemblyConventions(builder).Select(FromConvention);
    }

    private IEnumerable<IConvention> GetAssemblyConventions(ConventionContextBuilder builder)
    {
        var assemblyProvider = builder.Properties.GetOrAdd(() => CreateAssemblyProvider(builder));
        var assemblies = assemblyProvider
                        .GetAssemblies(z => z.FromAssemblyDependenciesOf<IConvention>())
                        .ToImmutableArray();

        return assemblies.SelectMany(z => GetConventionsFromAssembly(builder, z));
    }

    private static IConventionWithDependencies FromConvention(IConvention convention)
    {
        var type = convention.GetType();
        var dependencies = type.GetCustomAttributes().OfType<IConventionDependency>();
        var hostType = convention.GetType().GetCustomAttributes().OfType<IHostBasedConvention>().FirstOrDefault()?.HostType ?? HostType.Undefined;

        var c =  new ConventionWithDependencies(convention, hostType);
        foreach (var dependency in dependencies)
        {
            c.WithDependency(dependency.Direction, dependency.Type);
        }

        return c;
    }

    private static IEnumerable<IConvention> GetConventionsFromAssembly(ConventionContextBuilder builder, Assembly assembly)
    {
        object selector(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)] Type type
        ) => ActivatorUtilities.CreateInstance(builder.Properties, type);

        var types = assembly
                   .GetCustomAttributes<ExportedConventionsAttribute>()
                   .SelectMany(x => x.ExportedConventions)
                   .Union(assembly.GetCustomAttributes<ConventionAttribute>().Select(z => z.Type))
                   .Distinct()
                   .Select(selector)
                   .Cast<IConvention>();

        foreach (var item in types)
        {
            yield return item;
        }
    }
}