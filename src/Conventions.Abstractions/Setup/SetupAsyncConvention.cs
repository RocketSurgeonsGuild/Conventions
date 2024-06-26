namespace Rocket.Surgery.Conventions.Setup;

/// <summary>
///     Initialize or configure a convention before any other convention has run against the context.
/// </summary>
/// <remarks>
///     This runs immediately after creating a convention context
/// </remarks>
/// <param name="context">The context.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask SetupAsyncConvention(IConventionContext context, CancellationToken cancellationToken);