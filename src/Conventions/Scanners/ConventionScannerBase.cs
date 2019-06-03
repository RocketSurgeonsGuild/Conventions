using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Base class for various scanners
    /// </summary>
    public abstract class ConventionScannerBase : IConventionScanner
    {
        /// <summary>
        /// Conventions to the included explictly.
        /// </summary>
        protected readonly List<object> PrependedConventions = new List<object>();

        /// <summary>
        /// Conventions to the included explictly.
        /// </summary>
        protected readonly List<object> AppendedConventions = new List<object>();

        /// <summary>
        /// Conventions to be excluded
        /// </summary>
        protected readonly List<Type> ExceptConventions = new List<Type>();

        /// <summary>
        /// The assemblys to exclude conventions
        /// </summary>
        protected readonly List<Assembly> ExceptAssemblyConventions = new List<Assembly>();

        private static readonly ConcurrentDictionary<Assembly, List<Type>> Conventions = new ConcurrentDictionary<Assembly, List<Type>>();
        private IConventionProvider _provider;
        private readonly IAssemblyCandidateFinder _assemblyCandidateFinder;

        /// <summary>
        /// Default constructor for the scanner
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder</param>
        protected ConventionScannerBase(IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            _assemblyCandidateFinder = assemblyCandidateFinder;
        }

        /// <summary>
        /// Gets all the assemblies by convention.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<Type> GetAssemblyConventions()
        {
            var assemblies = _assemblyCandidateFinder.GetCandidateAssemblies(
                "Rocket.Surgery.Conventions.Abstractions",
                "Rocket.Surgery.Conventions");
            foreach (var assembly in assemblies.Except(ExceptAssemblyConventions))
            {
                if (!Conventions.TryGetValue(assembly, out var types))
                {
                    types = assembly.GetCustomAttributes<ConventionAttribute>()
                        .Select(x => x.Type)
                        .ToList();
                    Conventions.TryAdd(assembly, types);
                }

                foreach (var item in types)
                {
                    yield return item;
                }
            }
        }

        /// <inheritdoc />
        public IConventionProvider BuildProvider()
        {
            return _provider ?? (_provider = CreateProvider());
        }

        /// <summary>
        /// Method used to create a convention provider
        /// </summary>
        /// <returns></returns>
        protected virtual IConventionProvider CreateProvider()
        {
            var contributionTypes = GetAssemblyConventions()
                .Except(PrependedConventions.Select(x => x.GetType()))
                .Except(AppendedConventions.Select(x => x.GetType()))
                .Select(Activator.CreateInstance)
                .Cast<IConvention>();

            return new ConventionProvider(contributionTypes, ExceptConventions, PrependedConventions, AppendedConventions);
        }

        /// <inheritdoc />
        public IConventionScanner AppendConvention(params IConvention[] conventions)
        {
            _provider = null;
            AppendedConventions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            AppendedConventions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependConvention(params IConvention[] conventions)
        {
            _provider = null;
            PrependedConventions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            PrependedConventions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependDelegate(params Delegate[] delegates)
        {
            _provider = null;
            PrependedConventions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            PrependedConventions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner AppendDelegate(params Delegate[] delegates)
        {
            _provider = null;
            AppendedConventions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner AppendDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            AppendedConventions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Type[] types)
        {
            _provider = null;
            ExceptConventions.AddRange(types);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Type> types)
        {
            _provider = null;
            ExceptConventions.AddRange(types);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Assembly[] assemblies)
        {
            _provider = null;
            ExceptAssemblyConventions.AddRange(assemblies);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Assembly> assemblies)
        {
            _provider = null;
            ExceptAssemblyConventions.AddRange(assemblies);
            return this;
        }
    }
}
