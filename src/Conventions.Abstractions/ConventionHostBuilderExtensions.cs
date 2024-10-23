using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Hosting;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Conventions.Setup;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Base convention extensions
/// </summary>
[PublicAPI]
public static class ConventionHostBuilderExtensions
{
    /// <summary>
    ///     Setup a convention to run as soon as the context is created
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder SetupConvention(
        this ConventionContextBuilder container,
        SetupConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder SetupConvention(
        this ConventionContextBuilder container,
        SetupAsyncConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
    public static ConventionContextBuilder UseServiceProviderFactory<TContainerBuilder>(
        this ConventionContextBuilder builder,
        IServiceProviderFactory<TContainerBuilder> serviceProviderFactory
    ) where TContainerBuilder : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder._serviceProviderFactory =
            (_, _, _) => ValueTask.FromResult<IServiceProviderFactory<object>>(new ServiceProviderWrapper<TContainerBuilder>(serviceProviderFactory));
        return builder;
    }

    /// <summary>
    ///     Set the service provider factory to be used for hosting or other systems.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceProviderFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ConventionContextBuilder UseServiceProviderFactory<TContainerBuilder>(
        this ConventionContextBuilder builder,
        Func<IConventionContext, IServiceCollection, CancellationToken, ValueTask<IServiceProviderFactory<TContainerBuilder>>> serviceProviderFactory
    ) where TContainerBuilder : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder._serviceProviderFactory = async (context, collection, cancellationToken) =>
                                              new ServiceProviderWrapper<TContainerBuilder>(
                                                  await serviceProviderFactory(context, collection, cancellationToken)
                                              );
        return builder;
    }

    /// <summary>
    ///     Set the service provider factory to be used for hosting or other systems.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceProviderFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ConventionContextBuilder UseServiceProviderFactory<TContainerBuilder>(
        this ConventionContextBuilder builder,
        Func<IConventionContext, IServiceCollection, ValueTask<IServiceProviderFactory<TContainerBuilder>>> serviceProviderFactory
    ) where TContainerBuilder : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder._serviceProviderFactory = async (context, collection, _) =>
                                              new ServiceProviderWrapper<TContainerBuilder>(await serviceProviderFactory(context, collection));
        return builder;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        ServiceConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        ServiceAsyncConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Action<IConfiguration, IServiceCollection> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceConvention((_, configuration, services) => @delegate(configuration, services)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Func<IConfiguration, IServiceCollection, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        container.AppendDelegate(
            new ServiceAsyncConvention((_, configuration, services, cancellationToken) => @delegate(configuration, services, cancellationToken)),
            priority,
            category
        );
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Func<IConfiguration, IServiceCollection, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceAsyncConvention((_, configuration, services, _) => @delegate(configuration, services)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Action<IServiceCollection> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceConvention((_, _, services) => @delegate(services)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Func<IServiceCollection, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceAsyncConvention((_, _, services, _) => @delegate(services)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Func<IServiceCollection, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ServiceAsyncConvention((_, _, services, cancellationToken) => @delegate(services, cancellationToken)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        LoggingConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        LoggingAsyncConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Action<IConfiguration, ILoggingBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingConvention((_, configuration, builder) => @delegate(configuration, builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Func<IConfiguration, ILoggingBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new LoggingAsyncConvention((_, configuration, builder, cancellationToken) => @delegate(configuration, builder, cancellationToken)),
            priority,
            category
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Func<IConfiguration, ILoggingBuilder, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingAsyncConvention((_, configuration, builder, _) => @delegate(configuration, builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Action<ILoggingBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingConvention((_, _, builder) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Func<ILoggingBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingAsyncConvention((_, _, builder, cancellationToken) => @delegate(builder, cancellationToken)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Func<ILoggingBuilder, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new LoggingAsyncConvention((_, _, builder, _) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        ConfigurationConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        ConfigurationAsyncConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Action<IConfiguration, IConfigurationBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationConvention((_, configuration, builder) => @delegate(configuration, builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Func<IConfiguration, IConfigurationBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new ConfigurationAsyncConvention((_, configuration, builder, cancellationToken) => @delegate(configuration, builder, cancellationToken)),
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Func<IConfiguration, IConfigurationBuilder, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationAsyncConvention((_, configuration, builder, _) => @delegate(configuration, builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Action<IConfigurationBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationConvention((_, _, builder) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Func<IConfigurationBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new ConfigurationAsyncConvention((_, _, builder, cancellationToken) => @delegate(builder, cancellationToken)),
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Func<IConfigurationBuilder, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new ConfigurationAsyncConvention((_, _, builder, _) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the host created event for the given host type
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder OnHostCreated<THost>(
        this ConventionContextBuilder container,
        HostCreatedConvention<THost> @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder OnHostCreated<THost>(
        this ConventionContextBuilder container,
        HostCreatedAsyncConvention<THost> @delegate,
        int priority = 0,
        ConventionCategory? category = null
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder OnHostCreated<THost>(
        this ConventionContextBuilder container,
        Action<THost> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostCreatedConvention<THost>((_, host) => @delegate(host)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="category">The category.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder OnHostCreated<THost>(
        this ConventionContextBuilder container,
        Func<THost, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(
            new HostCreatedAsyncConvention<THost>((_, host, cancellationToken) => @delegate(host, cancellationToken)),
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder OnHostCreated<THost>(
        this ConventionContextBuilder container,
        Func<THost, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        container.AppendDelegate(new HostCreatedAsyncConvention<THost>((_, host, _) => @delegate(host)), priority, category);
        return container;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this ConventionContextBuilder context)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);

        return (T?)context.Properties[typeof(T)];
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this ConventionContextBuilder context, string key)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);

        return (T?)context.Properties[key];
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder">The builder</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    public static T GetOrAdd<T>(this ConventionContextBuilder builder, Func<T> factory)
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
    public static T GetOrAdd<T>(this ConventionContextBuilder builder, string key, Func<T> factory)
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
    public static ConventionContextBuilder Set<T>(this ConventionContextBuilder context, T value)
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
    public static ConventionContextBuilder Set(this ConventionContextBuilder context, Type key, object value)
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
    public static ConventionContextBuilder Set<T>(this ConventionContextBuilder context, string key, T value)
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
    public static ConventionContextBuilder AddIfMissing<T>(this ConventionContextBuilder builder, T value) where T : notnull
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
    public static ConventionContextBuilder AddIfMissing(this ConventionContextBuilder builder, Type key, object value)
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
    public static ConventionContextBuilder AddIfMissing<T>(this ConventionContextBuilder builder, string key, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Properties.AddIfMissing(key, value);
        return builder;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static bool IsUnitTestHost(this ConventionContextBuilder context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.GetHostType() == HostType.UnitTest;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static HostType GetHostType(this ConventionContextBuilder context)
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
        public object CreateBuilder(IServiceCollection services)
        {
            return serviceProviderFactoryImplementation.CreateBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(object containerBuilder)
        {
            return serviceProviderFactoryImplementation.CreateServiceProvider((TContainerBuilder)containerBuilder);
        }
    }
}
