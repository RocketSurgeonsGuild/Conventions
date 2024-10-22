namespace Rocket.Surgery.Conventions;

/// <summary>
///     Container for conventions
/// </summary>
[PublicAPI]
public sealed class ConventionMetadata : IConventionMetadata
{
    private readonly List<ConventionDependency> _dependencies;

    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="convention"></param>
    /// <param name="hostType"></param>
    public ConventionMetadata(IConvention convention, HostType hostType)
    {
        Convention = convention;
        HostType = hostType;
        _dependencies = new();
        Category = ConventionCategory.Application;
    }

    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="convention"></param>
    /// <param name="hostType"></param>
    /// <param name="category"></param>
    public ConventionMetadata(IConvention convention, HostType hostType, ConventionCategory category)
    {
        Convention = convention;
        HostType = hostType;
        Category = category;
        _dependencies = new();
    }

    /// <summary>
    ///     Adds a new dependency to the list
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public ConventionMetadata WithDependency(DependencyDirection direction, Type type)
    {
        _dependencies.Add(new(direction, type));
        return this;
    }

    /// <inheritdoc />
    public IConvention Convention { get; }

    /// <summary>
    ///     The dependencies
    /// </summary>
    public IEnumerable<IConventionDependency> Dependencies => _dependencies.OfType<IConventionDependency>();

    /// <inheritdoc />
    public HostType HostType { get; }

    public ConventionCategory Category { get; }
}
