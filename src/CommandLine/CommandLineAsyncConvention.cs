using Rocket.Surgery.Conventions;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

/// <summary>
///     Delegate CommandLineConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
/// <param name="cancellationToken"></param>
public delegate ValueTask CommandLineAsyncConvention(IConventionContext context, IConfigurator app, CancellationToken cancellationToken);
