using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Logging
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{ILoggingConventionContext}" />
    public interface ILoggingConvention : IConvention<ILoggingConventionContext> { }
}