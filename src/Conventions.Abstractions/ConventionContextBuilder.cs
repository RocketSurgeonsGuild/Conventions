using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;
using PropertiesDictionary = System.Collections.Generic.Dictionary<object, object>;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Builder that can be used to create a context.
/// </summary>
[PublicAPI]
public class ConventionContextBuilder
{
    /// <summary>
    ///     Create a default context builder
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static ConventionContextBuilder Create(PropertiesType? properties = null)
    {
        return new(properties ?? new PropertiesDictionary());
    }

    // this null is used a marker to indicate where in the list is the middle
    internal readonly List<object?> _conventions = [null!,];
    internal readonly List<Type> _exceptConventions = new();
    internal readonly List<Assembly> _exceptAssemblyConventions = new();
    internal readonly List<Assembly> _includeAssemblyConventions = new();
    internal IConventionFactory? _conventionProviderFactory;
    internal ServiceProviderFactoryAdapter? _serviceProviderFactory;
    internal bool _useAttributeConventions = true;

    /// <summary>
    ///     Create a context builder with a set of properties
    /// </summary>
    /// <param name="properties"></param>
    public ConventionContextBuilder(PropertiesType? properties)
    {
        Properties = new ServiceProviderDictionary(properties ?? new PropertiesDictionary());
        // Should we do configuration?
        if (Enum.TryParse<HostType>(Environment.GetEnvironmentVariable("ROCKETSURGERYCONVENTIONS__HOSTTYPE"), out var hostType))
            Properties[typeof(HostType)] = hostType;
    }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    public IServiceProviderDictionary Properties { get; }

    /// <summary>
    ///     Enables convention attributes
    /// </summary>
    /// <returns></returns>
    public ConventionContextBuilder EnableConventionAttributes()
    {
        _useAttributeConventions = true;
        _conventionProviderFactory = null;
        return this;
    }

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
    ///     Disables convention attributes
    /// </summary>
    /// <returns></returns>
    public ConventionContextBuilder DisableConventionAttributes()
    {
        _useAttributeConventions = false;
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
        UseDiagnosticLogger(
            new ServiceCollection()
               .AddLogging(action)
               .BuildServiceProvider()
               .GetRequiredService<ILoggerFactory>()
               .CreateLogger("DiagnosticLogger")
        );

        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <param name="priority">The priority of the delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AddDelegate(Delegate @delegate, int priority)
    {
        _conventions.Add(new ConventionOrDelegate(@delegate, priority));
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    public ConventionContextBuilder AppendConvention(IEnumerable<IConvention> conventions)
    {
        _conventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(IEnumerable<Type> conventions)
    {
        _conventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(IConvention convention)
    {
        _conventions.Add(convention);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(Type convention)
    {
        _conventions.Add(convention);
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
    public ConventionContextBuilder PrependConvention(IEnumerable<IConvention> conventions)
    {
        _conventions.InsertRange(0, conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(IEnumerable<Type> conventions)
    {
        _conventions.InsertRange(0, conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(IConvention convention)
    {
        _conventions.Insert(0, convention);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(Type convention)
    {
        _conventions.Insert(0, convention);
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
    /// <param name="delegates">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendDelegate(IEnumerable<Delegate> delegates)
    {
        _conventions.AddRange(delegates);
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendDelegate(Delegate @delegate)
    {
        _conventions.Add(@delegate);
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegates">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependDelegate(IEnumerable<Delegate> delegates)
    {
        _conventions.InsertRange(0, delegates);
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependDelegate(Delegate @delegate)
    {
        _conventions.Insert(0, @delegate);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="types">The convention types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(IEnumerable<Type> types)
    {
        _exceptConventions.AddRange(types);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="type">The first type to exclude</param>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(Type type, params Type[] types)
    {
        _exceptConventions.Add(type);
        _exceptConventions.AddRange(types);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The convention types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(IEnumerable<Assembly> assemblies)
    {
        _exceptAssemblyConventions.AddRange(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assembly">The assembly to exclude</param>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder ExceptConvention(Assembly assembly, params Assembly[] assemblies)
    {
        _exceptAssemblyConventions.Add(assembly);
        _exceptAssemblyConventions.AddRange(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assemblies">The convention types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder IncludeConvention(IEnumerable<Assembly> assemblies)
    {
        _includeAssemblyConventions.AddRange(assemblies);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="assembly">The assembly to exclude</param>
    /// <param name="assemblies">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder IncludeConvention(Assembly assembly, params Assembly[] assemblies)
    {
        _includeAssemblyConventions.Add(assembly);
        _includeAssemblyConventions.AddRange(assemblies);
        return this;
    }
}