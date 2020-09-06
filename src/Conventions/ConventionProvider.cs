using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ConventionProvider.
    /// Implements the <see cref="IConventionProvider" />
    /// </summary>
    /// <seealso cref="IConventionProvider" />
    internal class ConventionProvider : IConventionProvider
    {
        private readonly HostType _hostType;

        private static IEnumerable<T> TopographicalSort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
            {
                Visit(item, visited, sorted, dependencies);
            }

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                {
                    Visit(dep, visited, sorted, dependencies);
                }

                sorted.Add(item);
            }
            else
            {
                if (!sorted.Contains(item))
                {
                    throw new NotSupportedException($"Cyclic dependency found {item}");
                }
            }
        }

        private readonly Lazy<DelegateOrConvention[]> _conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionProvider" /> class.
        /// </summary>
        /// <param name="hostType"></param>
        /// <param name="contributions">The contributions.</param>
        /// <param name="prependedContributionsOrDelegates">The prepended contributions or delegates.</param>
        /// <param name="appendedContributionsOrDelegates">The appended contributions or delegates.</param>
        public ConventionProvider(
            HostType hostType,
            IEnumerable<IConvention> contributions,
            IEnumerable<object> prependedContributionsOrDelegates,
            IEnumerable<object> appendedContributionsOrDelegates
        )
        {
            _hostType = hostType;
            _conventions = new Lazy<DelegateOrConvention[]>(
                () =>
                {
                    var prepended = prependedContributionsOrDelegates as object[] ??
                        prependedContributionsOrDelegates.ToArray();
                    var appended = appendedContributionsOrDelegates as object[] ??
                        appendedContributionsOrDelegates.ToArray();
                    var contributionsList = contributions as IConvention[] ?? contributions.ToArray();

                    var c = prepended.Union(contributionsList).Union(appended).Select(selector).ToArray();

                    if (c.Where(x => x.Convention != null).Any(
                        cc => ( cc.Convention?.GetType().GetCustomAttributes() ?? Array.Empty<Attribute>() ).OfType<IConventionDependency>().Any()
                    ))
                    {
                        var conventions = c
                           .Where(x => x.Convention != null)
                           .Select(
                                convention =>
                                {
                                    var type = convention.Convention?.GetType();
                                    var dependencies =
                                        type?.GetCustomAttributes().OfType<IConventionDependency>().ToArray() ??
                                        Array.Empty<IConventionDependency>();
                                    return (
                                        convention,
                                        type,
                                        dependsOn: dependencies.Where(x => x.Direction == DependencyDirection.DependsOn)
                                           .Select(z => z.Type),
                                        dependentFor: dependencies
                                           .Where(x => x.Direction == DependencyDirection.DependentOf)
                                           .Select(z => z.Type)
                                    );
                                }
                            )
                           .ToArray();

                        var lookup = conventions.ToLookup(z => z.type, z => z.convention);
                        var dependentFor = conventions
                           .SelectMany(
                                data => data.dependentFor
                                   .SelectMany(z => lookup[z])
                                   .Select(innerDependentFor => ( dependentFor: innerDependentFor, data.convention ))
                            )
                           .ToLookup(z => z.dependentFor, z => z.convention);

                        var dependsOn = conventions
                           .SelectMany(
                                data => data.dependsOn
                                   .SelectMany(z => lookup[z])
                                   .Select(innerDependsOn => ( data.convention, dependsOn: innerDependsOn ))
                            )
                           .Concat(
                                conventions
                                   .SelectMany(
                                        data =>
                                            dependentFor[data.convention]
                                               .Select(innerDependsOn => ( data.convention, dependsOn: innerDependsOn ))
                                    )
                            )
                           .ToLookup(x => x.convention, x => x.dependsOn);

                        return TopographicalSort(
                                prepended.Union(contributionsList).Union(appended).Select(selector).Where(x => x != DelegateOrConvention.None),
                                x => dependsOn[x]
                            )
                           .ToArray();
                    }

                    return c;
                }
            );

            DelegateOrConvention selector(object value) => value switch
            {
                IConvention a => new DelegateOrConvention(a, a.GetType().GetCustomAttributes().OfType<IHostBasedConvention>().FirstOrDefault()?.HostType ?? HostType.Undefined),
                Delegate d    => new DelegateOrConvention(d),
                var _         => DelegateOrConvention.None
            };
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TContribution">The type of the contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="hostType">The host type.</param>
        public IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>(HostType hostType = HostType.Undefined)
            where TContribution : IConvention
            where TDelegate : Delegate => _conventions.Value
           .Select(
                x =>
                {
                    if (x.Convention is TContribution && (x.HostType == HostType.Undefined || x.HostType == hostType || x.HostType == _hostType))
                    {
                        return x;
                    }

                    // ReSharper disable once ConvertIfStatementToReturnStatement
                    if (x.Delegate is TDelegate)
                    {
                        return x;
                    }

                    return DelegateOrConvention.None;
                }
            )
           .Where(x => x != DelegateOrConvention.None);

        /// <summary>
        /// Gets a all the conventions from the provider
        /// </summary>
        /// <param name="hostType">The host type.</param>
        public IEnumerable<DelegateOrConvention> GetAll(HostType hostType = HostType.Undefined) => _conventions.Value
           .Where(x => hostType == HostType.Undefined || x.HostType == HostType.Undefined || x.HostType == hostType || x.HostType == _hostType);
    }
}