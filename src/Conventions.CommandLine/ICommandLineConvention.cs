using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     ICommandLineConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface ICommandLineConvention : IConvention
{
    /// <summary>
    ///     Register additional services with the service collection
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="app"></param>
    void Register(IConventionContext context, IConfigurator app);
}