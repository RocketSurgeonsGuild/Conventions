using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Class BasicConventionScanner.
    /// </summary>
    /// <seealso cref="IConventionScanner" />
    /// TODO Edit XML Comment Template for BasicConventionScanner
    public class BasicConventionScanner : IConventionScanner
    {
        private readonly List<IConvention> _contributions = new List<IConvention>();
        private readonly List<Type> _exceptContributions = new List<Type>();
        private readonly List<Delegate> _delegates = new List<Delegate>();
        private IConventionProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConventionScanner"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// TODO Edit XML Comment Template for #ctor
        public BasicConventionScanner(params IConvention[] conventions)
        {
            _contributions.AddRange(conventions);
        }

        /// <summary>
        /// Excepts the convention.
        /// </summary>
        /// <param name="conventionType">Type of the convention.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// TODO Edit XML Comment Template for ExceptConvention
        public void ExceptConvention(Type conventionType)
        {
            _provider = null;
            _exceptContributions.Add(conventionType);
        }

        /// <summary>
        /// Gets the contributors.
        /// </summary>
        /// <returns>IEnumerable&lt;IServiceConvention&gt;.</returns>
        /// TODO Edit XML Comment Template for Get
        public IConventionProvider BuildProvider()
        {
            if (_provider is null)
            {
                return _provider = new ConventionProvider(_contributions, _exceptContributions, _delegates);
            }
            return _provider;
        }

        /// <summary>
        /// Add delegate, also removes the provider if it has been built
        /// </summary>
        /// <param name="delegate"></param>
        public void AddDelegate(Delegate @delegate)
        {
            _provider = null;
            _delegates.Add(@delegate);
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
            _contributions.Add(convention);
        }
    }
}
