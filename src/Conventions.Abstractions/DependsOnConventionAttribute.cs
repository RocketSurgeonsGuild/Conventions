namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that ensures the convention runs after the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
///     The type to be used with the convention type
/// </remarks>
/// <param name="type">The type.</param>
/// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnConventionAttribute(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
        Type type
    ) : Attribute, IConventionDependency
{
    private readonly Type _type = ThrowHelper.EnsureTypeIsConvention(type);

    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependsOn;
    Type IConventionDependency.Type => _type;
}

/// <summary>
///     An attribute that ensures the convention runs after the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnConventionAttribute<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
T> : Attribute,
    IConventionDependency
    where T : IConvention
{
    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependsOn;
    Type IConventionDependency.Type => typeof(T);
}
