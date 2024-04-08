using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
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
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder SetupConvention(this ConventionContextBuilder container, SetupConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Setup a convention to run as soon as the context is created
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder SetupConvention(this ConventionContextBuilder container, SetupAsyncConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
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
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        #if NET6_0_OR_GREATER
        builder._serviceProviderFactory =
            (_, _, _) => ValueTask.FromResult<IServiceProviderFactory<object>>(new ServiceProviderWrapper<TContainerBuilder>(serviceProviderFactory));
        #else
        builder._serviceProviderFactory = async (_, _, _) =>
                                          {
                                              await Task.Yield();
                                              return new ServiceProviderWrapper<TContainerBuilder>(serviceProviderFactory);
                                          };
        #endif
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
        if (builder == null) throw new ArgumentNullException(nameof(builder));

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
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder._serviceProviderFactory = async (context, collection, _) =>
                                              new ServiceProviderWrapper<TContainerBuilder>(await serviceProviderFactory(context, collection));
        return builder;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(this ConventionContextBuilder container, ServiceConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(this ConventionContextBuilder container, ServiceAsyncConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(this ConventionContextBuilder container, Action<IConfiguration, IServiceCollection> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ServiceConvention((_, configuration, services) => @delegate(configuration, services)));
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Func<IConfiguration, IServiceCollection, CancellationToken, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(
            new ServiceAsyncConvention((_, configuration, services, cancellationToken) => @delegate(configuration, services, cancellationToken))
        );
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Func<IConfiguration, IServiceCollection, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ServiceAsyncConvention((_, configuration, services, _) => @delegate(configuration, services)));
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(this ConventionContextBuilder container, Action<IServiceCollection> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ServiceConvention((_, _, services) => @delegate(services)));
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(this ConventionContextBuilder container, Func<IServiceCollection, ValueTask> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ServiceAsyncConvention((_, _, services, _) => @delegate(services)));
        return container;
    }

    /// <summary>
    ///     Configure the services delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureServices(
        this ConventionContextBuilder container,
        Func<IServiceCollection, CancellationToken, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ServiceAsyncConvention((_, _, services, cancellationToken) => @delegate(services, cancellationToken)));
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(this ConventionContextBuilder container, LoggingConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(this ConventionContextBuilder container, LoggingAsyncConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(this ConventionContextBuilder container, Action<IConfiguration, ILoggingBuilder> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new LoggingConvention((_, configuration, builder) => @delegate(configuration, builder)));
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Func<IConfiguration, ILoggingBuilder, CancellationToken, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(
            new LoggingAsyncConvention((_, configuration, builder, cancellationToken) => @delegate(configuration, builder, cancellationToken))
        );
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(this ConventionContextBuilder container, Func<IConfiguration, ILoggingBuilder, ValueTask> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new LoggingAsyncConvention((_, configuration, builder, _) => @delegate(configuration, builder)));
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(this ConventionContextBuilder container, Action<ILoggingBuilder> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new LoggingConvention((_, _, builder) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(
        this ConventionContextBuilder container,
        Func<ILoggingBuilder, CancellationToken, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new LoggingAsyncConvention((_, _, builder, cancellationToken) => @delegate(builder, cancellationToken)));
        return container;
    }

    /// <summary>
    ///     Configure the logging delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureLogging(this ConventionContextBuilder container, Func<ILoggingBuilder, ValueTask> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new LoggingAsyncConvention((_, _, builder, _) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(this ConventionContextBuilder container, ConfigurationConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(this ConventionContextBuilder container, ConfigurationAsyncConvention @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Action<IConfiguration, IConfigurationBuilder> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ConfigurationConvention((_, configuration, builder) => @delegate(configuration, builder)));
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Func<IConfiguration, IConfigurationBuilder, CancellationToken, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(
            new ConfigurationAsyncConvention((_, configuration, builder, cancellationToken) => @delegate(configuration, builder, cancellationToken))
        );
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Func<IConfiguration, IConfigurationBuilder, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ConfigurationAsyncConvention((_, configuration, builder, _) => @delegate(configuration, builder)));
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(this ConventionContextBuilder container, Action<IConfigurationBuilder> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ConfigurationConvention((_, _, builder) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(
        this ConventionContextBuilder container,
        Func<IConfigurationBuilder, CancellationToken, ValueTask> @delegate
    )
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ConfigurationAsyncConvention((_, _, builder, cancellationToken) => @delegate(builder, cancellationToken)));
        return container;
    }

    /// <summary>
    ///     Configure the configuration delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns><see cref="ConventionContextBuilder" />.</returns>
    public static ConventionContextBuilder ConfigureConfiguration(this ConventionContextBuilder container, Func<IConfigurationBuilder, ValueTask> @delegate)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        container.AppendDelegate(new ConfigurationAsyncConvention((_, _, builder, _) => @delegate(builder)));
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
        if (context == null) throw new ArgumentNullException(nameof(context));

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
        if (context == null) throw new ArgumentNullException(nameof(context));

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
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (factory == null) throw new ArgumentNullException(nameof(factory));

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
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (factory == null) throw new ArgumentNullException(nameof(factory));

        if (!( builder.Properties[key] is T value ))
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
        if (context == null) throw new ArgumentNullException(nameof(context));

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
        if (context == null) throw new ArgumentNullException(nameof(context));

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
        if (context == null) throw new ArgumentNullException(nameof(context));

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
        if (builder == null) throw new ArgumentNullException(nameof(builder));
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
        if (builder == null) throw new ArgumentNullException(nameof(builder));
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
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        builder.Properties.AddIfMissing(key, value);
        return builder;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static bool IsUnitTestHost(this ConventionContextBuilder context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        return context.GetHostType() == HostType.UnitTest;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static HostType GetHostType(this ConventionContextBuilder context)
    {
        return context.Properties.TryGetValue(typeof(HostType), out var hostType)
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            ? (HostType)hostType!
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