using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public IConventionScanner AppendConvention(params IConvention[] conventions)
        {
            _provider = null;
            _appendContributions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            _appendContributions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependConvention(params IConvention[] conventions)
        {
            _provider = null;
            _prependContributions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            _prependContributions.AddRange(conventions);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependDelegate(params Delegate[] delegates)
        {
            _provider = null;
            _prependContributions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner PrependDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            _prependContributions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner AppendDelegate(params Delegate[] delegates)
        {
            _provider = null;
            _appendContributions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner AppendDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            _appendContributions.AddRange(delegates);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Type[] types)
        {
            _provider = null;
            _exceptContributions.AddRange(types);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Type> types)
        {
            _provider = null;
            _exceptContributions.AddRange(types);
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Assembly[] assemblies)
        {
            return this;
        }

        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Assembly> assemblies)
        {
            return this;
        }
    }
}
