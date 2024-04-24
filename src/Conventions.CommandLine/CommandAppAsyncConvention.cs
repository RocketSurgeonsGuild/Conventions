using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Delegate CommandAppConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
/// <param name="cancellationToken"></param>
public delegate ValueTask CommandAppAsyncConvention(IConventionContext context, CommandApp app, CancellationToken cancellationToken);
