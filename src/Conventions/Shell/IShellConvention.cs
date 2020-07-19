namespace Rocket.Surgery.Conventions.Shell
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{ICommandLineConventionContext}" />
    public interface IShellConvention : IConvention<IShellConventionContext> { }
}