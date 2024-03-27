using System.Reflection;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that ensures the convention runs after the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnConventionAttribute : Attribute, IConventionDependency
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    private readonly Type _type;

    /// <summary>
    ///     The type to be used with the convention type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
    public DependsOnConventionAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type) => _type = ThrowHelper.EnsureTypeIsConvention(type);

    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependsOn;
    Type IConventionDependency.Type => _type;
}

/// <summary>
///     An attribute that ensures the convention runs after the given <see cref="IConvention" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnConventionAttribute<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T> : Attribute, IConventionDependency
    where T : IConvention
{
    DependencyDirection IConventionDependency.Direction => DependencyDirection.DependsOn;
    Type IConventionDependency.Type => typeof(T);
}
