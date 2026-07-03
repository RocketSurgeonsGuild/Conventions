using Clavus.Infrastructure;

namespace Clavus;

/// <summary>
///     An attribute that ensures the convention runs before the given <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
///     The type to be used with the convention type
/// </remarks>
/// <param name="type">The type.</param>
/// <exception cref="NotSupportedException">Type must inherit from " + nameof(IClavusPart)</exception>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependentOfPartAttribute(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
    Type type
) : Attribute, IClavusDependency
{
    private readonly Type _type = ThrowHelper.EnsureTypeIsConvention(type);

    DependencyDirection IClavusDependency.Direction => DependencyDirection.DependentOf;
    Type IClavusDependency.Type => _type;
}

/// <summary>
///     An attribute that ensures the convention runs before the given <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="Attribute" />
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependentOfPartAttribute<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
    T> : Attribute,
    IClavusDependency
    where T : IClavusPart
{
    DependencyDirection IClavusDependency.Direction => DependencyDirection.DependentOf;
    Type IClavusDependency.Type => typeof(T);
}
