using System.Collections.Immutable;
using System.Runtime.Loader;

using Microsoft.Extensions.DependencyInjection;

using Clavus.Extensions;
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
        await context.ApplyPartsAsync(cancellationToken).ConfigureAwait(false);
        return context;
    }

    /// <summary>
    ///     The categories of the convention context
    /// </summary>
    public ImmutableHashSet<ClavusCategory> Categories { get; set; }

    /// <summary>
    ///     Get the conventions from the context
    /// </summary>
    public IClavusProvider Conventions { get; }

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
    /// <param name="conventionProvider"></param>
    private ClavusContext(
        ClavusContextBuilder builder,
        IClavusProvider conventionProvider
    )
    {
        Conventions = conventionProvider;
        Properties = builder.Properties;
        Categories = builder.Categories.ToImmutableHashSet(ClavusCategory.ValueComparer);
    }

    private static ClavusProvider CreateProvider(ClavusContextBuilder builder, LoadClavusParts loadConventions)
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

        return new(builder.GetHostType(), builder.Categories.ToImmutableHashSet(ClavusCategory.ValueComparer), conventions);
    }

    private static ClavusContext FromInitInternal(ClavusContextBuilder builder)
    {
        var conventions = builder.Require<LoadClavusParts>();
        builder
           .AddIfMissing(AssemblyLoadContext.Default)
           .AddIfMissing("ExecutingAssembly", conventions.Method.Module.Assembly)
           .AddIfMissing(ConventionExceptionPolicy.IgnoreNotSupported);
        var provider = CreateProvider(builder, conventions);
        // ReSharper disable once NullableWarningSuppressionIsUsed
        if (builder.state.ServiceProviderFactory is { })
            builder.Properties.Set(builder.state.ServiceProviderFactory);
        return new(builder, provider);
    }
}
