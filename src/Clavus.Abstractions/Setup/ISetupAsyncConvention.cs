namespace Rocket.Surgery.Clavus.Setup;

/// <summary>
///     IInitConvention
/// </summary>
[PublicAPI]
public interface ISetupAsyncPart : IClavusPart
{
    /// <summary>
    ///     Initialize or configure a convention before any other convention has run against the context.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, CancellationToken cancellationToken);
}
