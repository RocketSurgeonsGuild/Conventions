using System.Runtime.CompilerServices;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.Extensions.Hosting.IHostApplicationBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Class RocketBooster.
/// </summary>
[PublicAPI]
public static partial class RocketBooster
{
    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionFactory">The generated method that contains all the referenced conventions</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    [OverloadResolutionPriority(-1)]
    public static AppDelegate ForConventions(IConventionFactory conventionFactory, params ConventionCategory[] categories)
    {
        return (builder, _) => ValueTask.FromResult(new ConventionContextBuilder(builder.Properties, categories).UseConventionFactory(conventionFactory));
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionFactory">The generated method that contains all the referenced conventions</param>
    /// <param name="categories"></param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(IConventionFactory conventionFactory, params IEnumerable<ConventionCategory> categories)
    {
        return (builder, _) => ValueTask.FromResult(new ConventionContextBuilder(builder.Properties, categories).UseConventionFactory(conventionFactory));
    }

    /// <summary>
    ///     Fors the specified factory.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IConventionFactory factory) => ForConventions(factory);
}
