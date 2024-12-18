using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Extensions;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Base convention context that allows for stashing items in index keys
///     Implements the <see cref="IConventionContext" />
/// </summary>
/// <seealso cref="IConventionContext" />
public sealed class ConventionContext : IConventionContext
{
    /// <summary>
    ///     Create a context from a given builder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IConventionContext> FromAsync(ConventionContextBuilder builder, CancellationToken cancellationToken = default)
    {
        var context = FromInitInternal(builder, Assembly.GetCallingAssembly());
        if (context.Properties.ContainsKey(ConventionsSetup)) return context;

        await context.ApplyConventionsAsync(cancellationToken);
        context.Properties.Add(ConventionsSetup, true);
        return context;
    }

    private const string ConventionsSetup = "__ConventionsSetup__" + nameof(ConventionContext);

    private static ConventionContext FromInitInternal(ConventionContextBuilder builder, Assembly callerAssembly)
    {
        builder.AddIfMissing(AssemblyLoadContext.Default);
        builder._conventionProviderFactory ??= ImportHelpers.CallerConventions(callerAssembly);
        if (builder._conventionProviderFactory is null) throw new InvalidOperationException("No convention provider factory was found");

        // ReSharper disable once NullableWarningSuppressionIsUsed
        var assemblyProvider = builder._conventionProviderFactory!.CreateTypeProvider(builder);
        var provider = ConventionContextHelpers.CreateProvider(builder, builder.Get<ILogger>());
        // ReSharper disable once NullableWarningSuppressionIsUsed
        builder.Properties.Set(builder._serviceProviderFactory!);
        return new(builder, provider, assemblyProvider);
    }

    private static readonly IConfiguration _emptyConfiguration = new ConfigurationBuilder().Build();

    private readonly ConventionContextBuilder _builder;

    /// <summary>
    ///     Creates a base context
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionProvider"></param>
    /// <param name="typeProvider"></param>
    private ConventionContext(
        ConventionContextBuilder builder,
        IConventionProvider conventionProvider,
        ICompiledTypeProvider typeProvider
    )
    {
        _builder = builder;
        Conventions = conventionProvider;
        TypeProvider = typeProvider;
        Properties = builder.Properties;
        Categories = builder.Categories.ToImmutableHashSet(ConventionCategory.ValueComparer);
    }

    /// <summary>
    ///     The host type
    /// </summary>
    public HostType HostType => this.GetHostType();

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    public ImmutableHashSet<ConventionCategory> Categories { get; set; }

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>System.Object.</returns>
    public object this[object item]
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        #pragma warning disable CS8603 // Possible null reference return.
        get => Properties.TryGetValue(item, out var value) ? value : null!;
        #pragma warning restore CS8603 // Possible null reference return.
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
    ///     Gets the type provider.
    /// </summary>
    /// <value>The type provider.</value>
    public ICompiledTypeProvider TypeProvider { get; }

    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    public IConfiguration Configuration => this.Get<IConfiguration>() ?? _emptyConfiguration;

    /// <summary>
    ///     Return the source builder for this context (to create new contexts if required).
    ///     Avoid doing this unless you absolutely need to.
    /// </summary>
    /// <returns></returns>
    public ConventionContextBuilder ToBuilder() => _builder;
}
