using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// Delegate CommandLineConvention
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commandLineContext"></param>
    public delegate void CommandLineConvention([NotNull] IConventionContext context, [NotNull] ICommandLineContext commandLineContext);
}