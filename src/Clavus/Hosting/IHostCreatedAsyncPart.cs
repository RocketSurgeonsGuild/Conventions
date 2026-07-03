namespace Clavus.Hosting;

/// <summary>
///     IHostCreatedAsyncPart
///     Implements the <see cref="IClavusPart" />
/// </summary>
/// <seealso cref="IClavusPart" />
[PublicAPI]
public interface IHostCreatedAsyncPart<in THost> : IClavusPart
{
    /// <summary>
    ///     Register an event to happen when the host is created
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, THost builder, CancellationToken cancellationToken);
}
