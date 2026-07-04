using System.Reflection;

namespace Clavus.Infrastructure;

internal partial class ClavusContextState
{
    private readonly List<object?> _conventions = [null];
    private readonly List<Type> _exceptConventions = [];
    private readonly List<Assembly> _exceptAssemblyConventions = [];
    public ServiceProviderFactoryAdapter? ServiceProviderFactory { get; set; }

    public void AppendParts(params IEnumerable<object> conventions) => _conventions.AddRange(conventions);

    public void PrependParts(params IEnumerable<object> conventions) => _conventions.InsertRange(0, conventions);

    public void ExceptConventions(params IEnumerable<Type> types) => _exceptConventions.AddRange(types);

    public void ExceptConventions(params IEnumerable<Assembly> assemblies) => _exceptAssemblyConventions.AddRange(assemblies);

    public List<object?> GetConventions() => _conventions;

    internal IEnumerable<IClavusPartMetadata> CalculateConventions(ClavusContextBuilder builder, LoadClavusParts factory)
    {
        return factory(builder)
              .Where(z => _exceptConventions.All(x => x != z.Convention.GetType()))
              .Where(z => _exceptAssemblyConventions.All(x => x != z.Convention.GetType().Assembly));
    }
}
