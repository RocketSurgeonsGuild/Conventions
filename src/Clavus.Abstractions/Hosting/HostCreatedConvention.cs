namespace Rocket.Surgery.Clavus.Hosting;

/// <summary>
///     Delegate HostCreatedPart
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The host.</param>
[PublicAPI]
public delegate void HostCreatedPart<in THost>(IClavusContext context, THost builder);
