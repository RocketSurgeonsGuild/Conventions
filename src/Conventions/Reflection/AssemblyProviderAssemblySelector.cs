using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

record AssemblyProviderAssemblySelector : IAssemblyProviderAssemblySelector
{
    public HashSet<Assembly> Assemblies { get; } = new();
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
}
