namespace Rocket.Surgery.Conventions.Configuration
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IConfigurationConventionContext}" />
    public interface IConfigurationConvention : IConvention<IConfigurationConventionContext> { }
}