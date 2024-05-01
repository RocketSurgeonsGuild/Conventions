namespace Rocket.Surgery.Conventions.Hosting;

/// <summary>
///     IHostCreatedAsyncConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IHostCreatedAsyncConvention<in THost> : IConvention
{
    /// <summary>
    ///     Register an event to happen when the host is created
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, THost builder, CancellationToken cancellationToken);
}
