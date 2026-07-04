using Clavus.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace Clavus;

/// <summary>
///     Base convention extensions
/// </summary>
[PublicAPI]
public static class ClavusHostBuilderExtensions
{
    /// <summary>
    ///     Set the service provider factory to be used for hosting or other systems.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceProviderFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ClavusContextBuilder UseServiceProviderFactory<TContainerBuilder>(
        this ClavusContextBuilder builder,
        IServiceProviderFactory<TContainerBuilder> serviceProviderFactory
    ) where TContainerBuilder : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Set<ServiceProviderFactoryAdapter>((_, _, _) => ValueTask.FromResult<IServiceProviderFactory<object>>(new ServiceProviderWrapper<TContainerBuilder>(serviceProviderFactory)));
        return builder;
    }

    /// <summary>
    ///     Set the service provider factory to be used for hosting or other systems.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceProviderFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ClavusContextBuilder UseServiceProviderFactory<TContainerBuilder>(
        this ClavusContextBuilder builder,
        Func<IClavusContext, IServiceCollection, CancellationToken, ValueTask<IServiceProviderFactory<TContainerBuilder>>> serviceProviderFactory
    ) where TContainerBuilder : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Set<ServiceProviderFactoryAdapter>(async (context, collection, cancellationToken) => new ServiceProviderWrapper<TContainerBuilder>(await serviceProviderFactory(context, collection, cancellationToken).ConfigureAwait(false)));
        return builder;
    }

    /// <summary>
    ///     Set the service provider factory to be used for hosting or other systems.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceProviderFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ClavusContextBuilder UseServiceProviderFactory<TContainerBuilder>(
        this ClavusContextBuilder builder,
        Func<IClavusContext, IServiceCollection, ValueTask<IServiceProviderFactory<TContainerBuilder>>> serviceProviderFactory
    ) where TContainerBuilder : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Set<ServiceProviderFactoryAdapter>(async (context, collection, _) => new ServiceProviderWrapper<TContainerBuilder>(await serviceProviderFactory(context, collection).ConfigureAwait(false)));
        return builder;
    }
    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this ClavusContextBuilder context)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);

        return (T?)context.Properties[typeof(T)];
    }

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <returns>T.</returns>
    public static T Require<T>(this ClavusContextBuilder context)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Properties.TryGetValue(typeof(T), out var value) && value is T t
            ? t
            : throw new KeyNotFoundException($"The value of type {typeof(T).Name} was not found in the context");
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this ClavusContextBuilder context, string key)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);

        return (T?)context.Properties[key];
    }

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T Require<T>(this ClavusContextBuilder context, string key)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Properties.TryGetValue(key, out var value) && value is T t
            ? t
            : throw new KeyNotFoundException($"The value of type {typeof(T).Name} with the {key} was not found in the context");
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder">The builder</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    public static T GetOrAdd<T>(this ClavusContextBuilder builder, Func<T> factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        ArgumentNullException.ThrowIfNull(factory);

        if (builder.Properties[typeof(T)] is T value) return value;

        value = factory();
        builder.Set(value);

        return value;
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder">The builder</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    public static T GetOrAdd<T>(this ClavusContextBuilder builder, string key, Func<T> factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        ArgumentNullException.ThrowIfNull(factory);

        if (builder.Properties[key] is not T value)
        {
            value = factory();
            builder.Set(value);
        }

        return value;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="value">The value to save</param>
    public static ClavusContextBuilder Set<T>(this ClavusContextBuilder context, T value)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Properties[typeof(T)] = value;
        return context;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static ClavusContextBuilder Set(this ClavusContextBuilder context, Type key, object value)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Properties[key] = value;
        return context;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static ClavusContextBuilder Set<T>(this ClavusContextBuilder context, string key, T value)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Properties[key] = value;
        return context;
    }

    /// <summary>
    ///     Set key to the value if the type is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="builder">The builder</param>
    /// <param name="value">The value to save</param>
    public static ClavusContextBuilder AddIfMissing<T>(this ClavusContextBuilder builder, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Properties.AddIfMissing(value);
        return builder;
    }

    /// <summary>
    ///     Set key to the value if the key is missing
    /// </summary>
    /// <param name="builder">The builder</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static ClavusContextBuilder AddIfMissing(this ClavusContextBuilder builder, Type key, object value)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Properties.AddIfMissing(key, value);
        return builder;
    }

    /// <summary>
    ///     Set key to the value if the key is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="builder">The builder</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static ClavusContextBuilder AddIfMissing<T>(this ClavusContextBuilder builder, string key, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Properties.AddIfMissing(key, value);
        return builder;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static bool IsUnitTestHost(this ClavusContextBuilder context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.GetHostType() == HostType.UnitTest;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static HostType GetHostType(this ClavusContextBuilder context)
    {
        return context.Properties.TryGetValue(typeof(HostType), out var hostType)
         && ( hostType is HostType ht || ( hostType is string str && Enum.TryParse(str, true, out ht) ) )
                ? ht
                : HostType.Undefined;
    }

    private class ServiceProviderWrapper<TContainerBuilder>
        (IServiceProviderFactory<TContainerBuilder> serviceProviderFactoryImplementation) : IServiceProviderFactory<object>
        where TContainerBuilder : notnull
    {
        public object CreateBuilder(IServiceCollection services) => serviceProviderFactoryImplementation.CreateBuilder(services);

        public IServiceProvider CreateServiceProvider(object containerBuilder) => serviceProviderFactoryImplementation.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}
