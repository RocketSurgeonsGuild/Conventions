using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
public abstract partial class ConventionFactoryBase : IConventionFactory
{
    private static IConventionMetadata FromConvention(IConvention convention)
    {
        var type = convention.GetType();
        var dependencies = type.GetCustomAttributes().OfType<IConventionDependency>();
        var hostType = convention.GetType().GetCustomAttributes().OfType<IHostBasedConvention>().FirstOrDefault()?.HostType ?? HostType.Undefined;
        var category = convention.GetType().GetCustomAttribute<ConventionCategoryAttribute>(true)?.Category ?? ConventionCategory.Application;

        var c = new ConventionMetadata(convention, hostType, category);
        foreach (var dependency in dependencies)
        {
            c.WithDependency(dependency.Direction, dependency.Type);
        }

        return c;
    }

    private static IEnumerable<IConvention> GetConventionsFromAssembly(ConventionContextBuilder builder, Assembly assembly)
    {
        object selector(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
            Type type
        )
        {
            return ActivatorUtilities.CreateInstance(builder.Properties, type);
        }

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

    private IEnumerable<IConvention> GetAssemblyConventions(ConventionContextBuilder builder)
    {
        var assemblyProvider = builder.Properties.GetOrAdd(() => CreateTypeProvider(builder));
        var assemblies = assemblyProvider
                        .GetAssemblies(z => z.FromAssemblyDependenciesOf<IConvention>())
                        .ToImmutableArray();

        return assemblies.SelectMany(z => GetConventionsFromAssembly(builder, z));
    }

    /// <inheritdoc />
    public abstract ICompiledTypeProvider CreateTypeProvider(ConventionContextBuilder builder);

    /// <inheritdoc />
    public IEnumerable<IConventionMetadata> LoadConventions(ConventionContextBuilder builder)
    {
        return GetAssemblyConventions(builder).Select(FromConvention);
    }
}
