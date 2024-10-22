using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.WebAssembly.Hosting;

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
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(IConventionFactory conventionFactory, params ConventionCategory[] categories)
    {
        return (_, _) => ValueTask.FromResult(new ConventionContextBuilder(new Dictionary<object, object>(), categories).UseConventionFactory(conventionFactory));
    }

    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="conventionProvider">The conventions provider.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IConventionFactory conventionProvider)
    {
        return ForConventions(conventionProvider);
    }
}
