using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// Delegate OnRunDelegate
    /// </summary>
    /// <param name="state">The state.</param>
    public delegate Task<int> OnRunAsyncDelegate(IApplicationState state);

    /// <summary>
    /// Delegate OnRunDelegate
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="cancellationToken">The state.</param>
    public delegate Task<int> OnRunAsyncCancellableDelegate(
        IApplicationState state,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Delegate OnRunDelegate
    /// </summary>
    /// <param name="state">The state.</param>
    public delegate int OnRunDelegate(IApplicationState state);

    /// <summary>
    /// Delegate OnParseDelegate
    /// </summary>
    /// <param name="state">The state.</param>
    public delegate void OnParseDelegate(IApplicationState state);

    /// <summary>
    /// IDefaultCommand
    /// </summary>
    public interface IDefaultCommand
    {
        /// <summary>
        /// Runs the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        int Run(IApplicationState state);
    }

    /// <summary>
    /// IDefaultCommand
    /// </summary>
    public interface IDefaultCommandAsync
    {
        /// <summary>
        /// Runs the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="cancellationToken">The state.</param>
        Task<int> Run(IApplicationState state, CancellationToken cancellationToken);
    }
}