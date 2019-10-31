using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.WebJobs;
using ConfigurationBuilder = Rocket.Surgery.Extensions.Configuration.ConfigurationBuilder;
using MsftConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace Rocket.Surgery.Hosting.Functions
{
    /// <summary>
    /// Class RocketFunctionHostBuilder.
    /// Implements the <see cref="ConventionHostBuilder{IRocketFunctionHostBuilder}" />
    /// Implements the <see cref="IRocketFunctionHostBuilder" />
    /// </summary>
    /// <seealso cref="ConventionHostBuilder{IRocketFunctionHostBuilder}" />
    /// <seealso cref="IRocketFunctionHostBuilder" />
    class RocketFunctionHostBuilder : ConventionHostBuilder<IRocketFunctionHostBuilder>, IRocketFunctionHostBuilder
    {
        private readonly object _startupInstance;
        private readonly IRocketEnvironment _environment;
        private readonly DiagnosticLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketFunctionHostBuilder"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="functionsAssembly">The functions assembly.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="serviceProviderDictionary">The service provider dictionary of values</param>
        public RocketFunctionHostBuilder(
            IWebJobsBuilder builder,
            Assembly functionsAssembly,
            object startupInstance,
            IRocketEnvironment environment,
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            DiagnosticSource diagnosticSource,
            IServiceProviderDictionary serviceProviderDictionary) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticSource, serviceProviderDictionary)
        {
            _startupInstance = startupInstance;
            _environment = environment ?? CreateEnvironment();
            _logger = new DiagnosticLogger(DiagnosticSource);
            Builder = builder;
            FunctionsAssembly = functionsAssembly;
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public IWebJobsBuilder Builder { get; }

        /// <summary>
        /// Gets the functions assembly.
        /// </summary>
        /// <value>The functions assembly.</value>
        public Assembly FunctionsAssembly { get; }

        /// <summary>
        /// Withes the specified scanner.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IConventionScanner scanner)
        {
            return new RocketFunctionHostBuilder(Builder, FunctionsAssembly, _startupInstance, _environment, scanner, AssemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties);
        }

        /// <summary>
        /// Withes the specified assemby.
        /// </summary>
        /// <param name="assemby">The assemby.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(Assembly assemby)
        {
            return new RocketFunctionHostBuilder(Builder, assemby, _startupInstance, _environment, Scanner, AssemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties);
        }

        /// <summary>
        /// Withes the specified assembly candidate finder.
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            return new RocketFunctionHostBuilder(Builder, FunctionsAssembly, _startupInstance, _environment, Scanner, assemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties);
        }

        /// <summary>
        /// Withes the specified assembly provider.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IAssemblyProvider assemblyProvider)
        {
            return new RocketFunctionHostBuilder(Builder, FunctionsAssembly, _startupInstance, _environment, Scanner, AssemblyCandidateFinder, assemblyProvider, DiagnosticSource, ServiceProperties);
        }

        /// <summary>
        /// Withes the specified diagnostic source.
        /// </summary>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(DiagnosticSource diagnosticSource)
        {
            return new RocketFunctionHostBuilder(Builder, FunctionsAssembly, _startupInstance, _environment, Scanner, AssemblyCandidateFinder, AssemblyProvider, diagnosticSource, ServiceProperties);
        }

        /// <summary>
        /// Withes the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IRocketEnvironment environment)
        {
            return new RocketFunctionHostBuilder(Builder, FunctionsAssembly, _startupInstance, environment, Scanner, AssemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties);
        }

        private static IRocketEnvironment CreateEnvironment()
        {
            var environmentNames = new[]
            {
                Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"),
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME"),
                "Production"
            };

            var applicationNames = new[]
            {
                Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"),
                "Functions"
            };

            return new RocketEnvironment(
                environmentNames.First(x => !string.IsNullOrEmpty(x)),
                applicationNames.First(x => !string.IsNullOrEmpty(x)),
                contentRootPath: null,
                contentRootFileProvider: null
            );
        }

        private IConfiguration SetupConfiguration()
        {
            var currentDirectory = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") ?? "/home/site/wwwroot";
            bool isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")) && !Directory.Exists(currentDirectory);
            if (isLocal)
            {
                currentDirectory = Environment.CurrentDirectory;
            }

            var existingConfiguration = Builder.Services.First(z => z.ServiceType == typeof(IConfiguration))
                .ImplementationInstance as IConfiguration;

            var configurationOptions = this.GetOrAdd(() => new ConfigurationOptions());

            var fileProvider = new PhysicalFileProvider(currentDirectory);
            var configurationBuilder = new MsftConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddConfiguration(existingConfiguration)
                .AddJsonFile(fileProvider, "appsettings.json", optional: true, reloadOnChange: true);

            foreach (var provider in configurationOptions.SettingsConfigurationSourceProviders)
                configurationBuilder = configurationBuilder.Add(provider(fileProvider));

            configurationBuilder.AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            foreach (var provider in configurationOptions.EnvironmentSettingsConfigurationSourceProviders)
                configurationBuilder = configurationBuilder.Add(provider(fileProvider, _environment.EnvironmentName));

            configurationBuilder.AddJsonFile($"appsettings.local.json", optional: true, reloadOnChange: true);

            foreach (var provider in configurationOptions.LocalSettingsConfigurationSourceProvider)
                configurationBuilder = configurationBuilder.Add(provider(fileProvider));

            if (_environment.IsDevelopment())
            {
                configurationBuilder.AddUserSecrets(FunctionsAssembly, optional: true);
            }

            configurationBuilder
                .AddEnvironmentVariables("RSG_")
                .AddEnvironmentVariables();

            IConfigurationSource? source = null;
            foreach (var item in configurationBuilder.Sources.Reverse())
            {
                if ((item is EnvironmentVariablesConfigurationSource env && (string.IsNullOrWhiteSpace(env.Prefix) || string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase))) || (item is JsonConfigurationSource a && string.Equals(a.Path, "secrets.json", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                source = item;
                break;
            }

            var index = source == null ? configurationBuilder.Sources.Count - 1 : configurationBuilder.Sources.IndexOf(source);

            var cb = new ConfigurationBuilder(
                Scanner,
                _environment,
                new MsftConfigurationBuilder().AddConfiguration(existingConfiguration!).AddConfiguration(configurationBuilder.Build()).Build(),
                configurationBuilder,
                _logger,
                Properties);

            configurationBuilder.Sources.Insert(index + 1, new ChainedConfigurationSource()
            {
                Configuration = cb.Build()
            });

            var newConfig = configurationBuilder.Build();

            Builder.Services.Replace(ServiceDescriptor.Singleton<IConfiguration>(newConfig));
            return newConfig;
        }

        private void SetupServices(IConfiguration existingConfiguration)
        {
            var builder = new ServicesBuilder(
                Scanner,
                AssemblyProvider,
                AssemblyCandidateFinder,
                Builder.Services,
                existingConfiguration,
                _environment,
                _logger,
                Properties);

            Composer.Register<IServiceConventionContext, IServiceConvention, ServiceConventionDelegate>(Scanner.BuildProvider(), builder);
        }

        private void SetupWebJobs(IConfiguration existingConfiguration)
        {
            var builder = new WebJobsConventionBuilder(
                Scanner,
                AssemblyProvider,
                AssemblyCandidateFinder,
                Builder,
                existingConfiguration,
                _environment,
                _logger,
                Properties);

            builder.Build();
        }

        /// <summary>
        /// Composes this instance.
        /// </summary>
        public void Compose()
        {
            var existingHostedServices = Builder.Services.Where(x => x.ServiceType == typeof(IHostedService)).ToArray();
            if (_startupInstance is IConvention convention)
            {
                Scanner.AppendConvention(convention);
            }

            var configuration = SetupConfiguration();
            SetupServices(configuration);
            SetupWebJobs(configuration);

            Builder.Services.RemoveAll<IHostedService>();
            Builder.Services.Add(existingHostedServices);
        }
    }
}
