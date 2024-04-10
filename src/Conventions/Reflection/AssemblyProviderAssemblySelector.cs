using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

record AssemblyProviderAssemblySelector : IAssemblyProviderAssemblySelector, ITypeSelector
{
    public HashSet<Assembly> Assemblies { get; } = new();
    public HashSet<Assembly> ExcludeAssemblies { get; } = new();
    public HashSet<Assembly> AssemblyDependencies { get; } = new();
    public bool AllAssemblies { get; private set; }

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

    ITypeSelector ITypeProviderAssemblySelector.FromAssemblies() => (ITypeSelector)FromAssemblies();
    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyDependenciesOf<T>() => (ITypeSelector)FromAssemblyDependenciesOf<T>();
    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyDependenciesOf(Type type) => (ITypeSelector)FromAssemblyDependenciesOf(type);
    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyOf<T>() => (ITypeSelector)FromAssemblyOf<T>();
    ITypeSelector ITypeProviderAssemblySelector.FromAssemblyOf(Type type) => (ITypeSelector)FromAssemblyOf(type);
    ITypeSelector ITypeProviderAssemblySelector.NotFromAssemblyOf<T>() => (ITypeSelector)NotFromAssemblyOf<T>();
    ITypeSelector ITypeProviderAssemblySelector.NotFromAssemblyOf(Type type) => (ITypeSelector)NotFromAssemblyOf(type);
    ITypeSelector ITypeProviderAssemblySelector.FromAssembly() => (ITypeSelector)FromAssembly();
    IEnumerable<Type> ITypeSelector.GetTypes() => Enumerable.Empty<Type>();
    IEnumerable<Type> ITypeSelector.GetTypes(bool publicOnly) => Enumerable.Empty<Type>();
    IEnumerable<Type> ITypeSelector.GetTypes(Action<ITypeFilter> action) => Enumerable.Empty<Type>();
    IEnumerable<Type> ITypeSelector.GetTypes(bool publicOnly, Action<ITypeFilter> action) => Enumerable.Empty<Type>();
}
