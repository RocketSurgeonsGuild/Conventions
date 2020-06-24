using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rocket.Surgery.Conventions
{
    class UninitializedConventionScanner : IConventionScanner
    {
        internal readonly List<object> _prependedConventions = new List<object>();
        internal  readonly List<object> _appendedConventions = new List<object>();
        internal  readonly List<Type> _exceptConventions = new List<Type>();
        internal  readonly List<Assembly> _exceptAssemblyConventions = new List<Assembly>();

        public IConventionProvider BuildProvider() => throw new NotImplementedException();
        
        
        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<IConvention> conventions)
        {
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
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention<T>()
            where T : IConvention
        {
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
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention<T>()
            where T : IConvention
        {
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
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }
    }
}