namespace Rocket.Surgery.Clavus;

/// <summary>
///     A factory that provides a list of conventions
/// </summary>
public delegate IEnumerable<IClavusPartMetadata> LoadClavusParts(ClavusContextBuilder builder);
