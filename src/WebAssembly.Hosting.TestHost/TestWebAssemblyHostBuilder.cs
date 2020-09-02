using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.WebAssembly.Hosting;
using Rocket.Surgery.WebAssembly.Hosting.Internals;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// A program initialization utility.
    /// </summary>
    public class TestWebAssemblyHostBuilder : IWebAssemblyHostBuilder, IConventionHostBuilder
    {
        private Func<IServiceProvider> _createServiceProvider;

        internal TestWebAssemblyHostBuilder(
            IServiceProviderDictionary properties,
            string environmentName
        )
        {
            // Private right now because we don't have much reason to expose it. This can be exposed
            // in the future if we want to give people a choice between CreateDefault and something
            // less opinionated.
            Configuration = new WebAssemblyHostConfiguration();
            RootComponents = new RootComponentMappingCollection();
            Services = new ServiceCollection();
            Logging = new LoggingBuilder(Services);
            HostEnvironment = new WebAssemblyHostEnvironment(environmentName, new Uri("http://localhost/").ToString());
            Services.AddSingleton<IWebAssemblyHostEnvironment>(HostEnvironment);
            Services.AddSingleton<IConfiguration>(Configuration);
            Services.AddSingleton<IConfigurationRoot>(Configuration);
            _createServiceProvider = () => Services.BuildServiceProvider(validateScopes: HostEnvironment.IsDevelopment());
            Services.AddSingleton<IJSRuntime>(properties.Get<IJSRuntime>() ??  new NoopJSRuntime() );
            Services.AddSingleton<NavigationManager>(properties.Get<NavigationManager>() ?? new NoopNavigationManager());
            Services.AddSingleton<INavigationInterception>(properties.Get<INavigationInterception>() ?? new NoopNavigationInterception());
        }

        /// <summary>
        /// Run the given actions to initialize the service collection and it's container, and then resolve this type from the container.
        /// </summary>
        /// <returns>An initialized <see cref="WebAssemblyHost"/></returns>
        public T Parse<T>() => _createServiceProvider().GetRequiredService<T>();

        /// <summary>
        /// Run the given actions to initialize the service collection.
        /// </summary>
        /// <returns>An initialized <see cref="WebAssemblyHost"/></returns>
        public IServiceCollection Parse() => Services;

        #region Interfaces

        public ILoggingBuilder Logging { get;  }
        public IServiceCollection Services { get; }
        public RootComponentMappingCollection RootComponents { get;  }
        public WebAssemblyHostConfiguration Configuration { get;  }
        /// <summary>
        /// Gets information about the app's host environment.
        /// </summary>
        public IWebAssemblyHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Registers a <see cref="IServiceProviderFactory{TBuilder}" /> instance to be used to create the <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="factory">The <see cref="IServiceProviderFactory{TBuilder}" />.</param>
        /// <param name="configure">
        /// A delegate used to configure the <typeparamref T="TBuilder" />. This can be used to configure services using
        /// APIS specific to the <see cref="IServiceProviderFactory{TBuilder}" /> implementation.
        /// </param>
        /// <typeparam name="TBuilder">The type of builder provided by the <see cref="IServiceProviderFactory{TBuilder}" />.</typeparam>
        /// <remarks>
        /// <para>
        /// <see cref="ConfigureContainer{TBuilder}(IServiceProviderFactory{TBuilder}, Action{TBuilder})"/> is called by <see cref="Build"/>
        /// and so the delegate provided by <paramref name="configure"/> will run after all other services have been registered.
        /// </para>
        /// <para>
        /// Multiple calls to <see cref="ConfigureContainer{TBuilder}(IServiceProviderFactory{TBuilder}, Action{TBuilder})"/> will replace
        /// the previously stored <paramref name="factory"/> and <paramref name="configure"/> delegate.
        /// </para>
        /// </remarks>
        public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure = null)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _createServiceProvider = () =>
            {
                var container = factory.CreateBuilder(Services);
                configure?.Invoke(container);
                return factory.CreateServiceProvider(container);
            };
        }

        /// <summary>
        /// Builds a <see cref="WebAssemblyHost"/> instance based on the configuration of this builder.
        /// </summary>
        /// <returns>A <see cref="WebAssemblyHost"/> object.</returns>
        public WebAssemblyHost Build()
        {
            // Intentionally overwrite configuration with the one we're creating.
            Services.AddSingleton<IConfiguration>(Configuration);
            var current = this.GetConventions();

            // A Blazor application always runs in a scope. Since we want to make it possible for the user
            // to configure services inside *that scope* inside their startup code, we create *both* the
            // service provider and the scope here.
            var services = _createServiceProvider();
            var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            // this gets removed, but we're going toa dd it back so that same factory can be re-used.
            Services.AddSingleton(current);

            return Activator.CreateInstance(
                typeof(WebAssemblyHost),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                null,
                new object[] { services, scope, Configuration, RootComponents.ToArray() },
                null
            ) as WebAssemblyHost;
        }

        private IConventionHostBuilder ConventionHostBuilder => this.GetConventions();

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
        public TestWebAssemblyHostBuilder Set<T>(T value)
        {
            ServiceProperties[typeof(T)] = value;
            return this;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public TestWebAssemblyHostBuilder Set<T>(string key, T value)
        {
            ServiceProperties[key] = value;
            return this;
        }
        #endregion
    }
}