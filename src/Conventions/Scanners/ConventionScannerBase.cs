using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Base class for various scanners
    /// Implements the <see cref="Rocket.Surgery.Conventions.Scanners.IConventionScanner" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.Scanners.IConventionScanner" />
    public abstract class ConventionScannerBase : IConventionScanner
    {
        private readonly List<object> _prependedConventions = new List<object>();
        private readonly List<object> _appendedConventions = new List<object>();
        private readonly List<Type> _exceptConventions = new List<Type>();
        private readonly List<Assembly> _exceptAssemblyConventions = new List<Assembly>();

        private static readonly ConcurrentDictionary<Assembly, List<IConvention>> Conventions = new ConcurrentDictionary<Assembly, List<IConvention>>();
        private IConventionProvider _provider;
        private readonly IAssemblyCandidateFinder _assemblyCandidateFinder;
        private readonly IServiceProvider _serviceProvider;
        private bool _updatedConventionCollections = false;

        /// <summary>
        /// Default constructor for the scanner
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder</param>
        /// <param name="serviceProvider">The service provider for creating instances of conventions (usually a <see cref="IServiceProviderDictionary"/>.</param>
        protected ConventionScannerBase(IAssemblyCandidateFinder assemblyCandidateFinder, IServiceProvider serviceProvider)
        {
            _assemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(assemblyCandidateFinder));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets all the assemblies by convention.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IConvention> GetAssemblyConventions()
        {
            var assemblies = _assemblyCandidateFinder.GetCandidateAssemblies(
                "Rocket.Surgery.Conventions.Abstractions",
                "Rocket.Surgery.Conventions");

            var prependedConventionTypes = new Lazy<List<Type>>(() => _prependedConventions.Select(x => x.GetType()).Distinct().ToList());
            var appendedConventionTypes = new Lazy<List<Type>>(() => _appendedConventions.Select(x => x.GetType()).Distinct().ToList());

            foreach (var assembly in assemblies.Except(_exceptAssemblyConventions))
            {
                if (!Conventions.TryGetValue(assembly, out var types))
                {
                    types = assembly.GetCustomAttributes<ConventionAttribute>()
                        .Select(x => x.Type)
                        .Distinct()
                        .Except(prependedConventionTypes.Value)
                        .Except(appendedConventionTypes.Value)
                        .Select(type => ActivatorUtilities.CreateInstance(_serviceProvider, type))
                        .Cast<IConvention>()
                        .ToList();
                    Conventions.TryAdd(assembly, types);
                }

                foreach (var item in types)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Creates a provider that returns a set of conventions.
        /// </summary>
        /// <returns>IConventionProvider.</returns>
        /// <inheritdoc />
        public IConventionProvider  BuildProvider()
        {
            return _provider ??= CreateProvider();
        }

        /// <summary>
        /// Method used to create a convention provider
        /// </summary>
        /// <returns></returns>
        private IConventionProvider CreateProvider()
        {
            if (!_updatedConventionCollections)
            {
                for (var i = 0; i < _prependedConventions.Count; i++)
                    if (_prependedConventions[i] is Type type)
                        _prependedConventions[i] = ActivatorUtilities.CreateInstance(_serviceProvider, type);
                for (var i = 0; i < _appendedConventions.Count; i++)
                    if (_appendedConventions[i] is Type type)
                        _appendedConventions[i] = ActivatorUtilities.CreateInstance(_serviceProvider, type);
                _updatedConventionCollections = true;
            }

            var contributionTypes = GetAssemblyConventions()
                .Where(z => _exceptConventions.All(x => x != z.GetType()));

            return new ConventionProvider(contributionTypes, _prependedConventions, _appendedConventions);
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<Type> conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(params IConvention[] conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(params Type[] conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention<T>() where T : IConvention
        {
            _provider = null;
            _appendedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<Type> conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(params IConvention[] conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(params Type[] conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention<T>() where T : IConvention
        {
            _provider = null;
            _prependedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendDelegate(params Delegate[] delegates)
        {
            _provider = null;
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependDelegate(params Delegate[] delegates)
        {
            _provider = null;
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The convention types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Type> types)
        {
            _provider = null;
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The additional types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Type[] types)
        {
            _provider = null;
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The convention types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Assembly> assemblies)
        {
            _provider = null;
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The additional types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Assembly[] assemblies)
        {
            _provider = null;
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }
    }
}
