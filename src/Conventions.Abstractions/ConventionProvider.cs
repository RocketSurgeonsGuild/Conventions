using System.Collections.Immutable;
using System.Reflection;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     ConventionProvider.
///     Implements the <see cref="IConventionProvider" />
/// </summary>
/// <seealso cref="IConventionProvider" />
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
internal class ConventionProvider : IConventionProvider
{
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
        if (visited.Add(item))
        {
            foreach (var dep in dependencies(item))
            {
                Visit(dep, visited, sorted, dependencies);
            }

            sorted.Add(item);
        }
        else
        {
            if (!sorted.Contains(item)) throw new NotSupportedException($"Cyclic dependency found {item}");
        }
    }

    private static ConventionOrDelegate FromConvention(object? value)
    {
        return value switch
               {
                   IConventionMetadata cwd => new(cwd),
                   IConvention convention  => FromConvention(convention),
                   Delegate d              => new(d, 0, default),
                   ConventionOrDelegate d  => d,
                   _                       => ConventionOrDelegate.None,
               };
    }

    private static ConventionOrDelegate FromConvention(IConvention convention)
    {
        var type = convention.GetType();
        var dependencies =
            type.GetCustomAttributes().OfType<IConventionDependency>().ToArray();
        var hostType = convention.GetType().GetCustomAttributes().OfType<IHostBasedConvention>().FirstOrDefault()?.HostType ?? HostType.Undefined;
        var category = convention.GetType().GetCustomAttribute<ConventionCategoryAttribute>()?.Category ?? ConventionCategory.Application;
        return new(convention, hostType, category, dependencies);
    }

    private static object? ToObject(ConventionOrDelegate delegateOrConvention)
    {
        return (object?)delegateOrConvention.Delegate ?? delegateOrConvention.Convention;
    }

    private readonly HostType _hostType;

    private readonly Lazy<ImmutableArray<ConventionOrDelegate>> _conventions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConventionProvider" /> class.
    /// </summary>
    /// <param name="hostType"></param>
    /// <param name="categories"></param>
    /// <param name="contributions">The contributions.</param>
    public ConventionProvider(HostType hostType, ImmutableHashSet<ConventionCategory> categories, List<object?> contributions) : this(
        hostType,
        categories,
        contributions.Where(z => z is { }).Select(FromConvention)
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConventionProvider" /> class.
    /// </summary>
    /// <param name="hostType"></param>
    /// <param name="categories"></param>
    /// <param name="contributions">The contributions.</param>
    private ConventionProvider(HostType hostType, ImmutableHashSet<ConventionCategory> categories, IEnumerable<ConventionOrDelegate> contributions)
    {
        _hostType = hostType;
        _conventions = new(
            () =>
            {
                var contributionsList = contributions as IReadOnlyCollection<ConventionOrDelegate> ?? contributions.ToArray();

                var c = contributionsList.AsEnumerable();
                if (categories.Any())
                {
                    c = c.Where(z => categories.Contains(z.Category));
                }

                if (!c.Any(z => z.Dependencies.Length > 0)) return [..c.OrderBy(z => z.Priority),];
                var conventions = c
                                 .Where(x => x.Convention != null)
                                 .Select(
                                      convention =>
                                      {
                                          return (
                                              convention,
                                              // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                                              type: convention.Convention!.GetType(),
                                              dependsOn: convention
                                                        .Dependencies.Where(x => x.Direction == DependencyDirection.DependsOn)
                                                        .Select(z => z.Type),
                                              dependentFor: convention
                                                           .Dependencies
                                                           .Where(x => x.Direction == DependencyDirection.DependentOf)
                                                           .Select(z => z.Type)
                                          );
                                      }
                                  )
                                 .ToArray();

                var lookup = conventions.ToLookup(z => z.type, z => z.convention);
                var dependentFor = conventions
                                  .SelectMany(
                                       data => data
                                              .dependentFor
                                              .SelectMany(z => lookup[z])
                                              .Select(innerDependentFor => ( dependentFor: innerDependentFor, data.convention ))
                                   )
                                  .ToLookup(z => z.dependentFor, z => z.convention);

                var dependsOn = conventions
                               .SelectMany(
                                    data => data
                                           .dependsOn
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
                               .ToLookup(x => x.convention.Convention, x => x.dependsOn);

                return [..TopographicalSort(c.OrderBy(z => z.Priority), x => dependsOn[x.Convention]),];
            }
        );
    }

    /// <summary>
    ///     Gets this instance.
    /// </summary>
    /// <typeparam name="TContribution">The type of the contribution.</typeparam>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    public IEnumerable<object> Get<TContribution, TDelegate>()
        where TContribution : IConvention
        where TDelegate : Delegate
    {
        return GetAll().Where(x => x is TContribution or TDelegate);
    }

    /// <summary>
    ///     Gets this instance.
    /// </summary>
    /// <typeparam name="TContribution">The type of the contribution.</typeparam>
    /// <typeparam name="TAsyncContribution">The type of the async contribution.</typeparam>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <typeparam name="TAsyncDelegate">The type of the async delegate.</typeparam>
    public IEnumerable<object> Get<TContribution, TDelegate, TAsyncContribution, TAsyncDelegate>()
        where TContribution : IConvention
        where TDelegate : Delegate
        where TAsyncContribution : IConvention
        where TAsyncDelegate : Delegate
    {
        return GetAll().Where(x => x is TContribution or TDelegate or TAsyncContribution or TAsyncDelegate);
    }

    /// <summary>
    ///     Gets a all the conventions from the provider
    /// </summary>
    public IEnumerable<object> GetAll()
    {
        return _conventions
              .Value
              .Where(cod => cod.HostType == HostType.Undefined || cod.HostType == _hostType)
              .Select(ToObject)
              .Where(
                   // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                   x => x != null!
                   // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
               )!;
    }
}
