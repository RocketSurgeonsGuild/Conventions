﻿using McMaster.Extensions.CommandLineUtils;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     ICommandLineExecutor
/// </summary>
public interface ICommandLineExecutor
{
    /// <summary>
    ///     Gets a value indicating whether this instance is default command.
    /// </summary>
    /// <value><c>true</c> if this instance is default command; otherwise, <c>false</c>.</value>
    bool IsDefaultCommand { get; }

    /// <summary>
    ///     Gets the application.
    /// </summary>
    /// <value>The application.</value>
    CommandLineApplication Application { get; }

    /// <summary>
    ///     Gets the state of the application.
    /// </summary>
    /// <value>The state of the application.</value>
    IApplicationState ApplicationState { get; }

    /// <summary>
    ///     Executes the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    int Execute(IServiceProvider serviceProvider);

    /// <summary>
    ///     Executes the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<int> ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
