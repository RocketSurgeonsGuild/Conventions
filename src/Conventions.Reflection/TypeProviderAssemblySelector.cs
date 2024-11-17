using System.Reflection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
internal record TypeProviderAssemblySelector : IReflectionTypeSelector
{
    public bool PublicTypes { get; internal set; }
    public Action<ITypeFilter>? Filter { get; internal set; }
    public IEnumerable<Assembly> Assemblies { get; init; } = [];

    internal IEnumerable<Type> GetTypesInternal()
    {
        var types = Assemblies
           .SelectMany(
                x =>
                {
                    try
                    {
                        return PublicTypes
                            ? x.GetExportedTypes()
                            : x.GetTypes();
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                }
            );
        if (this is not { Filter: { } filterAction, }) return types;

        var filter = new TypeFilter();
        filterAction(filter);
        return filter.Filters.Aggregate(types, (current, f) => current.Where(f));
    }

    public IReflectionTypeSelector FromAssembly()
    {
        return this;
    }

    public IReflectionTypeSelector FromAssemblies()
    {
        return this;
    }

    public IReflectionTypeSelector IncludeSystemAssemblies()
    {
        return this;
    }

    public IReflectionTypeSelector FromAssemblyOf<T>()
    {
        return this;
    }

    public IReflectionTypeSelector FromAssemblyOf(Type type)
    {
        return this;
    }

    public IReflectionTypeSelector NotFromAssemblyOf<T>()
    {
        return this;
    }

    public IReflectionTypeSelector NotFromAssemblyOf(Type type)
    {
        return this;
    }

    public IReflectionTypeSelector FromAssemblyDependenciesOf<T>()
    {
        return this;
    }

    public IReflectionTypeSelector FromAssemblyDependenciesOf(Type type)
    {
        return this;
    }

    public IEnumerable<Type> GetTypes()
    {
        return GetTypesInternal();
    }

    public IEnumerable<Type> GetTypes(bool publicOnly)
    {
        PublicTypes = publicOnly;
        return GetTypesInternal();
    }

    public IEnumerable<Type> GetTypes(Action<ITypeFilter> action)
    {
        Filter = action;
        return GetTypesInternal();
    }

    public IEnumerable<Type> GetTypes(bool publicOnly, Action<ITypeFilter> action)
    {
        PublicTypes = publicOnly;
        Filter = action;
        return GetTypesInternal();
    }
}
