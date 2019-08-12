﻿using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    ///  ILoggingConvention
    /// Implements the <see cref="IConvention{IConfigurationConventionContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IConfigurationConventionContext}" />
    public interface IConfigurationConvention : IConvention<IConfigurationConventionContext>
    {

    }
}
