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

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    public ImmutableHashSet<ConventionCategory> Categories { get; set; }

    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    public IConfiguration Configuration => this.Get<IConfiguration>() ?? _emptyConfiguration;

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    public IConventionProvider Conventions { get; }

    /// <summary>
    ///     The host type
    /// </summary>
    public HostType HostType => this.GetHostType();

    /// <summary>
    ///     A logger that is configured to work with each convention item
    /// </summary>
    /// <value>The logger.</value>
    public ILogger Logger => this.GetOrAdd<ILogger>(() => NullLogger.Instance);

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    public IServiceProviderDictionary Properties { get; }

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

    private static ConventionProvider CreateProvider(ConventionContextBuilder builder, LoadConventions loadConventions)
    {
        var conventions = builder.state.GetConventions();
        for (var i = 0; i < conventions.Count; i++)
        {
            if (conventions[i] is Type type) conventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
        }

        conventions.InsertRange(
            conventions.FindIndex(z => z is null),
            builder.state.CalculateConventions(builder, loadConventions)
        );

        return new(builder.GetHostType(), builder.Categories.ToImmutableHashSet(ConventionCategory.ValueComparer), conventions);
    }

    private static ConventionContext FromInitInternal(ConventionContextBuilder builder)
    {
        var conventions = builder.Require<LoadConventions>();
        builder
           .AddIfMissing(AssemblyLoadContext.Default)
           .AddIfMissing("ExecutingAssembly", conventions.Method.Module.Assembly);
        var provider = CreateProvider(builder, conventions);
        // ReSharper disable once NullableWarningSuppressionIsUsed
        if (builder.state.ServiceProviderFactory is { })
            builder.Properties.Set(builder.state.ServiceProviderFactory);
        return new(builder, provider);
    }

    private static readonly IConfiguration _emptyConfiguration = new ConfigurationBuilder().Build();
}
