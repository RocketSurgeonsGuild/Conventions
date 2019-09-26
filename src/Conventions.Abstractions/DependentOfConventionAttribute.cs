using System;
using System.Reflection;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// An attribute that ensures the convention runs before the given <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DependentOfConventionAttribute : Attribute, IDependentOfConvention
    {
        /// <summary>
        /// The convention type
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        /// The type to be used with the convention type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
        public DependentOfConventionAttribute(Type type)
        {
            if (!typeof(IConvention).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                throw new NotSupportedException("Type must inherit from " + nameof(IConvention));
            Type = type;
        }
    }

    internal interface IIsHostBasedConvention
    {
        HostType HostType { get; }
    }

    /// <summary>
    /// Defines this convention as one that only runs during live usage to avoid unit tests
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class LiveConventionAttribute : Attribute, IIsHostBasedConvention
    {
        HostType IIsHostBasedConvention.HostType => HostType.Live;
    }

    /// <summary>
    /// Defines this convention as one that only runs during a unit test run
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class UnitTestConventionAttribute : Attribute, IIsHostBasedConvention
    {
        HostType IIsHostBasedConvention.HostType => HostType.UnitTestHost;
    }
}
