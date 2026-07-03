using Spectre.Console.Cli;

namespace Clavus.CommandLine;

/// <summary>
///     Delegate CommandLineConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
public delegate void CommandLineConvention(IClavusContext context, IConfigurator app);
