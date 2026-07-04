using Clavus.Infrastructure;

namespace Clavus.Hosting;

/// <summary>
///     Delegate HostCreatedPart
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The host.</param>
[PublicAPI]
public delegate void HostCreatedPart<in THost>(IClavusContext context, THost builder);

/// <summary>
///     Delegate HostCreatedAsyncPart
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The host.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask HostCreatedAsyncPart<in THost>(IClavusContext context, THost builder, CancellationToken cancellationToken);

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

file class PrivateHostCreatedPart<THost>(HostCreatedPart<THost> fromDelegate, int priority) : IHostCreatedPart<THost>
{
    public void Register(IClavusContext context, THost builder) => fromDelegate(context, builder);
    int IClavusPart.Priority => priority;
}

file class PrivateHostCreatedAsyncPart<THost>(HostCreatedAsyncPart<THost> fromDelegate, int priority) : IHostCreatedAsyncPart<THost>
{
    public ValueTask Register(IClavusContext context, THost builder, CancellationToken cancellationToken) => fromDelegate(context, builder, cancellationToken);
    int IClavusPart.Priority => priority;
}

/// <summary>
///    Extension methods for the <see cref="ClavusContextBuilder"/>
/// </summary>
public static partial class HostCreatedExtensions
{
    extension(ClavusContextBuilder container)
    {
    }
}


/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
internal static class ClavusHostCreatedExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="context"></param>
    /// <param name="host">The host</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IClavusContext> ApplyHostCreatedPartsAsync<THost>(
        this IClavusContext context,
        THost host,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<IHostCreatedPart<THost>>(convention => convention.Register(context, host))
                      .AddHandler<IHostCreatedAsyncPart<THost>>(convention => convention.Register(context, host, cancellationToken))
              )
             .ConfigureAwait(false);
        return context;
    }
}
