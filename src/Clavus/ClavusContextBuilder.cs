using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Clavus.Infrastructure;
using PropertiesDictionary = System.Collections.Generic.Dictionary<object, object>;
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;

namespace Clavus;

/// <summary>
///     Builder that can be used to create a context.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class ClavusContextBuilder
{
    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="conventions"></param>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    public static ClavusContextBuilder Create(IEnumerable<IClavusPartMetadata> conventions, PropertiesType properties, IEnumerable<ClavusCategory> categories) =>
        new(conventions, properties, categories);

    private static readonly string[] categoryEnvironmentVariables = ["CLAVUS__CATEGORY", "CLAVUS__CATEGORIES"];

    private readonly UniqueQueue _conventions;
    private static readonly string[] hostTypeEnvironmentVariables = ["CLAVUS__HOSTTYPE"];

    /// <summary>
    ///     Create a context builder with a set of properties
    /// </summary>
    /// <param name="conventions"></param>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    private ClavusContextBuilder(IEnumerable<IClavusPartMetadata> conventions, PropertiesType? properties, IEnumerable<ClavusCategory> categories)
    {
        Properties = new ServiceProviderDictionary(properties ?? new PropertiesDictionary());
        _conventions = new();
        _conventions.Append(conventions);

        foreach (var variable in hostTypeEnvironmentVariables)
        {
            if (Environment.GetEnvironmentVariable(variable) is { Length: > 0 } hostType && Enum.TryParse<HostType>(hostType, true, out var type)) Properties[typeof(HostType)] = type;
        }

        List<ClavusCategory> categoriesBuilder = [.. categories];
        foreach (var variable in categoryEnvironmentVariables)
        {
            if (Environment.GetEnvironmentVariable(variable) is not { Length: > 0 } category) continue;
            categoriesBuilder.AddRange(category.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(item => new ClavusCategory(item)));
        }

        Categories = ImmutableHashSet.CreateRange(ClavusCategory.ValueComparer, categoriesBuilder);
    }

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    public ImmutableHashSet<ClavusCategory> Categories { get; }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    public IServiceProviderDictionary Properties { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString();

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    public ClavusContextBuilder AppendPart(params IEnumerable<IClavusPartMetadata> conventions)
    {
        _conventions.Append(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder PrependPart(params IEnumerable<IClavusPartMetadata> conventions)
    {
        _conventions.Prepend(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    public ClavusContextBuilder AppendPart(params IEnumerable<IClavusPart> conventions)
    {
        _conventions.Append(conventions.Select(FromConvention));
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder PrependPart(params IEnumerable<IClavusPart> conventions)
    {
        _conventions.Prepend(conventions.Select(FromConvention));
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder AppendPart<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : IClavusPart
    {
        _conventions.Append(FromConvention<T>(Properties is IReadOnlyServiceProviderDictionary ro ? ro : new ReadOnlyServiceProviderDictionary(Properties)));
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder PrependPart<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : IClavusPart
    {
        _conventions.Prepend(FromConvention<T>(Properties is IReadOnlyServiceProviderDictionary ro ? ro : new ReadOnlyServiceProviderDictionary(Properties)));
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder ExceptConvention(params IEnumerable<Assembly> assemblies)
    {
        var set = assemblies.ToHashSet();
        _conventions.Remove(a => set.Contains(a.Convention.GetType().Assembly));
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder ExceptConvention(params IEnumerable<Type> types)
    {
        var set = types.ToHashSet();
        _conventions.Remove(a => set.Contains(a.Convention.GetType()));
        return this;
    }

    internal IEnumerable<IClavusPartMetadata> Ashlar() => _conventions;

    private static IClavusPartMetadata FromConvention(IClavusPart part)
    {
        var type = part.GetType();
        var dependencies = type.GetCustomAttributes().OfType<IClavusDependency>().ToArray();
        var hostType = type.GetCustomAttributes().OfType<IHostBasedPart>().FirstOrDefault()?.HostType
         ?? type.GetCustomAttribute<ClavusHostTypeAttribute>()?.HostType
         ?? HostType.Undefined;
        var category = type.GetCustomAttribute<ClavusCategoryAttribute>()?.Category ?? ClavusCategory.Application;


        return new ClavusPartMetadata(part, hostType, category) { Dependencies = dependencies };
    }

    private static IClavusPartMetadata FromConvention<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(IReadOnlyServiceProviderDictionary properties)
        where T : IClavusPart
    {
        var type = typeof(T);
        var dependencies = type.GetCustomAttributes().OfType<IClavusDependency>().ToArray();
        var hostType = type.GetCustomAttributes().OfType<IHostBasedPart>().FirstOrDefault()?.HostType
         ?? type.GetCustomAttribute<ClavusHostTypeAttribute>()?.HostType
         ?? HostType.Undefined;
        var category = type.GetCustomAttribute<ClavusCategoryAttribute>()?.Category ?? ClavusCategory.Application;


        return new ClavusDeferredPartMetadata<T>(properties, hostType, category) { Dependencies = dependencies };
    }

    private class UniqueQueue : IEnumerable<IClavusPartMetadata>
    {
        private readonly List<IClavusPartMetadata> _list = [];

        public void Append(params IEnumerable<IClavusPartMetadata> conventions) => _list.AddRange(conventions);

        public void Prepend(params IEnumerable<IClavusPartMetadata> conventions) => _list.InsertRange(0, conventions);

        public void Remove(Func<IClavusPartMetadata, bool> predicate) => _list.RemoveAll(a => predicate(a));

        IEnumerator<IClavusPartMetadata> IEnumerable<IClavusPartMetadata>.GetEnumerator() => _list.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
