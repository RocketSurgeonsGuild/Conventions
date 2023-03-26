namespace Rocket.Surgery.Conventions;

/// <summary>
///     An attribute that defines a convention for this entire assembly
///     The type attached to the convention must implement <see cref="IConvention" /> but may also implement other interfaces
///     Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ConventionAttribute : Attribute
{
    /// <summary>
    ///     The type to be used with the convention type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
    public ConventionAttribute(
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        Type type
    )
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        Type = type;
    }

    /// <summary>
    ///     The convention type
    /// </summary>
    /// <value>The type.</value>
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    public Type Type { get; }
}
