namespace Clavus.Infrastructure;

/// <summary>
///     A class to help with executing conventions
/// </summary>
/// <remarks>
///     This class uses <see cref="ConventionExceptionPolicyDelegate" /> to handle exceptions
/// </remarks>
/// <param name="context"></param>
public class CalvusExecutor(IClavusContext context)
{
    /// <summary>
    ///     Add a synchronous convention
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="TConvention"></typeparam>
    /// <returns></returns>
    public CalvusExecutor AddHandler<TConvention>(Action<TConvention> action) where TConvention : IClavusPart
    {
        _conventionHandlers.Add(
            o =>
            {
                if (o is not TConvention convention) return;
                try
                {
                    action(convention);
                }
                catch (Exception ex) when (!context.ExceptionPolicy(ex))
                {
                    throw;
                }
            }
        );
        return this;
    }

    /// <summary>
    ///     Add an asynchronous convention
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="TConvention"></typeparam>
    /// <returns></returns>
    public CalvusExecutor AddHandler<TConvention>(Func<TConvention, ValueTask> action) where TConvention : IClavusPart
    {
        _asyncConventionHandlers.Add(
            async (o, _) =>
            {
                if (o is not TConvention convention) return;
                try
                {
                    await action(convention).ConfigureAwait(false);
                }
                catch (Exception ex) when (!context.ExceptionPolicy(ex))
                {
                    throw;
                }
            }
        );
        return this;
    }

    /// <summary>
    ///     Add an asynchronous convention
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="TConvention"></typeparam>
    /// <returns></returns>
    public CalvusExecutor AddHandler<TConvention>(Func<TConvention, CancellationToken, ValueTask> action) where TConvention : IClavusPart
    {
        _asyncConventionHandlers.Add(
            async (o, ct) =>
            {
                if (o is not TConvention convention) return;
                try
                {
                    await action(convention, ct).ConfigureAwait(false);
                }
                catch (Exception ex) when (!context.ExceptionPolicy(ex))
                {
                    throw;
                }
            }
        );
        return this;
    }

    /// <summary>
    ///     Run all the conventions
    /// </summary>
    public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
    {
        foreach (var convention in context.Parts)
        {
            foreach (var handler in _conventionHandlers)
            {
                handler(convention);
            }

            foreach (var handler in _asyncConventionHandlers)
            {
                await handler(convention, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    ///     Run all the conventions
    /// </summary>
    public void Execute()
    {
        foreach (var convention in context.Parts)
        {
            foreach (var handler in _conventionHandlers)
            {
                handler(convention);
            }
        }
    }

    private readonly List<Func<IClavusPart, CancellationToken, ValueTask>> _asyncConventionHandlers = [];
    private readonly List<Action<IClavusPart>> _conventionHandlers = [];
}
