namespace Rocket.Surgery.Conventions;

/// <summary>
///     Extension methods for <see cref="ConventionContextBuilder" />
/// </summary>
public static class AbstractConventionContextBuilderExtensions
{
    /// <summary>
    ///     Defines a callback that provides
    /// </summary>
    /// <param name="action"></param>
    /// <param name="conventionProvider"></param>
    /// <returns></returns>
    public static Func<TBuilder, CancellationToken, ValueTask<ConventionContextBuilder>> WithConventionsFrom<TBuilder>(
        this Func<TBuilder, CancellationToken, ValueTask<ConventionContextBuilder>> action,
        IConventionFactory conventionProvider
    )
    {
        return async (builder, token) => ( await action(builder, token) ).WithConventionsFrom(conventionProvider);
    }

    /// <summary>
    ///     Defines a callback that provides
    /// </summary>
    /// <param name="action"></param>
    /// <param name="conventionProvider"></param>
    /// <returns></returns>
    public static Func<TBuilder, ConventionContextBuilder> WithConventionsFrom<TBuilder>(
        this Func<TBuilder, ConventionContextBuilder> action,
        IConventionFactory conventionProvider
    )
    {
        return builder => action(builder).WithConventionsFrom(conventionProvider);
    }
}
