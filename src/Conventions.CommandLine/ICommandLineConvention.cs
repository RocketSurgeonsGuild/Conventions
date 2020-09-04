using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// <summary>
    /// ICommandLineConvention
    /// Implements the <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="IConvention" />
    public interface ICommandLineConvention : IConvention
    {
        /// <summary>
        /// Register additional services with the service collection
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commandLineContext"></param>
        void Register([NotNull] IConventionContext context, [NotNull] ICommandLineContext commandLineContext);
    }
}