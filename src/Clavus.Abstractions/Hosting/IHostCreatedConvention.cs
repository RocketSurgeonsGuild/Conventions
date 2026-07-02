namespace Rocket.Surgery.Clavus.Hosting;

/// <summary>
///     IHostCreatedPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IHostCreatedPart<in THost> : IClavusPart
{
    /// <summary>
    ///     Register an event to happen when the host is created
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IClavusContext context, THost builder);
}
