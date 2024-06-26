﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Logging;

/// <summary>
///     ILoggingConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface ILoggingConvention : IConvention
{
    /// <summary>
    ///     Register additional logging providers with the logging builder
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, IConfiguration configuration, ILoggingBuilder builder);
}