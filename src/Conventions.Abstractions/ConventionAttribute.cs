using System;
using System.Reflection;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Class ServiceContributionAttribute. This class cannot be inherited.
    /// </summary>
    /// TODO Edit XML Comment Template for ServiceContributionAttribute
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    // ReSharper disable once CheckNamespace
    public sealed class ConventionAttribute : Attribute
    {
        /// <summary>
        /// The type that derives from <see cref="IConvention"/>
        /// </summary>
        /// <value>The type.</value>
        /// TODO Edit XML Comment Template for Type
        public Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// TODO Edit XML Comment Template for #ctor
        public ConventionAttribute(Type type)
        {
            if (!typeof(IConvention).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                throw new NotSupportedException("Type must inherit from " + nameof(IConvention));
            Type = type;
        }
    }
}
