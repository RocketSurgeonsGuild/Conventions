namespace Rocket.Surgery.Conventions.Hosting;

/// <summary>
///     Delegate HostCreatedConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The host.</param>
[PublicAPI]
public delegate void HostCreatedConvention<in THost>(IConventionContext context, THost builder);
