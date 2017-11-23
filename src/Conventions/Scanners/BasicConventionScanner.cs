using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A basic convention scanner that doesn't scan any assemblies it only composes provided conventions.
    /// </summary>
    public class BasicConventionScanner : IConventionScanner
    {
        private readonly List<IConvention> _contributions = new List<IConvention>();
        private readonly List<Type> _exceptContributions = new List<Type>();
        private readonly List<Delegate> _delegates = new List<Delegate>();
        private IConventionProvider _provider;

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="conventions">The initial list of conventions</param>
        public BasicConventionScanner(params IConvention[] conventions)
        {
            _contributions.AddRange(conventions);
        }

        /// <inheritdoc />
        public void ExceptConvention(Type type)
        {
            _provider = null;
            _exceptContributions.Add(type);
        }

        /// <inheritdoc />
        public IConventionProvider BuildProvider()
        {
            if (_provider is null)
            {
                return _provider = new ConventionProvider(_contributions, _exceptContributions, _delegates);
            }
            return _provider;
        }

        /// <inheritdoc />
        public void AddDelegate(Delegate @delegate)
        {
            _provider = null;
            _delegates.Add(@delegate);
        }

        /// <inheritdoc />
        public void AddConvention(IConvention convention)
        {
            _provider = null;
            _contributions.Add(convention);
        }
    }
}
