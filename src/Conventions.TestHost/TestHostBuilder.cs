using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

#if NETSTANDARD2_1
using Rocket.Surgery.Conventions.Internals;
#endif

#pragma warning disable IDE0058 // Expression value is never used

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Class ConventionTestHostBuilder.
    /// Implements the <see cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    /// </summary>
    /// <remarks>
    /// The TestHost does not support other service builders
    /// </remarks>
    /// <seealso cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    public class TestHostBuilder : IHostBuilder, IConventionHostBuilder
    {
        private readonly IHostBuilder _hostBuilder;
        private IConventionHostBuilder _conventionHostBuilder => _hostBuilder.GetConventions();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestHostBuilder" /> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        internal TestHostBuilder(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        /// <summary>
        /// Build the configuration and service provider based on the input environment.
        /// </summary>
        public IHost Build() => _hostBuilder.Build();

#if NETSTANDARD2_1
        /// <summary>
        /// Gets the service collection (by building the host) to ensure all the services are populated
        /// </summary>
        /// <returns></returns>
        public IServiceCollection Parse()
        {
            var field = _hostBuilder.GetType().GetField("_serviceProviderFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldValue = field.GetValue(_hostBuilder);
            var factory = ServiceFactoryAdapter.Create(fieldValue);
            if (!fieldValue.GetType().IsGenericType)
            {
                throw new NotSupportedException("Service Provider Factory must be generic");
            }

            var genericType = fieldValue.GetType().GetGenericArguments()[0];
            var servicesContainer = ServiceProviderFactory.ConfigureFactory(_hostBuilder, genericType, factory);

            try
            {
                _hostBuilder.Build();
            }
            catch (InvalidOperationException exception)
            {

            }

            return servicesContainer.Services;
        }
#endif

        #region Interfaces
        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) => _hostBuilder.ConfigureHostConfiguration(configureDelegate);

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) => _hostBuilder.ConfigureAppConfiguration(configureDelegate);

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) => _hostBuilder.ConfigureServices(configureDelegate);

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) => _hostBuilder.UseServiceProviderFactory(factory);

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) => _hostBuilder.UseServiceProviderFactory(factory);

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) => _hostBuilder.ConfigureContainer(configureDelegate);

        public IDictionary<object, object> Properties => _hostBuilder.Properties;
        public IConventionScanner Scanner => _conventionHostBuilder.Scanner;

        public IAssemblyCandidateFinder AssemblyCandidateFinder => _conventionHostBuilder.AssemblyCandidateFinder;

        public IServiceProviderDictionary ServiceProperties => _conventionHostBuilder.ServiceProperties;

        public IAssemblyProvider AssemblyProvider => _conventionHostBuilder.AssemblyProvider;

        public ILogger DiagnosticLogger => _conventionHostBuilder.DiagnosticLogger;

        public IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions) => _conventionHostBuilder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(params IConvention[] conventions) => _conventionHostBuilder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions) => _conventionHostBuilder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(params Type[] conventions) => _conventionHostBuilder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention<T>()
            where T : IConvention => _conventionHostBuilder.AppendConvention<T>();

        public IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions) => _conventionHostBuilder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(params IConvention[] conventions) => _conventionHostBuilder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions) => _conventionHostBuilder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(params Type[] conventions) => _conventionHostBuilder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention<T>()
            where T : IConvention => _conventionHostBuilder.PrependConvention<T>();

        public IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates) => _conventionHostBuilder.AppendDelegate(delegates);

        public IConventionHostBuilder AppendDelegate(params Delegate[] delegates) => _conventionHostBuilder.AppendDelegate(delegates);

        public IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates) => _conventionHostBuilder.PrependDelegate(delegates);

        public IConventionHostBuilder PrependDelegate(params Delegate[] delegates) => _conventionHostBuilder.PrependDelegate(delegates);
        #endregion
    }
}