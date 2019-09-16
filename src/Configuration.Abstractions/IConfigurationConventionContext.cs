using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Collections.Generic;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    ///  IConfigurationConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// Implements the <see cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    public interface IConfigurationConventionContext : IConventionContext, Microsoft.Extensions.Configuration.IConfigurationBuilder
    {
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        new IDictionary<object, object?> Properties { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        IConfiguration Configuration { get; }


    }
}
