namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     Enum DependencyClassification
/// </summary>
internal enum DependencyClassification
{
    /// <summary>
    ///     The unknown
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The candidate
    /// </summary>
    Candidate = 1,

    /// <summary>
    ///     The not candidate
    /// </summary>
    NotCandidate = 2,

    /// <summary>
    ///     The reference
    /// </summary>
    Reference = 3
}
