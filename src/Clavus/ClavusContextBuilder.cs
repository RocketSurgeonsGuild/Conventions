using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    internal readonly ClavusContextState state;

    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <returns></returns>
    public static ClavusContextBuilder Create(LoadClavusParts conventionFactory) =>
        new(conventionFactory, new PropertiesDictionary(), []);

    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    [OverloadResolutionPriority(-1)]
    public static ClavusContextBuilder Create(LoadClavusParts conventionFactory, PropertiesType? properties, params ClavusCategory[] categories) =>
        new(conventionFactory, properties ?? new PropertiesDictionary(), categories);

    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    public static ClavusContextBuilder Create(LoadClavusParts conventionFactory, PropertiesType? properties, params IEnumerable<ClavusCategory> categories) =>
        new(conventionFactory, properties ?? new PropertiesDictionary(), categories);

    private static readonly string[] categoryEnvironmentVariables =
        ["ROCKETSURGERYCONVENTIONS__CATEGORY", "ROCKETSURGERYCONVENTIONS__CATEGORIES", "RSG__CATEGORY", "RSG__CATEGORIES"];

    private static readonly string[] hostTypeEnvironmentVariables = ["RSG__HOSTTYPE", "ROCKETSURGERYCONVENTIONS__HOSTTYPE"];

    /// <summary>
    ///     Create a context builder with a set of properties
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    private ClavusContextBuilder(LoadClavusParts conventionFactory, PropertiesType? properties, IEnumerable<ClavusCategory> categories)
    {
        Properties = new ServiceProviderDictionary(properties ?? new PropertiesDictionary());
        Properties.Set(conventionFactory);
        state = new();
        Properties.Set(state);

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

        Categories = new(categoriesBuilder, ClavusCategory.ValueComparer);
    }

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    public HashSet<ClavusCategory> Categories { get; }

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
    [OverloadResolutionPriority(-1)]
    public ClavusContextBuilder AppendPart(params IClavusPart[] conventions)
    {
        state.AppendParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    public ClavusContextBuilder AppendPart(params IEnumerable<IClavusPart> conventions)
    {
        state.AppendParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ClavusContextBuilder AppendPart(params Type[] conventions)
    {
        state.AppendParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder AppendPart(params IEnumerable<Type> conventions)
    {
        state.AppendParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder AppendPart<T>()
        where T : IClavusPart
    {
        state.AppendParts(typeof(T));
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ClavusContextBuilder PrependPart(params IClavusPart[] conventions)
    {
        state.PrependParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder PrependPart(params IEnumerable<IClavusPart> conventions)
    {
        state.PrependParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ClavusContextBuilder PrependPart(params Type[] conventions)
    {
        state.PrependParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder PrependPart(params IEnumerable<Type> conventions)
    {
        state.PrependParts(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder PrependPart<T>()
        where T : IClavusPart
    {
        state.PrependParts(typeof(T));
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder AppendDelegate(Delegate @delegate, int? priority, ClavusCategory? category)
    {
        state.AppendParts(new ClavusOrDelegate(@delegate, priority ?? 0, category));
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder PrependDelegate(Delegate @delegate, int? priority, ClavusCategory? category)
    {
        state.PrependParts(new ClavusOrDelegate(@delegate, priority ?? 0, category));
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ClavusContextBuilder ExceptConvention(params Assembly[] assemblies)
    {
        state.ExceptConventions(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder ExceptConvention(params IEnumerable<Assembly> assemblies)
    {
        state.ExceptConventions(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ClavusContextBuilder ExceptConvention(params Type[] types)
    {
        state.ExceptConventions(types);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public ClavusContextBuilder ExceptConvention(params IEnumerable<Type> types)
    {
        state.ExceptConventions(types);
        return this;
    }
}
