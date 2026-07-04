using Microsoft.Extensions.DependencyInjection;

namespace Clavus.Infrastructure;

/// <summary>
///     Container for conventions
/// </summary>
[PublicAPI]
public sealed class ClavusPartMetadata : IClavusPartMetadata
{
    private readonly List<IClavusDependency> _dependencies = [];

    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="convention"></param>
    /// <param name="hostType"></param>
    public ClavusPartMetadata(IClavusPart convention, HostType hostType)
    {
        Convention = convention;
        HostType = hostType;
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
    }

    /// <summary>
    ///     Adds a new dependency to the list
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public ClavusPartMetadata WithDependency(DependencyDirection direction, Type type)
    {
        _dependencies.Add(new ClavusDependency(direction, type));
        return this;
    }

    /// <inheritdoc />
    public IClavusPart Convention { get; }

    /// <summary>
    ///     The dependencies
    /// </summary>
    public IReadOnlyCollection<IClavusDependency> Dependencies
    {
        get => _dependencies;
        set
        {
            _dependencies.Clear();
            _dependencies.AddRange(value.Select(x => new ClavusDependency(x.Direction, x.Type)).OfType<IClavusDependency>());
        }
    }

    /// <inheritdoc />
    public HostType HostType { get; }

    /// <summary>
    ///    The category of the convention
    /// </summary>
    public ClavusCategory Category { get; }

    /// <summary>
    ///    The equality comparer for this type
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Convention.GetHashCode();
}

/// <summary>
///     Container for conventions
/// </summary>
/// <param name="properties"></param>
/// <param name="hostType"></param>
/// <param name="category"></param>
internal sealed class ClavusDeferredPartMetadata<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(IReadOnlyServiceProviderDictionary properties, HostType hostType, ClavusCategory category) : IClavusPartMetadata, IClavusDelegate where T : IClavusPart
{
    private readonly List<IClavusDependency> _dependencies = [];

    /// <summary>
    ///     Adds a new dependency to the list
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public ClavusDeferredPartMetadata<T> WithDependency(DependencyDirection direction, Type type)
    {
        _dependencies.Add(new ClavusDependency(direction, type));
        return this;
    }

    /// <inheritdoc />
    public IClavusPart Convention => field ??= ActivatorUtilities.CreateInstance<T>(properties);

    /// <summary>
    ///     The dependencies
    /// </summary>
    public IReadOnlyCollection<IClavusDependency> Dependencies
    {
        get => _dependencies;
        set
        {
            _dependencies.Clear();
            _dependencies.AddRange(value.Select(x => new ClavusDependency(x.Direction, x.Type)).OfType<IClavusDependency>());
        }
    }

    /// <inheritdoc />
    public HostType HostType { get; } = hostType;

    /// <summary>
    ///    The category of the convention
    /// </summary>
    public ClavusCategory Category { get; } = category;
}
