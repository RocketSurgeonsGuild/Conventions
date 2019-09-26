using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// ConventionProvider.
    /// Implements the <see cref="IConventionProvider" />
    /// </summary>
    /// <seealso cref="IConventionProvider" />
    internal class ConventionProvider : IConventionProvider
    {
        private readonly Lazy<DelegateOrConvention[]> _conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionProvider" /> class.
        /// </summary>
        /// <param name="contributions">The contributions.</param>
        /// <param name="prependedContributionsOrDelegates">The prepended contributions or delegates.</param>
        /// <param name="appendedContributionsOrDelegates">The appended contributions or delegates.</param>
        public ConventionProvider(IEnumerable<IConvention> contributions, IEnumerable<object> prependedContributionsOrDelegates, IEnumerable<object> appendedContributionsOrDelegates)
        {
            _conventions = new Lazy<DelegateOrConvention[]>(() =>
            {
                var prepended = prependedContributionsOrDelegates as object[] ??
                                prependedContributionsOrDelegates.ToArray();
                var appended = appendedContributionsOrDelegates as object[] ??
                               appendedContributionsOrDelegates.ToArray();
                var contributionsList = contributions as IConvention[] ?? contributions.ToArray();

                var c = prepended
                    .Union(contributionsList)
                    .Union(appended)
                    .Select(x =>
                    {
                        return x switch
                        {
                            IConvention a => new DelegateOrConvention(a, a.GetType().GetCustomAttributes().OfType<IIsHostBasedConvention>().FirstOrDefault()?.HostType),
                            Delegate d => new DelegateOrConvention(d),
                            _ => DelegateOrConvention.None,
                        };
                    })
                    .ToArray();

                var conventions = c
                    .Select(convention => (
                        convention,
                        type: convention.Convention?.GetType(),
                        dependsOn: convention.Convention?.GetType().GetCustomAttributes().OfType<IDependsOnConvention>().Select(z => z.Type) ?? Array.Empty<Type>(),
                        dependentFor: convention.Convention?.GetType().GetCustomAttributes().OfType<IDependentOfConvention>().Select(z => z.Type) ?? Array.Empty<Type>()
                    ))
                    .ToArray();

                if (conventions.Any(z => z.dependsOn.Any() || z.dependentFor.Any()))
                {
                    var lookup = conventions.ToLookup(z => z.type, z => z.convention);
                    var dependentFor = conventions
                        .SelectMany(data => data.dependentFor
                            .SelectMany(z => lookup[z])
                            .Select(dependentFor => (dependentFor, data.convention))
                        ).ToLookup(z => z.dependentFor, z => z.convention);

                    var dependsOn = conventions
                        .SelectMany(data => data.dependsOn
                            .SelectMany(z => lookup[z])
                            .Select(dependsOn => (data.convention, dependsOn)))
                        .Concat(conventions
                        .SelectMany(data =>
                            dependentFor[data.convention]
                                .Select(dependsOn => (data.convention, dependsOn))))
                        //.Concat(dependentFor[data.convention]))

                        .ToLookup(x => x.convention, x => x.dependsOn);

                    return TopographicalSort(prepended
                                .Union(contributionsList)
                                .Union(appended)
                                .Select(x =>
                                {
                                    return x switch
                                    {
                                        IConvention a => new DelegateOrConvention(a, a.GetType().GetCustomAttributes().OfType<IIsHostBasedConvention>().FirstOrDefault()?.HostType),
                                        Delegate d => new DelegateOrConvention(d),
                                        _ => DelegateOrConvention.None,
                                    };
                                })
                                .Where(x => x != DelegateOrConvention.None),
                            x => dependsOn[x]
                        )
                        .ToArray();
                }
                else
                {
                    return c;
                }
            });
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TContribution">The type of the contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        public IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>()
            where TContribution : IConvention
            where TDelegate : Delegate => Get<TContribution, TDelegate>(null);

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TContribution">The type of the contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="hostType">The host type.</param>
        public IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>(HostType? hostType)
            where TContribution : IConvention
            where TDelegate : Delegate => _conventions.Value
                .Select(x =>
                {
                    if (x.Convention is TContribution a)
                    {
                        if (!x.HostType.HasValue || x.HostType == hostType)
                        {
                            return x;
                        }
                    }
                    // ReSharper disable once ConvertIfStatementToReturnStatement
                    if (x.Delegate is TDelegate d)
                    {
                        return x;
                    }
                    return DelegateOrConvention.None;
                })
                .Where(x => x != DelegateOrConvention.None);

        /// <summary>
        /// Gets a all the conventions from the provider
        /// </summary>
        public IEnumerable<DelegateOrConvention> GetAll() => GetAll(null);

        /// <summary>
        /// Gets a all the conventions from the provider
        /// </summary>
        /// <param name="hostType">The host type.</param>
        public IEnumerable<DelegateOrConvention> GetAll(HostType? hostType) => _conventions.Value
            .Where(x => !x.HostType.HasValue || x.HostType == hostType);

        private static IEnumerable<T> TopographicalSort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
                Visit(item, visited, sorted, dependencies);

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                    Visit(dep, visited, sorted, dependencies);

                sorted.Add(item);
            }
            else
            {
                if (!sorted.Contains(item))
                    throw new NotSupportedException($"Cyclic dependency found {item}");
            }
        }
    }
}
