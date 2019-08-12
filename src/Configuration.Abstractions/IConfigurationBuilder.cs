using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    ///  ILoggingConvention
    /// Implements the <see cref="IConventionContainer{IConfigurationBuilder, IConfigurationConvention, ConfigurationConventionDelegate}" />
    /// Implements the <see cref="IConfigurationConvention" />
    /// Implements the <see cref="IConfigurationConventionContext" />
    /// Implements the <see cref="ConfigurationConventionDelegate" />
    /// </summary>
    /// <seealso cref="IConventionContainer{IConfigurationBuilder, IConfigurationConvention, ConfigurationConventionDelegate}" />
    /// <seealso cref="IConfigurationConvention" />
    /// <seealso cref="IConfigurationConventionContext" />
    /// <seealso cref="ConfigurationConventionDelegate" />
    public interface IConfigurationBuilder : IConventionContainer<IConfigurationBuilder, IConfigurationConvention,
        ConfigurationConventionDelegate>
    {
        /// <summary>
        /// Gets the environment.
        /// </summary>
        /// <value>The environment.</value>
        IRocketEnvironment Environment { get; }
    }
}
