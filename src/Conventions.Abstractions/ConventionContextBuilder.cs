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
public class ConventionContextBuilder
{
    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    [OverloadResolutionPriority(-1)]
    public static ConventionContextBuilder Create(PropertiesType? properties = null, params ConventionCategory[] categories) =>
        new(properties ?? new PropertiesDictionary(), categories);

    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    public static ConventionContextBuilder Create(PropertiesType? properties = null, params IEnumerable<ConventionCategory> categories) =>
        new(properties ?? new PropertiesDictionary(), categories);

    private static readonly string[] categoryEnvironmentVariables =
        ["ROCKETSURGERYCONVENTIONS__CATEGORY", "ROCKETSURGERYCONVENTIONS__CATEGORIES", "RSG__CATEGORY", "RSG__CATEGORIES"];

    private static readonly string[] hostTypeEnvironmentVariables = ["RSG__HOSTTYPE", "ROCKETSURGERYCONVENTIONS__HOSTTYPE"];

    // this null is used a marker to indicate where in the list is the middle
    // ReSharper disable once NullableWarningSuppressionIsUsed
    internal readonly List<object?> _conventions = [null!];
    internal readonly List<Type> _exceptConventions = [];
    internal readonly List<Assembly> _exceptAssemblyConventions = [];
    internal IConventionFactory? _conventionProviderFactory;
    internal ServiceProviderFactoryAdapter? _serviceProviderFactory;
    internal bool _useAttributeConventions = true;

    /// <summary>
    ///     Create a context builder with a set of properties
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="categories"></param>
    public ConventionContextBuilder(PropertiesType? properties, IEnumerable<ConventionCategory> categories)
    {
        Properties = new ServiceProviderDictionary(properties ?? new PropertiesDictionary());

        foreach (var variable in hostTypeEnvironmentVariables)
        {
            if (Environment.GetEnvironmentVariable(variable) is { Length: > 0 } hostType && Enum.TryParse<HostType>(hostType, true, out var type))
            {
                Properties[typeof(HostType)] = type;
            }
        }

        List<ConventionCategory> categoriesBuilder = [.. categories];
        foreach (var variable in categoryEnvironmentVariables)
        {
            if (Environment.GetEnvironmentVariable(variable) is not { Length: > 0 } category)
            {
                continue;
            }

            foreach (var item in category.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                categoriesBuilder.Add(new(item));
            }
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
    ///     Defines a callback that provides
    /// </summary>
    /// <param name="conventionFactory"></param>
    /// <returns></returns>
    public ConventionContextBuilder UseConventionFactory(IConventionFactory conventionFactory)
    {
        _conventionProviderFactory = conventionFactory;
        return this;
    }

    /// <summary>
    ///     Provide a diagnostic logger
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    public ConventionContextBuilder UseDiagnosticLogger(ILogger logger)
    {
        Properties[typeof(ILogger)] = logger;
        return this;
    }

    /// <summary>
    ///     Uses the diagnostic logging.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public ConventionContextBuilder UseDiagnosticLogging(Action<ILoggingBuilder> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _ = UseDiagnosticLogger(
            new ServiceCollection()
               .AddLogging(action)
               .BuildServiceProvider()
               .GetRequiredService<ILoggerFactory>()
               .CreateLogger("DiagnosticLogger")
        );

        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    [OverloadResolutionPriority(-1)]
    public ConventionContextBuilder AppendConvention(params IConvention[] conventions)
    {
        _conventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    public ConventionContextBuilder AppendConvention(params IEnumerable<IConvention> conventions)
    {
        _conventions.AddRange(conventions);
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
        _conventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(params IEnumerable<Type> conventions)
    {
        _conventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention<T>()
        where T : IConvention
    {
        _conventions.Add(typeof(T));
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
        _conventions.InsertRange(0, conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(params IEnumerable<IConvention> conventions)
    {
        _conventions.InsertRange(0, conventions);
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
        _conventions.InsertRange(0, conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(params IEnumerable<Type> conventions)
    {
        _conventions.InsertRange(0, conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention<T>()
        where T : IConvention
    {
        _conventions.Insert(0, typeof(T));
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
        _conventions.Add(new ConventionOrDelegate(@delegate, priority ?? 0, category));
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
        _conventions.Insert(0, new ConventionOrDelegate(@delegate, priority ?? 0, category));
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
        _exceptAssemblyConventions.AddRange(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(params IEnumerable<Assembly> assemblies)
    {
        _exceptAssemblyConventions.AddRange(assemblies);
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
        _exceptConventions.AddRange(types);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(params IEnumerable<Type> types)
    {
        _exceptConventions.AddRange(types);
        return this;
    }
}
