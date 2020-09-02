using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Internals;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A program initialization utility.
    /// </summary>
    public class TestHostBuilder : IHostBuilder, IConventionHostBuilder
    {
        internal readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();

        internal readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions =
            new List<Action<HostBuilderContext, IConfigurationBuilder>>();

        internal readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions =
            new List<Action<HostBuilderContext, IServiceCollection>>();

        internal readonly List<IConfigureContainerAdapter> _configureContainerActions = new List<IConfigureContainerAdapter>();
        internal IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

        public TestHostBuilder(IDictionary<object, object?> properties)
        {
            Properties = properties;
        }

        /// <summary>
        /// Run the given actions to initialize the host.
        /// </summary>
        /// <returns>An initialized <see cref="IHost"/></returns>
        public IHost Build() => TestHostFactory.Create(this);

        /// <summary>
        /// Run the given actions to initialize the service collection and it's container, and then resolve this type from the container.
        /// </summary>
        /// <returns>An initialized <see cref="IHost"/></returns>
        public T Parse<T>() => TestHostFactory.CreateServiceProvider(this).GetRequiredService<T>();

        /// <summary>
        /// Run the given actions to initialize the service collection.
        /// </summary>
        /// <returns>An initialized <see cref="IHost"/></returns>
        public IServiceCollection Parse() => TestHostFactory.CreateServiceCollection(this);

        #region Interfaces
        public IDictionary<object, object?> Properties { get; }

        /// <summary>
        /// Set up the configuration for the builder itself. This will be used to initialize the <see cref="IHostEnvironment"/>
        /// for use later in the build process. This can be called multiple times and the results will be additive.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the host.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            _configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <summary>
        /// Sets up the configuration for the remainder of the build process and application. This can be called multiple times and
        /// the results will be additive. The results will be available at <see cref="HostBuilderContext.Configuration"/> for
        /// subsequent operations, as well as in <see cref="IHost.Services"/>.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the host.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            _configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <summary>
        /// Adds services to the container. This can be called multiple times and the results will be additive.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the host.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            _configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <summary>
        /// Overrides the factory used to create the service provider.
        /// </summary>
        /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
        /// <param name="factory">A factory used for creating service providers.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException(nameof(factory)));
            return this;
        }

        /// <summary>
        /// Overrides the factory used to create the service provider.
        /// </summary>
        /// <param name="factory">A factory used for creating service providers.</param>
        /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException(nameof(factory)));
            return this;
        }

        /// <summary>
        /// Enables configuring the instantiated dependency container. This can be called multiple times and
        /// the results will be additive.
        /// </summary>
        /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the host.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
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

        private IConventionHostBuilder ConventionHostBuilder => Properties.GetConventions();

        [ExcludeFromCodeCoverage]
        public IConventionScanner Scanner => ConventionHostBuilder.Scanner;

        [ExcludeFromCodeCoverage]
        public IAssemblyCandidateFinder AssemblyCandidateFinder => ConventionHostBuilder.AssemblyCandidateFinder;

        [ExcludeFromCodeCoverage]
        public IServiceProviderDictionary ServiceProperties => ConventionHostBuilder.ServiceProperties;

        [ExcludeFromCodeCoverage]
        public IAssemblyProvider AssemblyProvider => ConventionHostBuilder.AssemblyProvider;

        [ExcludeFromCodeCoverage]
        public ILogger DiagnosticLogger => ConventionHostBuilder.DiagnosticLogger;

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions) => ConventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(params IConvention[] conventions) => ConventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions) => ConventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(params Type[] conventions) => ConventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention<T>()
            where T : IConvention => ConventionHostBuilder.AppendConvention<T>();

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions) => ConventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(params IConvention[] conventions) => ConventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions) => ConventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(params Type[] conventions) => ConventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention<T>()
            where T : IConvention => ConventionHostBuilder.PrependConvention<T>();

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates) => ConventionHostBuilder.AppendDelegate(delegates);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendDelegate(params Delegate[] delegates) => ConventionHostBuilder.AppendDelegate(delegates);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates) => ConventionHostBuilder.PrependDelegate(delegates);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependDelegate(params Delegate[] delegates) => ConventionHostBuilder.PrependDelegate(delegates);

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="value">The value to save</param>
        public TestHostBuilder Set<T>(T value)
        {
            this.ServiceProperties[typeof(T)] = value;
            return this;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public TestHostBuilder Set<T>(string key, T value)
        {
            this.ServiceProperties[key] = value;
            return this;
        }
        #endregion
    }
}