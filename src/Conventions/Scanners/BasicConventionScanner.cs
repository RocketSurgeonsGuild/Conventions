using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A basic convention scanner that doesn't scan any assemblies it only composes provided conventions.
    /// </summary>
    public class BasicConventionScanner : IConventionScanner
    {
        private readonly List<object> _prependContributions = new List<object>();
        private readonly List<object> _appendContributions = new List<object>();
        private readonly List<Type> _exceptContributions = new List<Type>();
        private IConventionProvider _provider;

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="conventions">The initial list of conventions</param>
        public BasicConventionScanner(params IConvention[] conventions)
        {
            _prependContributions.AddRange(conventions);
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
                return _provider = new ConventionProvider(
                    Enumerable.Empty<IConvention>(),
                    Enumerable.Empty<Type>(), 
                    _prependContributions.Where(z => _exceptContributions.All(x => x != z.GetType())), 
                    _appendContributions.Where(z => _exceptContributions.All(x => x != z.GetType()))
                );
            }
            return _provider;
        }

        /// <inheritdoc />
        public void PrependDelegate(Delegate @delegate)
        {
            _provider = null;
            _prependContributions.Add(@delegate);
        }

        /// <inheritdoc />
        public void PrependConvention(IConvention convention)
        {
            _provider = null;
            _prependContributions.Add(convention);
        }

        /// <inheritdoc />
        public void AppendDelegate(Delegate @delegate)
        {
            _provider = null;
            _appendContributions.Add(@delegate);
        }

        /// <inheritdoc />
        public void AppendConvention(IConvention convention)
        {
            _provider = null;
            _appendContributions.Add(convention);
        }
    }
}
