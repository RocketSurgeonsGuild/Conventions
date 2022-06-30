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

    private readonly Lazy<ConventionOrDelegate[]> _conventions;


    /// <summary>
    ///     Initializes a new instance of the <see cref="ConventionProvider" /> class.
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
    ) : this(hostType, contributions.Select(FromConvention), prependedContributionsOrDelegates, appendedContributionsOrDelegates)
    {
    }


    /// <summary>
    ///     Initializes a new instance of the <see cref="ConventionProvider" /> class.
    /// </summary>
    /// <param name="hostType"></param>
    /// <param name="contributions">The contributions.</param>
    /// <param name="prependedContributionsOrDelegates">The prepended contributions or delegates.</param>
    /// <param name="appendedContributionsOrDelegates">The appended contributions or delegates.</param>
    public ConventionProvider(
        HostType hostType,
        IEnumerable<IConventionWithDependencies> contributions,
        IEnumerable<object> prependedContributionsOrDelegates,
        IEnumerable<object> appendedContributionsOrDelegates
    ) : this(hostType, contributions.Select(FromConvention), prependedContributionsOrDelegates, appendedContributionsOrDelegates)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConventionProvider" /> class.
    /// </summary>
    /// <param name="hostType"></param>
    /// <param name="contributions">The contributions.</param>
    /// <param name="additionalContributions">The additional contributions.</param>
    /// <param name="prependedContributionsOrDelegates">The prepended contributions or delegates.</param>
    /// <param name="appendedContributionsOrDelegates">The appended contributions or delegates.</param>
    public ConventionProvider(
        HostType hostType,
        IEnumerable<IConventionWithDependencies> contributions,
        IEnumerable<IConvention> additionalContributions,
        IEnumerable<object> prependedContributionsOrDelegates,
        IEnumerable<object> appendedContributionsOrDelegates
    ) : this(
        hostType, contributions.Select(FromConvention).Concat(additionalContributions.Select(FromConvention)), prependedContributionsOrDelegates,
        appendedContributionsOrDelegates
    )
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConventionProvider" /> class.
    /// </summary>
    /// <param name="hostType"></param>
    /// <param name="contributions">The contributions.</param>
    /// <param name="prependedContributionsOrDelegates">The prepended contributions or delegates.</param>
    /// <param name="appendedContributionsOrDelegates">The appended contributions or delegates.</param>
    private ConventionProvider(
        HostType hostType,
        IEnumerable<ConventionOrDelegate> contributions,
        IEnumerable<object> prependedContributionsOrDelegates,
        IEnumerable<object> appendedContributionsOrDelegates
    )
    {
        _hostType = hostType;
        _conventions = new Lazy<ConventionOrDelegate[]>(
            () =>
            {
                var prepended = toConventionOrDelegate(prependedContributionsOrDelegates);
                var appended = toConventionOrDelegate(appendedContributionsOrDelegates);
                var contributionsList = contributions as IReadOnlyCollection<ConventionOrDelegate> ?? contributions.ToArray();

                static IReadOnlyCollection<ConventionOrDelegate> toConventionOrDelegate(IEnumerable<object> values)
                {
                    return values.Select(selector).Where(x => x != ConventionOrDelegate.None).ToArray();
                }

                var c = prepended.Union(contributionsList).Union(appended);

                if (c.Any(z => z.Dependencies.Length > 0))
                {
                    var conventions = c
                                     .Where(x => x.Convention != null)
                                     .Select(
                                          convention =>
                                          {
                                              return (
                                                  convention,
                                                  type: convention.Convention!.GetType(),
                                                  dependsOn: convention.Dependencies.Where(x => x.Direction == DependencyDirection.DependsOn)
                                                                       .Select(z => z.Type),
                                                  dependentFor: convention.Dependencies
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
                                   .ToLookup(x => x.convention.Convention, x => x.dependsOn);

                    return TopographicalSort(c, x => dependsOn[x.Convention]).ToArray();
                }

                return c.ToArray();
            }
        );

        static ConventionOrDelegate selector(object value)
        {
            return value switch
            {
                IConventionWithDependencies cwd => new ConventionOrDelegate(cwd),
                IConvention convention          => FromConvention(convention),
                Delegate d                      => new ConventionOrDelegate(d),
                var _                           => ConventionOrDelegate.None
            };
        }
    }

    private static ConventionOrDelegate FromConvention(IConvention convention)
    {
        var type = convention.GetType();
        var dependencies =
            type.GetCustomAttributes().OfType<IConventionDependency>().ToArray();
        var hostType = convention.GetType().GetCustomAttributes().OfType<IHostBasedConvention>().FirstOrDefault()?.HostType ?? HostType.Undefined;
        return new ConventionOrDelegate(convention, hostType, dependencies);
    }


    private static ConventionOrDelegate FromConvention(IConventionWithDependencies convention)
    {
        return new ConventionOrDelegate(convention);
    }

    private static object? ToObject(ConventionOrDelegate delegateOrConvention)
    {
        return (object?)delegateOrConvention.Delegate ?? delegateOrConvention.Convention;
    }

    /// <summary>
    ///     Gets this instance.
    /// </summary>
    /// <typeparam name="TContribution">The type of the contribution.</typeparam>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <param name="hostType">The host type.</param>
    public IEnumerable<object> Get<TContribution, TDelegate>(HostType hostType = HostType.Undefined)
        where TContribution : IConvention
        where TDelegate : Delegate
    {
        return GetAll(hostType)
           .Where(x => x is TContribution or TDelegate);
    }

    /// <summary>
    ///     Gets a all the conventions from the provider
    /// </summary>
    /// <param name="hostType">The host type.</param>
    public IEnumerable<object> GetAll(HostType hostType = HostType.Undefined)
    {
        return _conventions.Value
                           .Where(
                                cod => cod.HostType == HostType.Undefined
                                    || ( hostType == HostType.Undefined && _hostType == HostType.Undefined ) // if nothing was given here ignore the default
                                    || ( hostType != HostType.Undefined
                                           ? cod.HostType == hostType
                                           : _hostType != HostType.Undefined && cod.HostType == _hostType )
                            )
                           .Select(ToObject)
                           .Where(x => x != null!)!;
    }
}

/// <summary>
///     A pattern match class that is used to determine if a type is a <see cref="IConvention" />, a <see cref="Delegate" /> or
///     <see cref="None" />
///     Implements the <see cref="ConventionOrDelegate" />
/// </summary>
/// <seealso cref="ConventionOrDelegate" />
internal readonly struct ConventionOrDelegate : IEquatable<ConventionOrDelegate>
{
    /// <summary>
    ///     A nether case, if no delegate is found
    /// </summary>
    /// <value>The none.</value>
    public static ConventionOrDelegate None { get; } = default!;

    /// <summary>
    ///     Create a convention
    /// </summary>
    /// <param name="convention">The convention.</param>
    /// <param name="hostType">The host type.</param>
    /// <param name="dependencies">The dependencies.</param>
    internal ConventionOrDelegate(IConvention convention, HostType hostType, IEnumerable<IConventionDependency> dependencies)
    {
        Convention = convention;
        Delegate = default;
        HostType = hostType;
        Dependencies = dependencies
                      .Select(z => z is ConventionDependency cd ? cd : new ConventionDependency(z.Direction, z.Type))
                      .ToArray();
    }

    /// <summary>
    ///     Create a convention
    /// </summary>
    /// <param name="convention">The convention.</param>
    internal ConventionOrDelegate(IConventionWithDependencies convention)
    {
        Convention = convention.Convention;
        Delegate = default;
        HostType = convention.HostType;
        Dependencies = convention.Dependencies
                                 .Select(z => z is ConventionDependency cd ? cd : new ConventionDependency(z.Direction, z.Type))
                                 .ToArray();
    }

    /// <summary>
    ///     Create a delegate
    /// </summary>
    /// <param name="delegate">The delegate.</param>
    internal ConventionOrDelegate(Delegate @delegate)
    {
        Convention = default;
        Delegate = @delegate;
        HostType = HostType.Undefined;
        Dependencies = Array.Empty<ConventionDependency>();
    }

    /// <summary>
    ///     The convention, only Convention or Delegate are available
    /// </summary>
    /// <value>The convention.</value>
    public IConvention? Convention { get; }

    /// <summary>
    ///     The dependencies of this item
    /// </summary>
    public ConventionDependency[] Dependencies { get; }

    /// <summary>
    ///     The delegate, only Convention or Delegate are available
    /// </summary>
    /// <value>The delegate.</value>
    public Delegate? Delegate { get; }

    /// <summary>
    ///     The host type this applies to
    /// </summary>
    /// <value>The delegate.</value>
    public HostType HostType { get; }

    /// <summary>
    ///     Implements the operator ==.
    /// </summary>
    /// <param name="convention1">The convention1.</param>
    /// <param name="convention2">The convention2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(ConventionOrDelegate convention1, ConventionOrDelegate convention2)
    {
        return convention1.Equals(convention2);
    }

    /// <summary>
    ///     Implements the operator !=.
    /// </summary>
    /// <param name="convention1">The convention1.</param>
    /// <param name="convention2">The convention2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(ConventionOrDelegate convention1, ConventionOrDelegate convention2)
    {
        return !( convention1 == convention2 );
    }

    /// <summary>
    ///     Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is ConventionOrDelegate delegateOrConvention && Equals(delegateOrConvention);
    }

    /// <summary>
    ///     Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///     true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise,
    ///     false.
    /// </returns>
    public bool Equals(ConventionOrDelegate other)
    {
        return EqualityComparer<IConvention>.Default.Equals(Convention!, other.Convention!)
            && EqualityComparer<Delegate>.Default.Equals(Delegate!, other.Delegate!);
    }


    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        var hashCode = 190459212;
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<IConvention>.Default.GetHashCode(Convention!);
        hashCode = ( hashCode * -1521134295 ) + EqualityComparer<Delegate>.Default.GetHashCode(Delegate!);
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (Convention != null)
        {
            if (HostType != HostType.Undefined)
            {
                return $"{HostType}:{Convention.GetType().Name}";
            }

            return Convention.GetType().Name;
        }

        if (Delegate != null)
        {
            var name = Delegate.Method.Name;
            var methodType = Delegate.Method.DeclaringType;
            return $"{methodType?.FullName}:{name}";
        }

        return "None";
    }
}
