using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Ensures the convention runs after the given <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="Attribute" />
    public interface IDependsOnConvention
    {
        /// <summary>
        /// The <see cref="IConvention" /> type to link to
        /// </summary>
        Type Type { get; }
    }
}
