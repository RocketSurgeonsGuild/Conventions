namespace Rocket.Surgery.Conventions;

/// <summary>
///     A factory that provides a list of conventions
/// </summary>
public interface IConventionFactory
{
    /// <summary>
    ///     Attach this convention to the builder
    /// </summary>
    /// <param name="builder"></param>
    IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder);

    /// <summary>
    ///     A factory that provides a list of conventions
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    IEnumerable<IConventionMetadata> LoadConventions(ConventionContextBuilder builder);
}
