using Rocket.Surgery.Conventions;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

/// <summary>
///     ICommandLineConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface ICommandLineAsyncConvention : IConvention
{
    /// <summary>
    ///     Register additional services with the command line
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="app"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, IConfigurator app, CancellationToken cancellationToken);
}