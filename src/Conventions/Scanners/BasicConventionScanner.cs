using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A basic convention scanner that doesn't scan any assemblies it only composes provided conventions.
    /// Implements the <see cref="IConventionScanner" />
    /// </summary>
    /// <seealso cref="IConventionScanner" />
    public class BasicConventionScanner : IConventionScanner
    {
        private readonly List<object> _prependContributions;
        private readonly List<object> _appendContributions;
        private readonly List<Type> _exceptContributions;
        private readonly IServiceProvider _serviceProvider;
        private IConventionProvider? _provider;

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="serviceProvider">The service provider (generally a ServiceProviderDictionary)</param>
        /// <param name="conventions">The initial list of conventions</param>
        public BasicConventionScanner(IServiceProvider serviceProvider, params IConvention[] conventions)
        {
            _serviceProvider = serviceProvider;
            _appendContributions = new List<object>();
            _prependContributions = new List<object>();
            _exceptContributions = new List<Type>();
            _prependContributions.AddRange(conventions);
        }

        internal BasicConventionScanner(
            IServiceProvider serviceProvider,
            List<object> prependedConventions,
            List<object> appendedConventions,
            List<Type> exceptConventions
        )
        {
            _serviceProvider = serviceProvider;
            _appendContributions = appendedConventions;
            _prependContributions = prependedConventions;
            _exceptContributions = exceptConventions;
        }

        internal BasicConventionScanner(BasicConventionScanner source)
        {
            _serviceProvider = source._serviceProvider;
            _appendContributions = source._appendContributions;
            _prependContributions = source._prependContributions;
            _exceptContributions = source._exceptContributions;
        }

        /// <summary>
        /// Creates a provider that returns a set of conventions.
        /// </summary>
        /// <returns>IConventionProvider.</returns>
        /// <inheritdoc />
        public IConventionProvider BuildProvider()
        {
            if (_provider is null)
            {
                return _provider = new ConventionProvider(
                    Enumerable.Empty<IConvention>(),
                    _prependContributions.Where(z => _exceptContributions.All(x => x != z.GetType())).ToList(),
                    _appendContributions.Where(z => _exceptContributions.All(x => x != z.GetType())).ToList()
                );
            }

            return _provider;
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
            _appendContributions.AddRange(conventions);
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
            _appendContributions.AddRange(
                conventions.Select(t => ActivatorUtilities.CreateInstance(_serviceProvider, t)).Cast<IConvention>()
            );
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
            _appendContributions.AddRange(conventions);
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
            _appendContributions.AddRange(
                conventions.Select(t => ActivatorUtilities.CreateInstance(_serviceProvider, t)).Cast<IConvention>()
            );
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention<T>()
            where T : IConvention
        {
            _provider = null;
            _appendContributions.Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider));
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
            _prependContributions.AddRange(conventions);
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
            _prependContributions.AddRange(
                conventions.Select(t => ActivatorUtilities.CreateInstance(_serviceProvider, t)).Cast<IConvention>()
            );
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
            _prependContributions.AddRange(conventions);
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
            _prependContributions.AddRange(
                conventions.Select(t => ActivatorUtilities.CreateInstance(_serviceProvider, t)).Cast<IConvention>()
            );
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention<T>()
            where T : IConvention
        {
            _provider = null;
            _prependContributions.Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider));
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
            _appendContributions.AddRange(delegates);
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
            _appendContributions.AddRange(delegates);
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
            _prependContributions.AddRange(delegates);
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
            _prependContributions.AddRange(delegates);
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
            _exceptContributions.AddRange(types);
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
            _exceptContributions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The convention types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Assembly> assemblies) => this;

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The additional types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Assembly[] assemblies) => this;
    }
}