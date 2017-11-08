using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Class ConventionScannerBase.
    /// </summary>
    /// <seealso cref="IConventionScanner" />
    /// TODO Edit XML Comment Template for ConventionScannerBase
    public abstract class ConventionScannerBase : IConventionScanner
    {
        /// <summary>
        ///
        /// </summary>
        protected readonly List<object> IncludeContributions = new List<object>();

        /// <summary>
        ///
        /// </summary>
        protected readonly List<Type> ExceptContributions = new List<Type>();

        private static readonly ConcurrentDictionary<Assembly, List<Type>> Contributions = new ConcurrentDictionary<Assembly, List<Type>>();
        private IConventionProvider _provider;
        private readonly List<Assembly> _exceptAssemblyContributions = new List<Assembly>();
        private readonly IAssemblyCandidateFinder _assemblyCandidateFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionScannerBase"/> class.
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// TODO Edit XML Comment Template for #ctor
        protected ConventionScannerBase(IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            _assemblyCandidateFinder = assemblyCandidateFinder;
        }

        /// <summary>
        /// Gets the assembly conventions.
        /// </summary>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        /// TODO Edit XML Comment Template for GetAssemblyContributions
        protected IEnumerable<Type> GetAssemblyContributions()
        {
            var assemblies = _assemblyCandidateFinder.GetCandidateAssemblies(
                "ReadySelect.Abstractions",
                "ReadySelect.Core.Abstractions",
                "ReadySelect.AspNetCore.Abstractions");
            foreach (var assembly in assemblies.Except(_exceptAssemblyContributions))
            {
                if (!Contributions.TryGetValue(assembly, out var types))
                {
                    types = assembly.GetCustomAttributes<ContributionAttribute>()
                        .Select(x => x.Type)
                        .ToList();
                    Contributions.TryAdd(assembly, types);
                }

                foreach (var item in types)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Add delegate, also removes the provider if it has been built
        /// </summary>
        /// <param name="delegate"></param>

        public void AddDelegate(Delegate @delegate)
        {
            _provider = null;
            IncludeContributions.Add(@delegate);
        }

        /// <summary>
        /// Adds the convention.
        /// </summary>
        /// <param name="convention">The convention.</param>
        /// <returns>IEnumerable&lt;IServiceConvention&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// TODO Edit XML Comment Template for AddConvention
        public void AddConvention(IConvention convention)
        {
            _provider = null;
            IncludeContributions.Add(convention);
        }

        /// <summary>
        /// Excepts the convention.
        /// </summary>
        /// <param name="conventionType">Type of the convention.</param>
        /// TODO Edit XML Comment Template for ExceptConvention
        public void ExceptConvention(Type conventionType)
        {
            _provider = null;
            ExceptContributions.Add(conventionType);
        }

        /// <summary>
        /// Excepts the convention.
        /// </summary>
        /// <param name="assembly">Assembly containing the conventions to exclude.</param>
        /// TODO Edit XML Comment Template for ExceptConvention
        public void ExceptContribution(Assembly assembly)
        {
            _provider = null;
            _exceptAssemblyContributions.Add(assembly);
        }

        /// <summary>
        /// Builds the provider.
        /// </summary>
        /// <returns>IConventionProvider.</returns>
        /// TODO Edit XML Comment Template for BuildProvider
        public IConventionProvider BuildProvider()
        {
            return _provider ?? (_provider = CreateProvider());
        }

        /// <summary>
        /// Gets the conventions.
        /// </summary>
        /// <returns>IEnumerable&lt;IServiceConvention&gt;.</returns>
        /// TODO Edit XML Comment Template for Get
        protected virtual IConventionProvider CreateProvider()
        {
            var contributionTypes = GetAssemblyContributions()
                .Except(IncludeContributions.Select(x => x.GetType()))
                .Select(Activator.CreateInstance)
                .Cast<IConvention>();

            return new ConventionProvider(contributionTypes, ExceptContributions, IncludeContributions);
        }
    }
}
