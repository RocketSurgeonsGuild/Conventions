using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// ConventionProvider.
    /// Implements the <see cref="IConventionProvider" />
    /// </summary>
    /// <seealso cref="IConventionProvider" />
    internal class ConventionProvider : IConventionProvider
    {
        private readonly IEnumerable<DelegateOrConvention> _conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionProvider" /> class.
        /// </summary>
        /// <param name="contributions">The contributions.</param>
        /// <param name="prependedContributionsOrDelegates">The prepended contributions or delegates.</param>
        /// <param name="appendedContributionsOrDelegates">The appended contributions or delegates.</param>
        public ConventionProvider(IEnumerable<IConvention> contributions, IEnumerable<object> prependedContributionsOrDelegates, IEnumerable<object> appendedContributionsOrDelegates)
        {
            _conventions = prependedContributionsOrDelegates
                .Union(contributions)
                .Union(appendedContributionsOrDelegates)
                .Select(x =>
                {
                    switch (x)
                    {
                        case IConvention a: return new DelegateOrConvention(a);
                        case Delegate d: return new DelegateOrConvention(d);
                        default: return DelegateOrConvention.None;
                    }
                })
                .Where(x => x != DelegateOrConvention.None)
                .ToArray();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TContribution">The type of the contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <returns>IEnumerable{DelegateOrConvention}.</returns>
        public IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>()
            where TContribution : IConvention
            where TDelegate : Delegate
        {
            return _conventions
                .Select(x =>
                {
                    if (x.Convention is TContribution a)
                    {
                        return new DelegateOrConvention(a);
                    }
                    // ReSharper disable once ConvertIfStatementToReturnStatement
                    if (x.Delegate is TDelegate d)
                    {
                        return new DelegateOrConvention(d);
                    }
                    return DelegateOrConvention.None;
                })
                .Where(x => x != DelegateOrConvention.None);
        }

        /// <summary>
        /// Gets a all the conventions from the provider
        /// </summary>
        /// <returns>IEnumerable{DelegateOrConvention}.</returns>
        public IEnumerable<DelegateOrConvention> GetAll()
        {
            return _conventions;
        }
    }
}
