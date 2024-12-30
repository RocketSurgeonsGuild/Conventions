using System.Collections.Immutable;
using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        ArgumentNullException.ThrowIfNull(builder);
        var context = FromInitInternal(builder);
        await context.ApplyConventionsAsync(cancellationToken).ConfigureAwait(false);
        return context;
    }

    private static ConventionContext FromInitInternal(ConventionContextBuilder builder)
    {
        builder.AddIfMissing(AssemblyLoadContext.Default);
        var provider = CreateProvider(builder);
        // ReSharper disable once NullableWarningSuppressionIsUsed
        builder.Properties.Set(builder.state.ServiceProviderFactory);
        return new(builder, provider);
    }

    /// <summary>
    ///     Method used to create a convention provider
    /// </summary>
    /// <returns></returns>
    static ConventionProvider CreateProvider(ConventionContextBuilder builder)
    {
        var conventions = builder.state.GetConventions();
        for (var i = 0; i < conventions.Count; i++)
        {
            if (conventions[i] is Type type) conventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
        }

        conventions.InsertRange(
            conventions.FindIndex(z => z is null),
            builder.state.CalculateConventions(builder, builder.Require<LoadConventions>())
        );

        return new(builder.GetHostType(), builder.Categories.ToImmutableHashSet(ConventionCategory.ValueComparer), conventions);
    }

    private static readonly IConfiguration _emptyConfiguration = new ConfigurationBuilder().Build();

    /// <summary>
    ///     Creates a base context
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="conventionProvider"></param>
    private ConventionContext(
        ConventionContextBuilder builder,
        IConventionProvider conventionProvider
    )
    {
        Conventions = conventionProvider;
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
    public ILogger Logger => this.GetOrAdd<ILogger>(() => NullLogger.Instance);

    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    public IConfiguration Configuration => this.Get<IConfiguration>() ?? _emptyConfiguration;
}
