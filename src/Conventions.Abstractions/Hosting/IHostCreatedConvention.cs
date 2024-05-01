namespace Rocket.Surgery.Conventions.Hosting;

/// <summary>
///     IHostCreatedConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
[PublicAPI]
public interface IHostCreatedConvention<in THost> : IConvention
{
    /// <summary>
    ///     Register an event to happen when the host is created
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="builder"></param>
    void Register(IConventionContext context, THost builder);
}
