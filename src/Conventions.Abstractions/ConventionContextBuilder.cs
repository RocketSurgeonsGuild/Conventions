using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PropertiesDictionary = System.Collections.Generic.Dictionary<object, object>;
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Builder that can be used to create a context.
/// </summary>
[PublicAPI]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class ConventionContextBuilder
{
    internal readonly ConventionContextState state;

    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <returns></returns>
    public static ConventionContextBuilder Create(LoadConventions conventionFactory) =>
        new(conventionFactory, new PropertiesDictionary(), []);

    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    [OverloadResolutionPriority(-1)]
    public static ConventionContextBuilder Create(LoadConventions conventionFactory, PropertiesType? properties, params ConventionCategory[] categories) =>
        new(conventionFactory, properties ?? new PropertiesDictionary(), categories);

    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    public static ConventionContextBuilder Create(LoadConventions conventionFactory, PropertiesType? properties, params IEnumerable<ConventionCategory> categories) =>
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
    private ConventionContextBuilder(LoadConventions conventionFactory, PropertiesType? properties, IEnumerable<ConventionCategory> categories)
    {
        Properties = new ServiceProviderDictionary(properties ?? new PropertiesDictionary());
        Properties.Set(conventionFactory);
        state = new();
        Properties.Set(state);

        foreach (var variable in hostTypeEnvironmentVariables)
        {
            if (Environment.GetEnvironmentVariable(variable) is { Length: > 0 } hostType && Enum.TryParse<HostType>(hostType, true, out var type)) Properties[typeof(HostType)] = type;
        }

        List<ConventionCategory> categoriesBuilder = [.. categories];
        foreach (var variable in categoryEnvironmentVariables)
        {
            if (Environment.GetEnvironmentVariable(variable) is not { Length: > 0 } category) continue;
            categoriesBuilder.AddRange(category.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(item => new ConventionCategory(item)));
        }

        Categories = new(categoriesBuilder, ConventionCategory.ValueComparer);
    }

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    public HashSet<ConventionCategory> Categories { get; }

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
    public ConventionContextBuilder AppendConvention(params IConvention[] conventions)
    {
        state.AppendConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    public ConventionContextBuilder AppendConvention(params IEnumerable<IConvention> conventions)
    {
        state.AppendConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ConventionContextBuilder AppendConvention(params Type[] conventions)
    {
        state.AppendConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(params IEnumerable<Type> conventions)
    {
        state.AppendConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention<T>()
        where T : IConvention
    {
        state.AppendConventions(typeof(T));
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ConventionContextBuilder PrependConvention(params IConvention[] conventions)
    {
        state.PrependConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(params IEnumerable<IConvention> conventions)
    {
        state.PrependConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ConventionContextBuilder PrependConvention(params Type[] conventions)
    {
        state.PrependConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(params IEnumerable<Type> conventions)
    {
        state.PrependConventions(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention<T>()
        where T : IConvention
    {
        state.PrependConventions(typeof(T));
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendDelegate(Delegate @delegate, int? priority, ConventionCategory? category)
    {
        state.AppendConventions(new ConventionOrDelegate(@delegate, priority ?? 0, category));
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependDelegate(Delegate @delegate, int? priority, ConventionCategory? category)
    {
        state.PrependConventions(new ConventionOrDelegate(@delegate, priority ?? 0, category));
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ConventionContextBuilder ExceptConvention(params Assembly[] assemblies)
    {
        state.ExceptConventions(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(params IEnumerable<Assembly> assemblies)
    {
        state.ExceptConventions(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    [OverloadResolutionPriority(-1)]
    public ConventionContextBuilder ExceptConvention(params Type[] types)
    {
        state.ExceptConventions(types);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(params IEnumerable<Type> types)
    {
        state.ExceptConventions(types);
        return this;
    }
}
