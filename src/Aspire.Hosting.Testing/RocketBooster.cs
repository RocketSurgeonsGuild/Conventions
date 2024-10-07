using System.Reflection;
using Rocket.Surgery.Conventions;
using AppDelegate =
    System.Func<Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder, System.Threading.CancellationToken,
        System.Threading.Tasks.ValueTask<Rocket.Surgery.Conventions.ConventionContextBuilder>>;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Rocket.Surgery.Aspire.Hosting.Testing;

/// <summary>
///     Class RocketBooster.
/// </summary>
[PublicAPI]
public static partial class RocketBooster
{
    /// <summary>
    ///     ForTesting the specified conventions
    /// </summary>
    /// <param name="getConventions">The generated method that contains all the referenced conventions</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate ForConventions(IConventionFactory getConventions)
    {
        return (_, _) => ValueTask.FromResult(new ConventionContextBuilder(new Dictionary<object, object>()).UseConventionFactory(getConventions));
    }

    /// <summary>
    ///     Fors the specified factory.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <returns>Func&lt;WebApplicationBuilder, ConventionContextBuilder&gt;.</returns>
    public static AppDelegate For(IConventionFactory factory)
    {
        return ForConventions(factory);
    }
}
