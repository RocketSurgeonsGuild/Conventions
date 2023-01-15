using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Extensions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Base convention context that allows for stashing items in index keys
///     Implements the <see cref="IConventionContext" />
/// </summary>
/// <seealso cref="IConventionContext" />
public sealed class ConventionContext : IConventionContext
{
    private readonly ConventionContextBuilder _builder;
    private const string ConventionsSetup = "__ConventionsSetup__" + nameof(ConventionContext);

    /// <summary>
    ///     Create a context from a given builder
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IConventionContext From(ConventionContextBuilder builder)
    {
        builder._assemblyCandidateFinderFactory ??= ConventionContextHelpers.DefaultAssemblyCandidateFinderFactory;
        builder._assemblyProviderFactory ??= ConventionContextHelpers.DefaultAssemblyProviderFactory;

        var assemblyProvider = builder._assemblyProviderFactory(builder._source, builder.Get<ILogger>());
        var assemblyCandidateFinder = builder._assemblyCandidateFinderFactory(builder._source, builder.Get<ILogger>());
        var provider = ConventionContextHelpers.CreateProvider(builder, assemblyCandidateFinder, builder.Get<ILogger>());
        builder.Properties.Set(builder._serviceProviderFactory);
        var context = new ConventionContext(builder, provider, assemblyProvider, assemblyCandidateFinder, builder.Properties);

        if (context.Properties.ContainsKey(ConventionsSetup)) return context;

        context.ApplyConventions();
        context.Properties.Add(ConventionsSetup, true);
        return context;
    }

    /// <summary>
    ///     Creates a base context
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionProvider"></param>
    /// <param name="assemblyCandidateFinder"></param>
    /// <param name="properties"></param>
    /// <param name="assemblyProvider"></param>
    private ConventionContext(
        ConventionContextBuilder builder,
        IConventionProvider conventionProvider,
        IAssemblyProvider assemblyProvider,
        IAssemblyCandidateFinder assemblyCandidateFinder,
        IServiceProviderDictionary properties
    )
    {
        _builder = builder;
        Conventions = conventionProvider;
        AssemblyProvider = assemblyProvider;
        AssemblyCandidateFinder = assemblyCandidateFinder;
        Properties = properties;
    }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>System.Object.</returns>
    public object? this[object item]
    {
        get => Properties.TryGetValue(item, out var value) ? value : null;
        set => Properties[item] = value;
    }

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    public IConventionProvider Conventions { get; }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    public IServiceProviderDictionary Properties { get; }

    /// <summary>
    ///     A logger that is configured to work with each convention item
    /// </summary>
    /// <value>The logger.</value>
    public ILogger Logger => this.Get<ILogger>() ?? NullLogger.Instance;

    /// <summary>
    ///     Gets the assembly provider.
    /// </summary>
    /// <value>The assembly provider.</value>
    public IAssemblyProvider AssemblyProvider { get; }

    /// <summary>
    ///     Gets the assembly candidate finder.
    /// </summary>
    /// <value>The assembly candidate finder.</value>
    public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

    /// <summary>
    ///     Return the source builder for this context (to create new contexts if required).
    ///     Avoid doing this unless you absolutely need to.
    /// </summary>
    /// <returns></returns>
    public ConventionContextBuilder ToBuilder()
    {
        return _builder;
    }
}
