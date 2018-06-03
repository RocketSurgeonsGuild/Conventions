using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    internal class ConventionProvider : IConventionProvider
    {
        private readonly IEnumerable<object> _prependedConventionsOrDelegates;
        private readonly IEnumerable<object> _appendedConventionsOrDelegates;
        private readonly IEnumerable<IConvention> _conventions;

        public ConventionProvider(IEnumerable<IConvention> contributions, IEnumerable<Type> exceptContributions, IEnumerable<object> prependedContributionsOrDelegates, IEnumerable<object> appendedContributionsOrDelegates)
        {
            _prependedConventionsOrDelegates = prependedContributionsOrDelegates.ToArray();
            _appendedConventionsOrDelegates = appendedContributionsOrDelegates.ToArray();
            _conventions = contributions.Where(z => exceptContributions.All(x => x != z.GetType())).ToArray();
        }

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
                    if (x is Delegate d)
                    {
                        return new DelegateOrConvention(d);
                    }
                    return DelegateOrConvention.None;
                })
                .Where(x => x != DelegateOrConvention.None);
        }

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
