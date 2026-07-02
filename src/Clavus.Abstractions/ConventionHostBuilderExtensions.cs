using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Clavus.Configuration;
using Rocket.Surgery.Clavus.DependencyInjection;
using Rocket.Surgery.Clavus.Hosting;
using Rocket.Surgery.Clavus.Logging;
using Rocket.Surgery.Clavus.Setup;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace Rocket.Surgery.Clavus;

/// <summary>
///     Base convention extensions
/// </summary>
[PublicAPI]
public static class ClavusHostBuilderExtensions
{
    /// <summary>
    ///     Setup a convention to run as soon as the context is created
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder SetupPart(
        this ClavusContextBuilder container,
        SetupPart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Setup a convention to run as soon as the context is created
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder SetupPart(
        this ClavusContextBuilder container,
        SetupAsyncPart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
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
        IServiceProviderFactory<TContainerBuilder> serviceProviderFactory
    ) where TContainerBuilder : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.state.ServiceProviderFactory = (_, _, _) => ValueTask.FromResult<IServiceProviderFactory<object>>(new ServiceProviderWrapper<TContainerBuilder>(serviceProviderFactory));
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

        builder.state.ServiceProviderFactory = async (context, collection, cancellationToken) => new ServiceProviderWrapper<TContainerBuilder>(await serviceProviderFactory(context, collection, cancellationToken).ConfigureAwait(false));
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

        builder.state.ServiceProviderFactory = async (context, collection, _) => new ServiceProviderWrapper<TContainerBuilder>(await serviceProviderFactory(context, collection).ConfigureAwait(false));
        return builder;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        ServicePart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        ServiceAsyncPart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        Action<IConfiguration, IServiceCollection> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServicePart((_, configuration, services) => @delegate(configuration, services)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        Func<IConfiguration, IServiceCollection, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(
            new ServiceAsyncPart((_, configuration, services, cancellationToken) => @delegate(configuration, services, cancellationToken)),
            priority,
            category ?? ClavusCategory.Core
        );
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        Func<IConfiguration, IServiceCollection, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceAsyncPart((_, configuration, services, _) => @delegate(configuration, services)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        Action<IServiceCollection> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServicePart((_, _, services) => @delegate(services)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        Func<IServiceCollection, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceAsyncPart((_, _, services, _) => @delegate(services)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureServices(
        this ClavusContextBuilder container,
        Func<IServiceCollection, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceAsyncPart((_, _, services, cancellationToken) => @delegate(services, cancellationToken)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        LoggingPart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        LoggingAsyncPart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        Action<IConfiguration, ILoggingBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingPart((_, configuration, builder) => @delegate(configuration, builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        Func<IConfiguration, ILoggingBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new LoggingAsyncPart((_, configuration, builder, cancellationToken) => @delegate(configuration, builder, cancellationToken)),
            priority,
            category ?? ClavusCategory.Core
        );
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        Func<IConfiguration, ILoggingBuilder, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingAsyncPart((_, configuration, builder, _) => @delegate(configuration, builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        Action<ILoggingBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingPart((_, _, builder) => @delegate(builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        Func<ILoggingBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingAsyncPart((_, _, builder, cancellationToken) => @delegate(builder, cancellationToken)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureLogging(
        this ClavusContextBuilder container,
        Func<ILoggingBuilder, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingAsyncPart((_, _, builder, _) => @delegate(builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        ConfigurationPart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        ConfigurationAsyncPart @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        Action<IConfiguration, IConfigurationBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationPart((_, configuration, builder) => @delegate(configuration, builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        Func<IConfiguration, IConfigurationBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new ConfigurationAsyncPart((_, configuration, builder, cancellationToken) => @delegate(configuration, builder, cancellationToken)),
            priority,
            category ?? ClavusCategory.Core
        );
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        Func<IConfiguration, IConfigurationBuilder, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationAsyncPart((_, configuration, builder, _) => @delegate(configuration, builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        Action<IConfigurationBuilder> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationPart((_, _, builder) => @delegate(builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        Func<IConfigurationBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new ConfigurationAsyncPart((_, _, builder, cancellationToken) => @delegate(builder, cancellationToken)),
            priority,
            category ?? ClavusCategory.Core
        );
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder ConfigureConfiguration(
        this ClavusContextBuilder container,
        Func<IConfigurationBuilder, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationAsyncPart((_, _, builder, _) => @delegate(builder)), priority, category ?? ClavusCategory.Core);
        return container;
    }

    /// <summary>
    ///     Configure the host created event for the given host type
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder OnHostCreated<THost>(
        this ClavusContextBuilder container,
        HostCreatedPart<THost> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder OnHostCreated<THost>(
        this ClavusContextBuilder container,
        HostCreatedAsyncPart<THost> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder OnHostCreated<THost>(
        this ClavusContextBuilder container,
        Action<THost> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostCreatedPart<THost>((_, host) => @delegate(host)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder OnHostCreated<THost>(
        this ClavusContextBuilder container,
        Func<THost, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new HostCreatedAsyncPart<THost>((_, host, cancellationToken) => @delegate(host, cancellationToken)),
            priority,
            category
        );
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ClavusContextBuilder" />.</returns>
    public static ClavusContextBuilder OnHostCreated<THost>(
        this ClavusContextBuilder container,
        Func<THost, ValueTask> @delegate,
        int priority = 0,
        ClavusCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostCreatedAsyncPart<THost>((_, host, _) => @delegate(host)), priority, category);
        return container;
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
