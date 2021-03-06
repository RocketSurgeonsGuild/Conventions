using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Builder that can be used to create a context.
    /// </summary>
    public class ConventionContextBuilder
    {
        internal readonly List<object> _prependedConventions = new List<object>();
        internal readonly List<object> _appendedConventions = new List<object>();
        internal readonly List<Type> _exceptConventions = new List<Type>();
        internal readonly List<Assembly> _exceptAssemblyConventions = new List<Assembly>();
        internal Func<IServiceProvider, IEnumerable<IConventionWithDependencies>>? _conventionProvider;
        internal bool _useAttributeConventions = true;
        internal object? _source;
        internal AssemblyCandidateFinderFactory? _assemblyCandidateFinderFactory;
        internal AssemblyProviderFactory? _assemblyProviderFactory;

        /// <summary>
        /// Create a context builder with a set of properties
        /// </summary>
        /// <param name="properties"></param>
        public ConventionContextBuilder(IDictionary<object, object?>? properties)
        {
            Properties = new ServiceProviderDictionary(properties ?? new Dictionary<object, object?>());
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IServiceProviderDictionary Properties { get; }

        /// <summary>
        /// Use the given app domain for resolving assemblies
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public ConventionContextBuilder UseAppDomain([NotNull] AppDomain appDomain)
        {
            _source = appDomain;
            return this;
        }

        /// <summary>
        /// Use the given set of assemblies
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public ConventionContextBuilder UseAssemblies([NotNull] IEnumerable<Assembly> assemblies)
        {
            _source = assemblies;
            return this;
        }

        /// <summary>
        /// Enables convention attributes
        /// </summary>
        /// <returns></returns>
        public ConventionContextBuilder EnableConventionAttributes()
        {
            _useAttributeConventions = true;
            _conventionProvider = null;
            return this;
        }

        /// <summary>
        /// Defines a callback that provides
        /// </summary>
        /// <param name="conventionProvider"></param>
        /// <returns></returns>
        public ConventionContextBuilder WithConventionsFrom(Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> conventionProvider)
        {
            _conventionProvider = conventionProvider;
            return this;
        }

        /// <summary>
        /// Disables convention attributes
        /// </summary>
        /// <returns></returns>
        public ConventionContextBuilder DisableConventionAttributes()
        {
            _useAttributeConventions = false;
            _conventionProvider = null;
            return this;
        }

        /// <summary>
        /// Provide a diagnostic logger
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public ConventionContextBuilder UseDiagnosticLogger([NotNull] ILogger logger)
        {
            Properties[typeof(ILogger)] = logger;
            return this;
        }

        /// <summary>
        /// Uses the diagnostic logging.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public ConventionContextBuilder UseDiagnosticLogging([NotNull] Action<ILoggingBuilder> action)
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
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        public ConventionContextBuilder AppendConvention(IEnumerable<IConvention> conventions)
        {
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder AppendConvention(IEnumerable<Type> conventions)
        {
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="convention">The first convention</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder AppendConvention(IConvention convention, params IConvention[] conventions)
        {
            _appendedConventions.Add(convention);
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="convention">The first convention</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder AppendConvention(Type convention, params Type[] conventions)
        {
            _appendedConventions.Add(convention);
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder AppendConvention<T>()
            where T : IConvention
        {
            _appendedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder PrependConvention(IEnumerable<IConvention> conventions)
        {
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder PrependConvention(IEnumerable<Type> conventions)
        {
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="convention">The first convention</param>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder PrependConvention(IConvention convention, params IConvention[] conventions)
        {
            _prependedConventions.Add(convention);
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="convention">The first convention</param>
        /// <param name="conventions">The conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder PrependConvention(Type convention, params Type[] conventions)
        {
            _prependedConventions.Add(convention);
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder PrependConvention<T>()
            where T : IConvention
        {
            _prependedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder AppendDelegate(IEnumerable<Delegate> delegates)
        {
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegate">The initial delegate</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder AppendDelegate(Delegate @delegate, params Delegate[] delegates)
        {
            _appendedConventions.Add(@delegate);
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder PrependDelegate(IEnumerable<Delegate> delegates)
        {
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegate">The initial delegate</param>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder PrependDelegate(Delegate @delegate, params Delegate[] delegates)
        {
            _appendedConventions.Add(@delegate);
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific convention
        /// </summary>
        /// <param name="types">The convention types to exclude.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder ExceptConvention(IEnumerable<Type> types)
        {
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific convention
        /// </summary>
        /// <param name="type">The first type to exclude</param>
        /// <param name="types">The additional types to exclude.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder ExceptConvention(Type type, params Type[] types)
        {
            _exceptConventions.Add(type);
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific convention
        /// </summary>
        /// <param name="assemblies">The convention types to exclude.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder ExceptConvention(IEnumerable<Assembly> assemblies)
        {
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific convention
        /// </summary>
        /// <param name="assembly">The assembly to exclude</param>
        /// <param name="assemblies">The additional types to exclude.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public ConventionContextBuilder ExceptConvention(Assembly assembly, params Assembly[] assemblies)
        {
            _exceptAssemblyConventions.Add(assembly);
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }
    }
}