using System;
using System.Reflection;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// An attribute that defines a convention for this entire assembly
    /// The type attached to the convention must implement <see cref="IConvention" /> but may also implement other interfaces
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ConventionAttribute : Attribute
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
        public ConventionAttribute(Type type)
        {
            if (!typeof(IConvention).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                throw new NotSupportedException("Type must inherit from " + nameof(IConvention));
            Type = type;
        }
    }
}
