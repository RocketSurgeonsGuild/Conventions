using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
internal record AssemblyProviderAssemblySelector : IAssemblyProviderAssemblySelector, ITypeSelector
{
    public HashSet<Assembly> Assemblies { get; } = new();
    public HashSet<Assembly> ExcludeAssemblies { get; } = new();
    public HashSet<Assembly> AssemblyDependencies { get; } = new();
    public bool AllAssemblies { get; private set; }
    public bool SystemAssemblies { get; private set; }

    public IAssemblyProviderAssemblySelector FromAssembly()
    {
        Assemblies.Add(Assembly.GetCallingAssembly());
        return this;
    }

    public IAssemblyProviderAssemblySelector FromAssemblies()
    {
        AllAssemblies = true;
        return this;
    }

    public IAssemblyProviderAssemblySelector IncludeSystemAssemblies()
    {
        SystemAssemblies = true;
        return this;
    }

    public IAssemblyProviderAssemblySelector FromAssemblyOf<T>()
    {
        Assemblies.Add(typeof(T).Assembly);
        return this;
    }

    public IAssemblyProviderAssemblySelector FromAssemblyOf(Type type)
    {
        Assemblies.Add(type.Assembly);
        return this;
    }

    public IAssemblyProviderAssemblySelector NotFromAssemblyOf<T>()
    {
        ExcludeAssemblies.Add(typeof(T).Assembly);
        return this;
    }

    public IAssemblyProviderAssemblySelector NotFromAssemblyOf(Type type)
    {
        ExcludeAssemblies.Add(type.Assembly);
        return this;
    }

    public IAssemblyProviderAssemblySelector FromAssemblyDependenciesOf<T>()
    {
        AssemblyDependencies.Add(typeof(T).Assembly);
        return this;
    }

    public IAssemblyProviderAssemblySelector FromAssemblyDependenciesOf(Type type)
    {
        AssemblyDependencies.Add(type.Assembly);
        return this;
    }

    ITypeSelector ITypeProviderAssemblySelector.FromAssemblies()
    {
        return (ITypeSelector)FromAssemblies();
    }

    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyDependenciesOf<T>()
    {
        return (ITypeSelector)FromAssemblyDependenciesOf<T>();
    }

    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyDependenciesOf(Type type)
    {
        return (ITypeSelector)FromAssemblyDependenciesOf(type);
    }

    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyOf<T>()
    {
        return (ITypeSelector)FromAssemblyOf<T>();
    }

    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyOf(Type type)
    {
        return (ITypeSelector)FromAssemblyOf(type);
    }

    ITypeSelector ITypeProviderAssemblySelector.NotFromAssemblyOf<T>()
    {
        return (ITypeSelector)NotFromAssemblyOf<T>();
    }

    ITypeSelector ITypeProviderAssemblySelector.NotFromAssemblyOf(Type type)
    {
        return (ITypeSelector)NotFromAssemblyOf(type);
    }

    ITypeSelector ITypeProviderAssemblySelector.FromAssembly()
    {
        return (ITypeSelector)FromAssembly();
    }

    ITypeSelector ITypeProviderAssemblySelector.IncludeSystemAssemblies()
    {
        return (ITypeSelector)IncludeSystemAssemblies();
    }

    IEnumerable<Type> ITypeSelector.GetTypes()
    {
        return Enumerable.Empty<Type>();
    }

    IEnumerable<Type> ITypeSelector.GetTypes(bool publicOnly)
    {
        return Enumerable.Empty<Type>();
    }

    IEnumerable<Type> ITypeSelector.GetTypes(Action<ITypeFilter> action)
    {
        return Enumerable.Empty<Type>();
    }

    IEnumerable<Type> ITypeSelector.GetTypes(bool publicOnly, Action<ITypeFilter> action)
    {
        return Enumerable.Empty<Type>();
    }
}