using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

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

        #region Interfaces
        [ExcludeFromCodeCoverage]
        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) => _hostBuilder.ConfigureHostConfiguration(configureDelegate);

        [ExcludeFromCodeCoverage]
        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) => _hostBuilder.ConfigureAppConfiguration(configureDelegate);

        [ExcludeFromCodeCoverage]
        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) => _hostBuilder.ConfigureServices(configureDelegate);

        [ExcludeFromCodeCoverage]
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) => _hostBuilder.UseServiceProviderFactory(factory);

        [ExcludeFromCodeCoverage]
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) => _hostBuilder.UseServiceProviderFactory(factory);

        [ExcludeFromCodeCoverage]
        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) => _hostBuilder.ConfigureContainer(configureDelegate);

        [ExcludeFromCodeCoverage]
        public IDictionary<object, object> Properties => _hostBuilder.Properties;
        [ExcludeFromCodeCoverage]
        public IConventionScanner Scanner => _conventionHostBuilder.Scanner;

        [ExcludeFromCodeCoverage]
        public IAssemblyCandidateFinder AssemblyCandidateFinder => _conventionHostBuilder.AssemblyCandidateFinder;

        [ExcludeFromCodeCoverage]
        public IServiceProviderDictionary ServiceProperties => _conventionHostBuilder.ServiceProperties;

        [ExcludeFromCodeCoverage]
        public IAssemblyProvider AssemblyProvider => _conventionHostBuilder.AssemblyProvider;

        [ExcludeFromCodeCoverage]
        public ILogger DiagnosticLogger => _conventionHostBuilder.DiagnosticLogger;

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions) => _conventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(params IConvention[] conventions) => _conventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions) => _conventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention(params Type[] conventions) => _conventionHostBuilder.AppendConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendConvention<T>()
            where T : IConvention => _conventionHostBuilder.AppendConvention<T>();

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions) => _conventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(params IConvention[] conventions) => _conventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions) => _conventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention(params Type[] conventions) => _conventionHostBuilder.PrependConvention(conventions);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependConvention<T>()
            where T : IConvention => _conventionHostBuilder.PrependConvention<T>();

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates) => _conventionHostBuilder.AppendDelegate(delegates);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder AppendDelegate(params Delegate[] delegates) => _conventionHostBuilder.AppendDelegate(delegates);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates) => _conventionHostBuilder.PrependDelegate(delegates);

        [ExcludeFromCodeCoverage]
        public IConventionHostBuilder PrependDelegate(params Delegate[] delegates) => _conventionHostBuilder.PrependDelegate(delegates);
        #endregion
    }
}