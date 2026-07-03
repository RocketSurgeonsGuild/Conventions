using Spectre.Console.Cli;

namespace Clavus.CommandLine;

/// <summary>
///     ICommandLineConvention
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
public interface ICommandLineConvention : IClavusPart
{
    /// <summary>
    ///     Register additional services with the command line
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="app"></param>
    void Register(IClavusContext context, IConfigurator app);
}
