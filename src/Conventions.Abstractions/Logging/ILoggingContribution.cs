namespace Rocket.Surgery.Conventions.Logging
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{ILoggingConventionContext}" />
    public interface ILoggingConvention : IConvention<ILoggingConventionContext> { }
}