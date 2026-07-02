namespace Rocket.Surgery.Clavus;

/// <summary>
///     Container for conventions
/// </summary>
[PublicAPI]
public sealed class ClavusPartMetadata : IClavusPartMetadata
{
    private readonly List<ClavusDependency> _dependencies;

    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="convention"></param>
    /// <param name="hostType"></param>
    public ClavusPartMetadata(IClavusPart convention, HostType hostType)
    {
        Convention = convention;
        HostType = hostType;
        _dependencies = [];
        Category = ClavusCategory.Application;
    }

    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="convention"></param>
    /// <param name="hostType"></param>
    /// <param name="category"></param>
    public ClavusPartMetadata(IClavusPart convention, HostType hostType, ClavusCategory category)
    {
        Convention = convention;
        HostType = hostType;
        Category = category;
        _dependencies = [];
    }

    /// <summary>
    ///     Adds a new dependency to the list
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public ClavusPartMetadata WithDependency(DependencyDirection direction, Type type)
    {
        _dependencies.Add(new(direction, type));
        return this;
    }

    /// <inheritdoc />
    public IClavusPart Convention { get; }

    /// <summary>
    ///     The dependencies
    /// </summary>
    public IEnumerable<IClavusDependency> Dependencies => _dependencies.OfType<IClavusDependency>();

    /// <inheritdoc />
    public HostType HostType { get; }

    public ClavusCategory Category { get; }
}
