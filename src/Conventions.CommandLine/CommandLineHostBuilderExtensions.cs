using Rocket.Surgery.Conventions.CommandLine;
using Spectre.Console.Cli;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
public static class CommandAppHostBuilderExtensions
{
    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, CommandLineConvention @delegate)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the commandline delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureCommandLine(this ConventionContextBuilder container, Action<IConfigurator> @delegate)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        container.AppendDelegate(new CommandLineConvention((_, context) => @delegate(context)));
        return container;
    }
}
