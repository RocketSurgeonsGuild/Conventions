using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Conventions.Scanners
{
    internal class ConventionProvider : IConventionProvider
    {
        private readonly IEnumerable<object> _contributionsOrDelegates;
        private readonly IEnumerable<IConvention> _contributions;

        public ConventionProvider(IEnumerable<IConvention> contributions, IEnumerable<Type> exceptContributions, IEnumerable<object> contributionsOrDelegates)
        {
            _contributionsOrDelegates = contributionsOrDelegates;
            _contributions = contributions.Where(z => exceptContributions.All(x => x != z.GetType()));
        }

        public IEnumerable<DelegateOrConvention<TContribution, TDelegate>> Get<TContribution, TDelegate>()
        {
            return _contributions
                .Union(_contributionsOrDelegates)
                .Select(x =>
                {
                    if (x is TContribution a)
                    {
                        return new DelegateOrConvention<TContribution, TDelegate>(a);
                    }
                    // ReSharper disable once ConvertIfStatementToReturnStatement
                    if (x.GetType() == typeof(TDelegate))
                    {
                        return new DelegateOrConvention<TContribution, TDelegate>((TDelegate)x);
                    }
                    return DelegateOrConvention<TContribution, TDelegate>.None;
                })
                .Where(x => x != DelegateOrConvention<TContribution, TDelegate>.None);
        }
    }
}
