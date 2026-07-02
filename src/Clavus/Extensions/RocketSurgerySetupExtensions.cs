using Rocket.Surgery.Clavus.Setup;

namespace Rocket.Surgery.Clavus.Extensions;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
internal static class RocketSurgerySetupExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IClavusContext> ApplyConventionsAsync(
        this IClavusContext context,
        CancellationToken cancellationToken = default
    )
    {
        await context
             .RegisterConventions(
                  e => e
                      .AddHandler<ISetupPart>(convention => convention.Register(context))
                      .AddHandler<ISetupAsyncPart>(convention => convention.Register(context, cancellationToken))
                      .AddHandler<SetupPart>(convention => convention(context))
                      .AddHandler<SetupAsyncPart>(convention => convention(context, cancellationToken))
              )
             .ConfigureAwait(false);
        return context;
    }
}
