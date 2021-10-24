using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting.Internals;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     A program initialization utility.
/// </summary>
public class TestHostBuilder : IHostBuilder, IConventionContext
{
    internal readonly IConventionContext _conventionContext;
    internal readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();

    internal readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions =
        new List<Action<HostBuilderContext, IConfigurationBuilder>>();

    internal readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions =
        new List<Action<HostBuilderContext, IServiceCollection>>();

    internal readonly List<IConfigureContainerAdapter> _configureContainerActions = new List<IConfigureContainerAdapter>();
    internal IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="conventionContext"></param>
    public TestHostBuilder(IConventionContext conventionContext)
    {
        _conventionContext = conventionContext;
        _conventionContext.Set<IHostBuilder>(this);
        _conventionContext.Set(this);
    }

    /// <summary>
    ///     Run the given actions to initialize the host.
    /// </summary>
    /// <returns>An initialized <see cref="IHost" /></returns>
    public IHost Build()
    {
        return TestHostFactory.Create(this);
    }

    /// <summary>
    ///     Run the given actions to initialize the service collection and it's container, and then resolve this type from the container.
    /// </summary>
    /// <returns>An initialized <see cref="IHost" /></returns>
    public T Parse<T>() where T : notnull
    {
        return TestHostFactory.CreateServiceProvider(this).GetRequiredService<T>();
    }

    /// <summary>
    ///     Run the given actions to initialize the service collection.
    /// </summary>
    /// <returns>An initialized <see cref="IHost" /></returns>
    public IServiceCollection Parse()
    {
        return TestHostFactory.CreateServiceCollection(this);
    }

    /// <summary>
    ///     Configure the host builder
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public TestHostBuilder Configure(Action<IHostBuilder> action)
    {
        action(this);
        return this;
    }

    #region Interfaces

    /// <inheritdoc />
    public object? this[object item]
    {
        get => _conventionContext[item];
        set => _conventionContext[item] = value;
    }

    /// <inheritdoc />
    public IServiceProviderDictionary Properties => _conventionContext.Properties;

    IDictionary<object, object?> IHostBuilder.Properties => _conventionContext.Properties;

    /// <inheritdoc />
    public ILogger Logger => _conventionContext.Logger;

    /// <inheritdoc />
    public IAssemblyProvider AssemblyProvider => _conventionContext.AssemblyProvider;

    /// <inheritdoc />
    public IAssemblyCandidateFinder AssemblyCandidateFinder => _conventionContext.AssemblyCandidateFinder;

    /// <inheritdoc />
    public IConventionProvider Conventions => _conventionContext.Conventions;

    /// <summary>
    ///     Set up the configuration for the builder itself. This will be used to initialize the <see cref="IHostEnvironment" />
    ///     for use later in the build process. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">
    ///     The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used
    ///     to construct the <see cref="IConfiguration" /> for the host.
    /// </param>
    /// <returns>The same instance of the <see cref="IHostBuilder" /> for chaining.</returns>
    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        _configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <summary>
    ///     Sets up the configuration for the remainder of the build process and application. This can be called multiple times and
    ///     the results will be additive. The results will be available at <see cref="HostBuilderContext.Configuration" /> for
    ///     subsequent operations, as well as in <see cref="IHost.Services" />.
    /// </summary>
    /// <param name="configureDelegate">
    ///     The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used
    ///     to construct the <see cref="IConfiguration" /> for the host.
    /// </param>
    /// <returns>The same instance of the <see cref="IHostBuilder" /> for chaining.</returns>
    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <summary>
    ///     Adds services to the container. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">
    ///     The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used
    ///     to construct the <see cref="IConfiguration" /> for the host.
    /// </param>
    /// <returns>The same instance of the <see cref="IHostBuilder" /> for chaining.</returns>
    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <summary>
    ///     Overrides the factory used to create the service provider.
    /// </summary>
    /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
    /// <param name="factory">A factory used for creating service providers.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder" /> for chaining.</returns>
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
    {
        _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException(nameof(factory)));
        return this;
    }

    /// <summary>
    ///     Overrides the factory used to create the service provider.
    /// </summary>
    /// <param name="factory">A factory used for creating service providers.</param>
    /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
    /// <returns>The same instance of the <see cref="IHostBuilder" /> for chaining.</returns>
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        where TContainerBuilder : notnull
    {
        _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException(nameof(factory)));
        return this;
    }

    /// <summary>
    ///     Enables configuring the instantiated dependency container. This can be called multiple times and
    ///     the results will be additive.
    /// </summary>
    /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
    /// <param name="configureDelegate">
    ///     The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used
    ///     to construct the <see cref="IConfiguration" /> for the host.
    /// </param>
    /// <returns>The same instance of the <see cref="IHostBuilder" /> for chaining.</returns>
    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureContainerActions.Add(
            new ConfigureContainerAdapter<TContainerBuilder>(
                configureDelegate
             ?? throw new ArgumentNullException(nameof(configureDelegate))
            )
        );
        return this;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to save</param>
    public TestHostBuilder Set<T>(T value)
    {
        Properties[typeof(T)] = value;
        return this;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public T Get<T>()
    {
        return (T)Properties[typeof(T)]!;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The key where the value is saved</param>
    public T Get<T>(string key)
    {
        return (T)Properties[key]!;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public TestHostBuilder Set<T>(string key, T value)
    {
        Properties[key] = value;
        return this;
    }

    #endregion
}
