namespace Rocket.Surgery.Conventions.Setup;

/// <summary>
///     IInitConvention
/// </summary>
[PublicAPI]
public interface ISetupAsyncConvention : IConvention
{
    /// <summary>
    ///     Initialize or configure a convention before any other convention has run against the context.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, CancellationToken cancellationToken);
}