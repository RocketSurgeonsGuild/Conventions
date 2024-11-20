using System.Reflection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
internal record AssemblyProviderAssemblySelector : IReflectionAssemblySelector, IReflectionTypeSelector
{
    public HashSet<Assembly> Assemblies { get; } = new();
    public HashSet<Assembly> ExcludeAssemblies { get; } = new();
    public HashSet<Assembly> AssemblyDependencies { get; } = new();
    public bool AllAssemblies { get; private set; }
    public bool SystemAssemblies { get; private set; }

    public IReflectionTypeSelector FromAssembly()
    {
        Assemblies.Add(Assembly.GetCallingAssembly());
        return this;
    }

    public IReflectionTypeSelector FromAssemblies()
    {
        AllAssemblies = true;
        return this;
    }

    public IReflectionTypeSelector IncludeSystemAssemblies()
    {
        SystemAssemblies = true;
        return this;
    }

    public IReflectionTypeSelector FromAssemblyOf<T>()
    {
        Assemblies.Add(typeof(T).Assembly);
        return this;
    }

    public IReflectionTypeSelector FromAssemblyOf(Type type)
    {
        Assemblies.Add(type.Assembly);
        return this;
    }

    public IReflectionTypeSelector NotFromAssemblyOf<T>()
    {
        ExcludeAssemblies.Add(typeof(T).Assembly);
        return this;
    }

    public IReflectionTypeSelector NotFromAssemblyOf(Type type)
    {
        ExcludeAssemblies.Add(type.Assembly);
        return this;
    }

    public IReflectionTypeSelector FromAssemblyDependenciesOf<T>()
    {
        AssemblyDependencies.Add(typeof(T).Assembly);
        return this;
    }

    public IReflectionTypeSelector FromAssemblyDependenciesOf(Type type)
    {
        AssemblyDependencies.Add(type.Assembly);
        return this;
    }

    IReflectionTypeSelector IReflectionAssemblySelector.FromAssemblies()
    {
        return (IReflectionTypeSelector)FromAssemblies();
    }

    IReflectionTypeSelector IReflectionAssemblySelector.FromAssemblyDependenciesOf<T>()
    {
        return (IReflectionTypeSelector)FromAssemblyDependenciesOf<T>();
    }

    IReflectionTypeSelector IReflectionAssemblySelector.FromAssemblyDependenciesOf(Type type)
    {
        return (IReflectionTypeSelector)FromAssemblyDependenciesOf(type);
    }

    IReflectionTypeSelector IReflectionAssemblySelector.FromAssemblyOf<T>()
    {
        return (IReflectionTypeSelector)FromAssemblyOf<T>();
    }

    IReflectionTypeSelector IReflectionAssemblySelector.FromAssemblyOf(Type type)
    {
        return (IReflectionTypeSelector)FromAssemblyOf(type);
    }

    IReflectionTypeSelector IReflectionAssemblySelector.NotFromAssemblyOf<T>()
    {
        return (IReflectionTypeSelector)NotFromAssemblyOf<T>();
    }

    IReflectionTypeSelector IReflectionAssemblySelector.NotFromAssemblyOf(Type type)
    {
        return (IReflectionTypeSelector)NotFromAssemblyOf(type);
    }

    IReflectionTypeSelector IReflectionAssemblySelector.FromAssembly()
    {
        return (IReflectionTypeSelector)FromAssembly();
    }

    IReflectionTypeSelector IReflectionAssemblySelector.IncludeSystemAssemblies()
    {
        return (IReflectionTypeSelector)IncludeSystemAssemblies();
    }

    IEnumerable<Type> IReflectionTypeSelector.GetTypes()
    {
        return Enumerable.Empty<Type>();
    }

    IEnumerable<Type> IReflectionTypeSelector.GetTypes(bool publicOnly)
    {
        return Enumerable.Empty<Type>();
    }

    IEnumerable<Type> IReflectionTypeSelector.GetTypes(Action<ITypeFilter> action)
    {
        return Enumerable.Empty<Type>();
    }

    IEnumerable<Type> IReflectionTypeSelector.GetTypes(bool publicOnly, Action<ITypeFilter> action)
    {
        return Enumerable.Empty<Type>();
    }
}
