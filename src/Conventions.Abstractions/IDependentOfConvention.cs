using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Ensures the convention runs before the given <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="Attribute" />
    public interface IDependentOfConvention
    {
        /// <summary>
        /// The <see cref="IConvention" /> type to link to
        /// </summary>
        Type Type { get; }
    }

    
}
