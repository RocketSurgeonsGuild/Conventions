using Spectre.Console.Cli;

namespace Clavus.CommandLine;

/// <summary>
///     Delegate CommandAppConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
public delegate void CommandAppConvention(IClavusContext context, CommandApp app);
