using Rocket.Surgery.Conventions;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

/// <summary>
///     ICommandLineConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface ICommandAppConvention : IConvention
{
    /// <summary>
    ///     Register additional services with the <see cref="CommandApp" />
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="app"></param>
    void Register(IConventionContext context, CommandApp app);
}
