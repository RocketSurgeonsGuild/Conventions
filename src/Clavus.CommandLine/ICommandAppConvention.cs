using Spectre.Console.Cli;

namespace Clavus.CommandLine;

/// <summary>
///     ICommandLineConvention
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
public interface ICommandAppConvention : IClavusPart
{
    /// <summary>
    ///     Register additional services with the <see cref="CommandApp" />
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="app"></param>
    void Register(IClavusContext context, CommandApp app);
}
