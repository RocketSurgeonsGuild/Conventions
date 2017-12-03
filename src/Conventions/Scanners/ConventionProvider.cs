using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    internal class ConventionProvider : IConventionProvider
    {
        private readonly IEnumerable<object> _conventionsOrDelegates;
        private readonly IEnumerable<IConvention> _conventions;

        public ConventionProvider(IEnumerable<IConvention> contributions, IEnumerable<Type> exceptContributions, IEnumerable<object> contributionsOrDelegates)
        {
            _conventionsOrDelegates = contributionsOrDelegates.ToArray();
            _conventions = contributions.Where(z => exceptContributions.All(x => x != z.GetType())).ToArray();
        }

        public IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>()
            where TContribution : IConvention
        {
            return _conventions
                .Union(_conventionsOrDelegates)
                .Select(x =>
                {
                    if (x is TContribution a)
                    {
                        return new DelegateOrConvention(a);
                    }
                    // ReSharper disable once ConvertIfStatementToReturnStatement
                    if (x.GetType() == typeof(TDelegate))
                    {
                        return new DelegateOrConvention((Delegate)x);
                    }
                    return DelegateOrConvention.None;
                })
                .Where(x => x != DelegateOrConvention.None);
        }

        public IEnumerable<DelegateOrConvention> GetAll()
        {
            return _conventions
                .Union(_conventionsOrDelegates)
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
