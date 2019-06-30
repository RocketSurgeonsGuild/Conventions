using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// ConventionProvider.
    /// Implements the <see cref="Rocket.Surgery.Conventions.Scanners.IConventionProvider" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.Scanners.IConventionProvider" />
    internal class ConventionProvider : IConventionProvider
    {
        private readonly List<object> _prependedConventionsOrDelegates;
        private readonly List<object> _appendedConventionsOrDelegates;
        private readonly IEnumerable<IConvention> _conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionProvider" /> class.
        /// </summary>
        /// <param name="contributions">The contributions.</param>
        /// <param name="exceptContributions">The except contributions.</param>
        /// <param name="prependedContributionsOrDelegates">The prepended contributions or delegates.</param>
        /// <param name="appendedContributionsOrDelegates">The appended contributions or delegates.</param>
        public ConventionProvider(IEnumerable<IConvention> contributions, List<Type> exceptContributions, List<object> prependedContributionsOrDelegates, List<object> appendedContributionsOrDelegates)
        {
            _prependedConventionsOrDelegates = prependedContributionsOrDelegates;
            _appendedConventionsOrDelegates = appendedContributionsOrDelegates;
            _conventions = contributions.Where(z => exceptContributions.All(x => x != z.GetType())).ToArray();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TContribution">The type of the contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <returns>IEnumerable&lt;DelegateOrConvention&gt;.</returns>
        public IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>()
            where TContribution : IConvention
            where TDelegate : Delegate
        {
            return _prependedConventionsOrDelegates
                .Union(_conventions)
                .Union(_appendedConventionsOrDelegates)
                .Select(x =>
                {
                    if (x is TContribution a)
                    {
                        return new DelegateOrConvention(a);
                    }
                    // ReSharper disable once ConvertIfStatementToReturnStatement
                    if (x is TDelegate d)
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
        /// <returns>IEnumerable&lt;DelegateOrConvention&gt;.</returns>
        public IEnumerable<DelegateOrConvention> GetAll()
        {
            return _prependedConventionsOrDelegates
                .Union(_conventions)
                .Union(_appendedConventionsOrDelegates)
                .Select(x =>
                {
                    switch (x)
                    {
                        case IConvention a: return new DelegateOrConvention(a);
                        case Delegate d: return new DelegateOrConvention(d);
                        default: return DelegateOrConvention.None;
                    }
                })
                .Where(x => x != DelegateOrConvention.None);
        }
    }
}
