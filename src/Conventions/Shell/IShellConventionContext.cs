using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Shell
{
    /// <summary>
    /// ICommandLineConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface IShellConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull]
        IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull]
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// The root application command
        /// </summary>
        [NotNull]
        CommandLineBuilder Builder { get; }

        /// <summary>
        /// Uses a specific console
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        [NotNull]
        IShellConventionContext UseConsole(IConsole console);

        /// <summary>
        /// Add a command to the application
        /// </summary>
        /// <param name="alias">The command alias</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NotNull]
        IShellConventionContext AddCommand<T>(string? alias = null, string? description = null);

        /// <summary>
        /// Add a command to the application
        /// </summary>
        /// <param name="alias">The command alias</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NotNull]
        IShellConventionContext AddCommand(Type type, string? alias = null, string? description = null);
    }
}