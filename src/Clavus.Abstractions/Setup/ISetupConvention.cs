namespace Rocket.Surgery.Clavus.Setup;

/// <summary>
///     IInitConvention
/// </summary>
[PublicAPI]
public interface ISetupPart : IClavusPart
{
    /// <summary>
    ///     Initialize or configure a convention before any other convention has run against the context.
    /// </summary>
    /// <param name="context"></param>
    void Register(IClavusContext context);
}
