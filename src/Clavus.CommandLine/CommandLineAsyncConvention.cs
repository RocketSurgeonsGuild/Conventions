using Spectre.Console.Cli;

namespace Clavus.CommandLine;

/// <summary>
///     Delegate CommandLineConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
/// <param name="cancellationToken"></param>
public delegate ValueTask CommandLineAsyncConvention(IClavusContext context, IConfigurator app, CancellationToken cancellationToken);
