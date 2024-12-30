using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions;

internal partial class ConventionContextState
{
    private readonly List<object?> _conventions = [null];
    private readonly List<Type> _exceptConventions = [];
    private readonly List<Assembly> _exceptAssemblyConventions = [];
    public ServiceProviderFactoryAdapter? ServiceProviderFactory { get; set; }

    public void AppendConventions(params IEnumerable<object> conventions)
    {
        _conventions.AddRange(conventions);
    }

    public void PrependConventions(params IEnumerable<object> conventions)
    {
        _conventions.InsertRange(0, conventions);
    }

    public void ExceptConventions(params IEnumerable<Type> types)
    {
        _exceptConventions.AddRange(types);
    }

    public void ExceptConventions(params IEnumerable<Assembly> assemblies)
    {
        _exceptAssemblyConventions.AddRange(assemblies);
    }

    public List<object?> GetConventions() => _conventions;

    internal IEnumerable<IConventionMetadata> CalculateConventions(ConventionContextBuilder builder, LoadConventions factory)
    {
        return factory(builder)
              .Where(z => _exceptConventions.All(x => x != z.Convention.GetType()))
              .Where(z => _exceptAssemblyConventions.All(x => x != z.Convention.GetType().Assembly));
    }
}
