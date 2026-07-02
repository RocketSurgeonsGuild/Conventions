using Rocket.Surgery.Clavus;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Clavus.CommandLine;

/// <summary>
///     Delegate CommandAppConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
/// <param name="cancellationToken"></param>
public delegate ValueTask CommandAppAsyncConvention(IClavusContext context, CommandApp app, CancellationToken cancellationToken);
