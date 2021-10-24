using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     An assembly candidate finders
///     Users a given name of an assembly to find other assemblies that reference it.
/// </summary>
public interface IAssemblyCandidateFinder
{
    /// <summary>
    ///     Get the candidates for a given set
    /// </summary>
    /// <param name="candidates">The candidates as an enumerable</param>
    /// <returns>IEnumerable{Assembly}.</returns>
    IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates);
}
