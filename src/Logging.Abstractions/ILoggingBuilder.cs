using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Logging
{
    /// <summary>
    ///  ILoggingConvention
    /// Implements the <see cref="IConventionBuilder{ILoggingBuilder, ILoggingConvention, LoggingConventionDelegate}" />
    /// Implements the <see cref="ILoggingBuilder" />
    /// Implements the <see cref="ILoggingConvention" />
    /// Implements the <see cref="LoggingConventionDelegate" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{ILoggingBuilder, ILoggingConvention, LoggingConventionDelegate}" />
    /// <seealso cref="ILoggingBuilder" />
    /// <seealso cref="ILoggingConvention" />
    /// <seealso cref="LoggingConventionDelegate" />
    public interface ILoggingBuilder : IConventionBuilder<ILoggingBuilder, ILoggingConvention, LoggingConventionDelegate> { }
}
