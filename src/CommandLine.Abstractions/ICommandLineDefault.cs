using System.Threading.Tasks;

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// Delegate OnRunDelegate
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>System.Int32.</returns>
    public delegate int OnRunDelegate(IApplicationState state);

    /// <summary>
    /// Delegate OnParseDelegate
    /// </summary>
    /// <param name="state">The state.</param>
    public delegate void OnParseDelegate(IApplicationState state);

    /// <summary>
    ///  IDefaultCommand
    /// </summary>
    public interface IDefaultCommand
    {
        /// <summary>
        /// Runs the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>System.Int32.</returns>
        int Run(IApplicationState state);
    }
}
