using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
record TypeProviderAssemblySelector : ITypeSelector
{
    public HashSet<Assembly> Assemblies { get; } = new();
    public HashSet<Assembly> AssemblyDependencies { get; } = new();
    public bool AllAssemblies { get; private set; }
    public bool PublicTypes { get; internal set; }
    public Action<ITypeFilter>? Filter { get; internal set; }


    public ITypeSelector FromAssembly()
    {
        Assemblies.Add(Assembly.GetCallingAssembly());
        return this;
    }

    public ITypeSelector FromAssemblies()
    {
        AllAssemblies = true;
        return this;
    }

    public ITypeSelector FromAssemblyOf<T>()
    {
        Assemblies.Add(typeof(T).Assembly);
        return this;
    }

    public ITypeSelector FromAssemblyOf(Type type)
    {
        Assemblies.Add(type.Assembly);
        return this;
    }

    public ITypeSelector FromAssemblyDependenciesOf<T>()
    {
        AssemblyDependencies.Add(typeof(T).Assembly);
        return this;
    }

    public ITypeSelector FromAssemblyDependenciesOf(Type type)
    {
        AssemblyDependencies.Add(type.Assembly);
        return this;
    }

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

    private IEnumerable<Type> GetTypesInternal()
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
