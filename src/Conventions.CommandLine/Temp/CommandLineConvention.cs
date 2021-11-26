namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Delegate CommandLineConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="commandLineContext"></param>
public delegate void CommandLineConvention(IConventionContext context, ICommandLineContext commandLineContext);
