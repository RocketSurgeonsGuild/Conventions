namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that ensures the convention runs after the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AfterConventionAttribute : Attribute, IConventionDependency
{
    private readonly Type _type;

    /// <summary>
    ///     The type to be used with the convention type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
    public AfterConventionAttribute(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
        Type type
    )
    {
        _type = ThrowHelper.EnsureTypeIsConvention(type);
    }

    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependsOn;
    Type IConventionDependency.Type => _type;
}

/// <summary>
///     An attribute that ensures the convention runs after the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AfterConventionAttribute<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
    T> : Attribute,
    IConventionDependency
    where T : IConvention
{
    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependsOn;
    Type IConventionDependency.Type => typeof(T);
}