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
    public CalvusExecutor AddHandler<TConvention>(Action<TConvention> action)
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
    public CalvusExecutor AddHandler<TConvention>(Func<TConvention, ValueTask> action)
    {
        _asyncConventionHandlers.Add(
            async o =>
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
    ///     Run all the conventions
    /// </summary>
    public async ValueTask ExecuteAsync()
    {
        foreach (var convention in context.Conventions.GetAll())
        {
            foreach (var handler in _conventionHandlers)
            {
                handler(convention);
            }

            foreach (var handler in _asyncConventionHandlers)
            {
                await handler(convention).ConfigureAwait(false);
            }
        }
    }

    private readonly List<Func<object, ValueTask>> _asyncConventionHandlers = [];
    private readonly List<Action<object>> _conventionHandlers = [];
}
