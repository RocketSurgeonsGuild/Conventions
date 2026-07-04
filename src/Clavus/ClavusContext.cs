using System.Collections.Immutable;
using Clavus.Infrastructure;

namespace Clavus;

/// <summary>
///     Base convention context that allows for stashing items in index keys
///     Implements the <see cref="IClavusContext" />
/// </summary>
/// <seealso cref="IClavusContext" />
public sealed class ClavusContext : IClavusContext
{
    /// <summary>
    ///     Create a context from a given builder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IClavusContext> FromAsync(ClavusContextBuilder builder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var context = FromInitInternal(builder);
        await context.ApplySetup(cancellationToken).ConfigureAwait(false);
        return context;
    }

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    public ImmutableHashSet<ClavusCategory> Categories { get; set; }

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    public ImmutableList<IClavusPart> Parts { get; }

    /// <summary>
    ///     The host type
    /// </summary>
    public HostType HostType => this.GetHostType();

    /// <summary>
    ///     A central location for sharing state between components during the convention building process.
    /// </summary>
    /// <value>The properties.</value>
    public IServiceProviderDictionary Properties { get; }

    /// <summary>
    ///     Creates a base context
    /// </summary>
    /// <param name="builder"></param>
    private ClavusContext(ClavusContextBuilder builder)
    {
        Parts = ClavusResolver.Resolve(builder.GetHostType(), builder.Categories.ToImmutableHashSet(ClavusCategory.ValueComparer), builder.Ashlar());
        Properties = builder.Properties;
        Categories = builder.Categories.ToImmutableHashSet(ClavusCategory.ValueComparer);
    }

    private static ClavusContext FromInitInternal(ClavusContextBuilder builder)
    {
        builder.AddIfMissing(ConventionExceptionPolicy.IgnoreNotSupported);
        return new(builder);
    }
}
