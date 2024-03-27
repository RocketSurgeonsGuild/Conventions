namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that ensures the convention runs before the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class BeforeConventionAttribute : Attribute, IConventionDependency
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    private readonly Type _type;

    /// <summary>
    ///     The type to be used with the convention type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
    public BeforeConventionAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type) => _type = ThrowHelper.EnsureTypeIsConvention(type);

    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependentOf;
    Type IConventionDependency.Type => _type;
}

/// <summary>
///     An attribute that ensures the convention runs before the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class BeforeConventionAttribute<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T> : Attribute, IConventionDependency
    where T : IConvention
{
    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependentOf;
    Type IConventionDependency.Type => typeof(T);
}
