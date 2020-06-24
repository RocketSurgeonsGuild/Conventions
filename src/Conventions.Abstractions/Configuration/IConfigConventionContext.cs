using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

// ReSharper disable PossibleInterfaceMemberAmbiguity

namespace Rocket.Surgery.Conventions.Configuration
{
    /// <summary>
    /// IConfigurationConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// Implements the <see cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    public interface IConfigConventionContext : IConventionContext,
                                                IConfigurationBuilder
    {
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [NotNull] new IDictionary<object, object?> Properties { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [NotNull] IConfiguration Configuration { get; }
    }
}