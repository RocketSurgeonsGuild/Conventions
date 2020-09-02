using System;
using System.Linq;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    /// <summary>
    /// IWebAssemblyHostingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IWebAssemblyHostingConventionContext}" />
    public interface IWebAssemblyHostingConvention : IConvention<IWebAssemblyHostingConventionContext> { }

    /// <summary>
    /// The default blazor web assembly host build does not allow construction in non-blazor like environments.
    /// </summary>
    public interface IWebAssemblyHostBuilder
    {
        /// <summary>
        /// Gets an <see cref="WebAssemblyHostConfiguration"/> that can be used to customize the application's
        /// configuration sources and read configuration attributes.
        /// </summary>
        WebAssemblyHostConfiguration Configuration { get; }

        /// <summary>
        /// Gets the collection of root component mappings configured for the application.
        /// </summary>
        RootComponentMappingCollection RootComponents { get; }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets information about the app's host environment.
        /// </summary>
        IWebAssemblyHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the logging builder for configuring logging services.
        /// </summary>
        ILoggingBuilder Logging { get; }

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
        void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure = null);

        /// <summary>
        /// Builds a <see cref="WebAssemblyHost"/> instance based on the configuration of this builder.
        /// </summary>
        /// <returns>A <see cref="WebAssemblyHost"/> object.</returns>
        WebAssemblyHost Build();
    }

    internal class WrappedWebAssemblyHostBuilder : IWebAssemblyHostBuilder
    {
        private readonly WebAssemblyHostBuilder _webAssemblyHostBuilder;
        public WrappedWebAssemblyHostBuilder(WebAssemblyHostBuilder webAssemblyHostBuilder) => _webAssemblyHostBuilder = webAssemblyHostBuilder;

        public WebAssemblyHostConfiguration Configuration => _webAssemblyHostBuilder.Configuration;
        public RootComponentMappingCollection RootComponents => _webAssemblyHostBuilder.RootComponents;
        public IServiceCollection Services => _webAssemblyHostBuilder.Services;
        public IWebAssemblyHostEnvironment HostEnvironment => _webAssemblyHostBuilder.HostEnvironment;
        public ILoggingBuilder Logging => _webAssemblyHostBuilder.Logging;
        public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder>? configure = null)
            => _webAssemblyHostBuilder.ConfigureContainer(factory, configure!);
        public WebAssemblyHost Build() => _webAssemblyHostBuilder.Build();
        internal WebAssemblyHostBuilder WebAssemblyHostBuilder => _webAssemblyHostBuilder;
    }
}