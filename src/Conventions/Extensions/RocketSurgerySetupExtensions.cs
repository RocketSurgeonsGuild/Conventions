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
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IConventionContext> ApplyConventionsAsync(
        this IConventionContext conventionContext,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in conventionContext.Conventions.Get<
                     ISetupConvention,
                     SetupConvention,
                     ISetupAsyncConvention,
                     SetupAsyncConvention
                 >())
        {
            switch (item)
            {
                case ISetupConvention convention:
                    convention.Register(conventionContext);
                    break;
                case SetupConvention @delegate:
                    @delegate(conventionContext);
                    break;
                case ISetupAsyncConvention convention:
                    await convention.Register(conventionContext, cancellationToken).ConfigureAwait(false);
                    break;
                case SetupAsyncConvention @delegate:
                    await @delegate(conventionContext, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return conventionContext;
    }
}
