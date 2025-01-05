using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Extensions;

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
    public static async ValueTask<IConventionContext> ApplyConventionsAsync(
        this IConventionContext context,
        CancellationToken cancellationToken = default
    )
    {
        await context.RegisterConventions(
            e => e
                .AddHandler<ISetupConvention>(convention => convention.Register(context))
                .AddHandler<ISetupAsyncConvention>(convention => convention.Register(context, cancellationToken))
                .AddHandler<SetupConvention>(convention => convention(context))
                .AddHandler<SetupAsyncConvention>(convention => convention(context, cancellationToken))
        ).ConfigureAwait(false);
        return context;
    }
}
