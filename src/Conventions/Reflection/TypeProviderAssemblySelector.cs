using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
record TypeProviderAssemblySelector : ITypeSelector
{
    public bool PublicTypes { get; internal set; }
    public Action<ITypeFilter>? Filter { get; internal set; }
    public IEnumerable<Assembly> Assemblies { get; init; } = Enumerable.Empty<Assembly>();

    public ITypeSelector FromAssembly() => this;
    public ITypeSelector FromAssemblies() => this;
    public ITypeSelector IncludeSystemAssemblies() => this;
    public ITypeSelector FromAssemblyOf<T>() => this;
    public ITypeSelector FromAssemblyOf(Type type) => this;
    public ITypeSelector NotFromAssemblyOf<T>() => this;
    public ITypeSelector NotFromAssemblyOf(Type type) => this;
    public ITypeSelector FromAssemblyDependenciesOf<T>() => this;
    public ITypeSelector FromAssemblyDependenciesOf(Type type) => this;

    public IEnumerable<Type> GetTypes() => GetTypesInternal();

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
        if (this is not { Filter: { } filterAction }) return types;

        var filter = new TypeFilter();
        filterAction(filter);
        return filter.Filters.Aggregate(types, (current, f) => current.Where(f));
    }
}
