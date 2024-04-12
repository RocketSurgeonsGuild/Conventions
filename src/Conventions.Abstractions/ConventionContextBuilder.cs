#if NET8_0_OR_GREATER
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;
using PropertiesDictionary = System.Collections.Generic.Dictionary<object, object>;
#else
using PropertiesType = System.Collections.Generic.IDictionary<object, object?>;
using PropertiesDictionary = System.Collections.Generic.Dictionary<object, object?>;
#endif
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    internal readonly List<object> _prependedConventions = new();
    internal readonly List<object> _appendedConventions = new();
    internal readonly List<Type> _exceptConventions = new();
    internal readonly List<Assembly> _exceptAssemblyConventions = new();
    internal readonly List<object> _includeConventions = new();
    internal readonly List<Assembly> _includeAssemblyConventions = new();
    internal ConventionProviderFactory? _conventionProviderFactory;
    internal ServiceProviderFactoryAdapter? _serviceProviderFactory;
    internal bool _useAttributeConventions = true;
    internal object? _source;
    internal AssemblyProviderFactory? _assemblyProviderFactory;

    /// <summary>
    ///     Create a context builder with a set of properties
    /// </summary>
    /// <param name="properties"></param>
    public ConventionContextBuilder(PropertiesType? properties)
    {
        Properties = new ServiceProviderDictionary(properties ?? new PropertiesDictionary());
    }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    public IServiceProviderDictionary Properties { get; }

    /// <summary>
    ///     Use the given app domain for resolving assemblies
    /// </summary>
    /// <param name="appDomain"></param>
    /// <returns></returns>
    public ConventionContextBuilder UseAppDomain(AppDomain appDomain)
    {
        _source = appDomain;
        _conventionProviderFactory = null;
        return this;
    }

    /// <summary>
    ///     Use the given set of assemblies
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public ConventionContextBuilder UseAssemblies(IEnumerable<Assembly> assemblies)
    {
        _source = assemblies;
        _conventionProviderFactory = null;
        return this;
    }

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
    /// <param name="conventionProvider"></param>
    /// <returns></returns>
    public ConventionContextBuilder WithConventionsFrom(ConventionProviderFactory conventionProvider)
    {
        _conventionProviderFactory = conventionProvider;
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
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

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
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns>IConventionScanner.</returns>
    public ConventionContextBuilder AppendConvention(IEnumerable<IConvention> conventions)
    {
        _appendedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(IEnumerable<Type> conventions)
    {
        _appendedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <param name="conventions">The additional conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(IConvention convention, params IConvention[] conventions)
    {
        _appendedConventions.Add(convention);
        _appendedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <param name="conventions">The additional conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention(Type convention, params Type[] conventions)
    {
        _appendedConventions.Add(convention);
        _appendedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendConvention<T>()
        where T : IConvention
    {
        _appendedConventions.Add(typeof(T));
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(IEnumerable<IConvention> conventions)
    {
        _prependedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(IEnumerable<Type> conventions)
    {
        _prependedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <param name="conventions">The additional conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(IConvention convention, params IConvention[] conventions)
    {
        _prependedConventions.Add(convention);
        _prependedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <param name="convention">The first convention</param>
    /// <param name="conventions">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention(Type convention, params Type[] conventions)
    {
        _prependedConventions.Add(convention);
        _prependedConventions.AddRange(conventions);
        return this;
    }

    /// <summary>
    ///     Adds a set of conventions to the scanner
    /// </summary>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependConvention<T>()
        where T : IConvention
    {
        _prependedConventions.Add(typeof(T));
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegates">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendDelegate(IEnumerable<Delegate> delegates)
    {
        _appendedConventions.AddRange(delegates);
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <param name="delegates">The additional delegates.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder AppendDelegate(Delegate @delegate, params Delegate[] delegates)
    {
        _appendedConventions.Add(@delegate);
        _appendedConventions.AddRange(delegates);
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegates">The conventions.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependDelegate(IEnumerable<Delegate> delegates)
    {
        _prependedConventions.AddRange(delegates);
        return this;
    }

    /// <summary>
    ///     Adds a set of delegates to the scanner
    /// </summary>
    /// <param name="delegate">The initial delegate</param>
    /// <param name="delegates">The additional delegates.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder PrependDelegate(Delegate @delegate, params Delegate[] delegates)
    {
        _prependedConventions.Add(@delegate);
        _prependedConventions.AddRange(delegates);
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
    /// <param name="types">The convention types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder IncludeConvention(IEnumerable<Type> types)
    {
        _includeConventions.AddRange(types);
        return this;
    }

    /// <summary>
    ///     Adds an exception to the scanner to exclude a specific convention
    /// </summary>
    /// <param name="type">The first type to exclude</param>
    /// <param name="types">The additional types to exclude.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public ConventionContextBuilder IncludeConvention(Type type, params Type[] types)
    {
        _includeConventions.Add(type);
        _includeConventions.AddRange(types);
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