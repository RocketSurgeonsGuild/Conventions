using System.Diagnostics;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     A pattern match class that is used to determine if a type is a <see cref="IConvention" />, a <see cref="Delegate" /> or
///     <see cref="None" />
///     Implements the <see cref="ConventionOrDelegate" />
/// </summary>
/// <seealso cref="ConventionOrDelegate" />
[DebuggerDisplay("{ToString()}")]
internal readonly struct ConventionOrDelegate : IEquatable<ConventionOrDelegate>
{
    /// <summary>
    ///     A nether case, if no delegate is found
    /// </summary>
    /// <value>The none.</value>
    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
    public static ConventionOrDelegate None => default!;

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
        Priority = convention.Priority;
        HostType = hostType;
        Dependencies = dependencies
                      .Select(z => z is ConventionDependency cd ? cd : new(z.Direction, z.Type))
                      .ToArray();
    }

    /// <summary>
    ///     Create a convention
    /// </summary>
    /// <param name="convention">The convention.</param>
    internal ConventionOrDelegate(IConventionMetadata convention)
    {
        Convention = convention.Convention;
        Delegate = default;
        Priority = convention.Convention.Priority;
        HostType = convention.HostType;
        Dependencies = convention
                      .Dependencies
                      .Select(z => z is ConventionDependency cd ? cd : new(z.Direction, z.Type))
                      .ToArray();
    }

    /// <summary>
    ///     Create a delegate
    /// </summary>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    internal ConventionOrDelegate(Delegate @delegate, int priority)
    {
        Convention = default;
        Delegate = @delegate;
        Priority = priority;
        HostType = HostType.Undefined;
        Dependencies = [];
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
    ///     The priority of the convention or delegate
    /// </summary>
    public int Priority { get; }

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
        #pragma warning disable CS8604 // Possible null reference argument.
        return EqualityComparer<IConvention>.Default.Equals(Convention, other.Convention)
         && EqualityComparer<Delegate>.Default.Equals(Delegate, other.Delegate);
        #pragma warning restore CS8604 // Possible null reference argument.
    }


    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        var hashCode = 190459212;
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        hashCode = ( hashCode * -1521134295 ) + ( Convention is { } ? EqualityComparer<IConvention>.Default.GetHashCode(Convention) : 0 );
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        hashCode = ( hashCode * -1521134295 ) + ( Delegate is { } ? EqualityComparer<Delegate>.Default.GetHashCode(Delegate) : 0 );
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (Convention != null)
        {
            if (HostType != HostType.Undefined) return $"{HostType}:{Convention.GetType().Name} | Priority:{Priority}";

            return $"{Convention.GetType().Name} | Priority:{Priority}";
        }

        if (Delegate != null)
        {
            var name = Delegate.Method.Name;
            var methodType = Delegate.Method.DeclaringType;
            return $"{methodType?.FullName}:{name} | Priority:{Priority}";
        }

        return "None";
    }
}
