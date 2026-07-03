using System.Diagnostics;

namespace Clavus.Infrastructure;

/// <summary>
///     A pattern match class that is used to determine if a type is a <see cref="IClavusPart" />, a <see cref="Delegate" /> or
///     <see cref="None" />
///     Implements the <see cref="ClavusOrDelegate" />
/// </summary>
/// <seealso cref="ClavusOrDelegate" />
[DebuggerDisplay("{ToString()}")]
internal readonly struct ClavusOrDelegate : IEquatable<ClavusOrDelegate>
{
    /// <summary>
    ///     A nether case, if no delegate is found
    /// </summary>
    /// <value>The none.</value>
    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
    public static ClavusOrDelegate None => default!;

    /// <summary>
    ///     Create a convention
    /// </summary>
    /// <param name="convention">The convention.</param>
    /// <param name="hostType">The host type.</param>
    /// <param name="dependencies">The dependencies.</param>
    internal ClavusOrDelegate(IClavusPart convention, HostType hostType, IEnumerable<IClavusDependency> dependencies)
    {
        Convention = convention;
        Delegate = default;
        Priority = convention.Priority;
        HostType = hostType;
        Dependencies = [.. dependencies.Select(z => z is ClavusDependency cd ? cd : new(z.Direction, z.Type))];
        Category = ClavusCategory.Application;
    }

    /// <summary>
    ///     Create a convention
    /// </summary>
    /// <param name="convention">The convention.</param>
    /// <param name="hostType">The host type.</param>
    /// <param name="dependencies">The dependencies.</param>
    /// <param name="category">The category.</param>
    internal ClavusOrDelegate(IClavusPart convention, HostType hostType, ClavusCategory category, IEnumerable<IClavusDependency> dependencies)
    {
        Convention = convention;
        Delegate = default;
        Priority = convention.Priority;
        HostType = hostType;
        Dependencies = [.. dependencies.Select(z => z is ClavusDependency cd ? cd : new(z.Direction, z.Type))];
        Category = category;
    }

    /// <summary>
    ///     Create a convention
    /// </summary>
    /// <param name="convention">The convention.</param>
    internal ClavusOrDelegate(IClavusPartMetadata convention)
    {
        Convention = convention.Convention;
        Delegate = default;
        Priority = convention.Convention.Priority;
        HostType = convention.HostType;
        Dependencies = [.. convention
                      .Dependencies
                      .Select(z => z is ClavusDependency cd ? cd : new(z.Direction, z.Type))];
        Category = convention.Category;
    }

    /// <summary>
    ///     Create a delegate
    /// </summary>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    internal ClavusOrDelegate(Delegate @delegate, int priority, ClavusCategory? category)
    {
        Convention = default;
        Delegate = @delegate;
        Priority = priority;
        HostType = HostType.Undefined;
        Dependencies = [];
        Category = category ?? ClavusCategory.Application;
    }

    /// <summary>
    ///     The convention, only Convention or Delegate are available
    /// </summary>
    /// <value>The convention.</value>
    public IClavusPart? Convention { get; }

    /// <summary>
    ///     The convention, only Convention or Delegate are available
    /// </summary>
    /// <value>The convention.</value>
    public ClavusCategory Category { get; }

    /// <summary>
    ///     The dependencies of this item
    /// </summary>
    public ClavusDependency[] Dependencies { get; }

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
    public static bool operator ==(ClavusOrDelegate convention1, ClavusOrDelegate convention2) => convention1.Equals(convention2);

    /// <summary>
    ///     Implements the operator !=.
    /// </summary>
    /// <param name="convention1">The convention1.</param>
    /// <param name="convention2">The convention2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(ClavusOrDelegate convention1, ClavusOrDelegate convention2) => !( convention1 == convention2 );

    /// <summary>
    ///     Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is ClavusOrDelegate delegateOrConvention && Equals(delegateOrConvention);

    /// <summary>
    ///     Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///     true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise,
    ///     false.
    /// </returns>
    public bool Equals(ClavusOrDelegate other)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        return EqualityComparer<IClavusPart>.Default.Equals(Convention, other.Convention)
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
        hashCode = ( hashCode * -1521134295 ) + ( Convention is { } ? EqualityComparer<IClavusPart>.Default.GetHashCode(Convention) : 0 );
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        hashCode = ( hashCode * -1521134295 ) + ( Delegate is { } ? EqualityComparer<Delegate>.Default.GetHashCode(Delegate) : 0 );
        return hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (Convention != null)
        {
            return  HostType != HostType.Undefined
                ?  $"{HostType}:{Convention.GetType().Name} | Priority:{Priority}"
                :  $"{Convention.GetType().Name} | Priority:{Priority}";
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
