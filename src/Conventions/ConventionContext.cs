using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Extensions;

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
        var context = FromInitInternal(builder);
        if (context.Properties.ContainsKey(ConventionsSetup)) return context;

        await context.ApplyConventionsAsync(cancellationToken);
        context.Properties.Add(ConventionsSetup, true);
        return context;
    }

    private const string ConventionsSetup = "__ConventionsSetup__" + nameof(ConventionContext);

    private static ConventionContext FromInitInternal(ConventionContextBuilder builder)
    {
        builder.AddIfMissing(AssemblyLoadContext.Default);
        if (builder._conventionProviderFactory is null)
        {
            throw new NotSupportedException("The convention provider factory must be set on the builder");
        }

        // ReSharper disable once NullableWarningSuppressionIsUsed
        var assemblyProvider = builder._conventionProviderFactory!.CreateAssemblyProvider(builder);
        var provider = ConventionContextHelpers.CreateProvider(builder, assemblyProvider, builder.Get<ILogger>());
        // ReSharper disable once NullableWarningSuppressionIsUsed
        builder.Properties.Set(builder._serviceProviderFactory!);
        return new(builder, provider, assemblyProvider, builder.Properties);
    }

    private readonly ConventionContextBuilder _builder;

    /// <summary>
    ///     Creates a base context
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionProvider"></param>
    /// <param name="properties"></param>
    /// <param name="assemblyProvider"></param>
    private ConventionContext(
        ConventionContextBuilder builder,
        IConventionProvider conventionProvider,
        IAssemblyProvider assemblyProvider,
        IServiceProviderDictionary properties
    )
    {
        _builder = builder;
        Conventions = conventionProvider;
        AssemblyProvider = assemblyProvider;
        Properties = properties;
    }

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
    ///     Gets the assembly provider.
    /// </summary>
    /// <value>The assembly provider.</value>
    public IAssemblyProvider AssemblyProvider { get; }

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