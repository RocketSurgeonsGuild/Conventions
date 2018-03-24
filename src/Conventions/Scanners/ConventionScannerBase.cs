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
        public void PrependDelegate(Delegate @delegate)
        {
            _provider = null;
            PrependedConventions.Add(@delegate);
        }

        /// <inheritdoc />
        public void PrependConvention(IConvention convention)
        {
            _provider = null;
            PrependedConventions.Add(convention);
        }

        /// <inheritdoc />
        public void AppendDelegate(Delegate @delegate)
        {
            _provider = null;
            AppendedConventions.Add(@delegate);
        }

        /// <inheritdoc />
        public void AppendConvention(IConvention convention)
        {
            _provider = null;
            AppendedConventions.Add(convention);
        }

        /// <inheritdoc />
        public void ExceptConvention(Type type)
        {
            _provider = null;
            ExceptConventions.Add(type);
        }

        /// <summary>
        /// Excludes an assembly from the convention
        /// </summary>
        /// <param name="assembly"></param>
        public void ExceptConvention(Assembly assembly)
        {
            _provider = null;
            ExceptAssemblyConventions.Add(assembly);
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
    }
}
