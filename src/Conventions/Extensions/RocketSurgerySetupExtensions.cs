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
        foreach (var item in context.Conventions.Get<
                     ISetupConvention,
                     SetupConvention,
                     ISetupAsyncConvention,
                     SetupAsyncConvention
                 >())
        {
            switch (item)
            {
                case ISetupConvention convention:
                    convention.Register(context);
                    break;
                case SetupConvention @delegate:
                    @delegate(context);
                    break;
                case ISetupAsyncConvention convention:
                    await convention.Register(context, cancellationToken).ConfigureAwait(false);
                    break;
                case SetupAsyncConvention @delegate:
                    await @delegate(context, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return context;
    }
}