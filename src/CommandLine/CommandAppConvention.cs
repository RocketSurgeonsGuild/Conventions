using Rocket.Surgery.Conventions;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

/// <summary>
///     Delegate CommandAppConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
public delegate void CommandAppConvention(IConventionContext context, CommandApp app);
