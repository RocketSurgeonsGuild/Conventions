using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConventionBuilder" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{ICommandLineBuilder, ICommandLineConvention, CommandLineConventionDelegate}" />
    public interface ICommandLineBuilder : IConventionBuilder<ICommandLineBuilder, ICommandLineConvention,
        CommandLineConventionDelegate>
    {
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
        CommandLineApplication AddCommand(
            string name,
            Action<CommandLineApplication>? action = null,
            bool throwOnUnexpectedArg = true
        );

        /// <summary>
        /// Called when [parse].
        /// </summary>
        /// <param name="onParseDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        ICommandLineBuilder OnParse(OnParseDelegate onParseDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        ICommandLineBuilder OnRun(OnRunDelegate onRunDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunAsyncDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        ICommandLineBuilder OnRun(OnRunAsyncDelegate onRunAsyncDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <param name="onRunAsyncCancellableDelegate">The delegate.</param>
        /// <returns>ICommandLineBuilder.</returns>
        ICommandLineBuilder OnRun(OnRunAsyncCancellableDelegate onRunAsyncCancellableDelegate);

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ICommandLineBuilder.</returns>
        ICommandLineBuilder OnRun<T>()
            where T : IDefaultCommand;

        /// <summary>
        /// Called when [run].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ICommandLineBuilder.</returns>
        ICommandLineBuilder OnRunAsync<T>()
            where T : IDefaultCommandAsync;

        /// <summary>
        /// Builds the specified entry assembly.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <returns>ICommandLine.</returns>
        ICommandLine Build(Assembly? entryAssembly = null);
    }
}