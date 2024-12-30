namespace Rocket.Surgery.Conventions;

/// <summary>
///     A factory that provides a list of conventions
/// </summary>
public delegate IEnumerable<IConventionMetadata> LoadConventions(ConventionContextBuilder builder);
