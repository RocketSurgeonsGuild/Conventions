using System;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// ICommandLineConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface ICommandLineConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull] IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull] IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// Gets the command line application conventions.
        /// </summary>
        /// <value>The command line application conventions.</value>
        [NotNull] IConventionBuilder CommandLineApplicationConventions { get; }

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
        /// <returns>CommandLineApplication{T}.</returns>
        CommandLineApplication<T> AddCommand<T>(
            Action<CommandLineApplication<T>>? action = null,
            bool throwOnUnexpectedArg = true
        )
            where T : class;

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
        /// <returns>CommandLineApplication{T}.</returns>
        CommandLineApplication<T> AddCommand<T>(
            string name,
            Action<CommandLineApplication<T>>? action = null,
            bool throwOnUnexpectedArg = true
        )
            where T : class;

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="throwOnUnexpectedArg">if set to <c>true</c> [throw on unexpected argument].</param>
        /// <returns>CommandLineApplication.</returns>
        [NotNull]
        CommandLineApplication AddCommand(
            string name,
            Action<CommandLineApplication>? action = null,
            bool throwOnUnexpectedArg = true
        );

        /// <summary>
        /// Called when [parse].
        /// </summary>
        /// <param name="onParseDelegate">The delegate.</param>
        /// <returns>ICommandLineConventionContext.</returns>
        [NotNull] ICommandLineConventionContext OnParse([NotNull] OnParseDelegate onParseDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunDelegate">The delegate.</param>
        /// <returns>ICommandLineConventionContext.</returns>
        [NotNull] ICommandLineConventionContext OnRun([NotNull] OnRunDelegate onRunDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunAsyncDelegate">The delegate.</param>
        /// <returns>ICommandLineConventionContext.</returns>
        [NotNull] ICommandLineConventionContext OnRun([NotNull] OnRunAsyncDelegate onRunAsyncDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunAsyncCancellableDelegate">The delegate.</param>
        /// <returns>ICommandLineConventionContext.</returns>
        [NotNull] ICommandLineConventionContext OnRun([NotNull] OnRunAsyncCancellableDelegate onRunAsyncCancellableDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ICommandLineConventionContext.</returns>
        [NotNull]
        ICommandLineConventionContext OnRun<T>()
            where T : IDefaultCommand;

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ICommandLineConventionContext.</returns>
        [NotNull]
        ICommandLineConventionContext OnRunAsync<T>()
            where T : IDefaultCommandAsync;
    }
}