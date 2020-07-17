// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace Rocket.Surgery.Conventions.Internals
{
    public static class TestHostFactory
    {
        private static readonly ConditionalWeakTable<object, IConfiguration> _sharedConfigurations = new ConditionalWeakTable<object, IConfiguration>();
        internal static IHost Create(TestHostBuilder testHostBuilder) => CreateServiceProvider(testHostBuilder).GetRequiredService<IHost>();

        internal static IServiceCollection CreateServiceCollection(TestHostBuilder testHostBuilder)
        {
            var hostBuilderContext = CreateHostBuilderContext(
                testHostBuilder._configureHostConfigActions,
                testHostBuilder._configureAppConfigActions,
                testHostBuilder.Properties
            );
            return CreateServiceCollection(hostBuilderContext, testHostBuilder._configureServicesActions);
        }

        internal static IServiceProvider CreateServiceProvider(TestHostBuilder testHostBuilder)
        {
            var hostBuilderContext = CreateHostBuilderContext(
                testHostBuilder._configureHostConfigActions,
                testHostBuilder._configureAppConfigActions,
                testHostBuilder.Properties
            );
            var services = CreateServiceCollection(hostBuilderContext, testHostBuilder._configureServicesActions);
            return CreateServiceProvider(hostBuilderContext, services, testHostBuilder._configureContainerActions, testHostBuilder._serviceProviderFactory);
        }

        internal static HostBuilderContext CreateHostBuilderContext(
            IEnumerable<Action<IConfigurationBuilder>> configureHostConfigActions,
            IEnumerable<Action<HostBuilderContext, IConfigurationBuilder>> configureAppConfigActions,
            IDictionary<object, object?> properties
        )
        {
            if (properties.TryGetValue(typeof(IConfiguration).FullName, out var item))
            {
                if (item is IConfiguration configuration)
                {
                    return CreateReusedContext(configuration);
                }

                if (item != null)
                {
                    if (!_sharedConfigurations.TryGetValue(item, out configuration))
                    {
                        var innerContext = CreateContext();
                        configuration = innerContext.Configuration;
                        _sharedConfigurations.Add(item, configuration);
                        innerContext.Configuration = new ConfigurationBuilder().AddConfiguration(configuration, false).Build();
                        return innerContext;
                    }

                    return CreateReusedContext(configuration);
                }
            }

            return CreateContext();

            HostBuilderContext CreateContext()
            {
                var hostConfiguration = BuildHostConfiguration(configureHostConfigActions);
                var hostEnvironment = CreateHostingEnvironment(hostConfiguration);
                var hostBuilderContext = CreateHostBuilderContext(hostEnvironment, hostConfiguration, properties);
                BuildAppConfiguration(hostBuilderContext, configureAppConfigActions);
                return hostBuilderContext;
            }

            HostBuilderContext CreateReusedContext(IConfiguration configuration)
            {
                var safeConfig = new ConfigurationBuilder().AddConfiguration(configuration, false).Build();
                var hostEnvironment = CreateHostingEnvironment(safeConfig);
                var hostBuilderContext = CreateHostBuilderContext(hostEnvironment, safeConfig, properties);
                return hostBuilderContext;
            }
        }

        private static IConfiguration BuildHostConfiguration(IEnumerable<Action<IConfigurationBuilder>> configureHostConfigActions)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
               .AddInMemoryCollection(); // Make sure there's some default storage since there are no default providers

            foreach (Action<IConfigurationBuilder> buildAction in configureHostConfigActions)
            {
                buildAction(configBuilder);
            }

            return configBuilder.Build();
        }

        private static IHostEnvironment CreateHostingEnvironment(IConfiguration hostConfiguration)
        {
            var hostingEnvironment = new HostingEnvironment()
            {
                ApplicationName = hostConfiguration[HostDefaults.ApplicationKey],
                EnvironmentName = hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production,
                ContentRootPath = ResolveContentRootPath(hostConfiguration[HostDefaults.ContentRootKey], AppContext.BaseDirectory),
            };

            if (string.IsNullOrEmpty(hostingEnvironment.ApplicationName))
            {
                // Note GetEntryAssembly returns null for the net4x console test runner.
                hostingEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
            }

            hostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(hostingEnvironment.ContentRootPath);
            return hostingEnvironment;
        }

        internal static string ResolveContentRootPath(string contentRootPath, string basePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                return basePath;
            }

            if (Path.IsPathRooted(contentRootPath))
            {
                return contentRootPath;
            }

            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
        }

        private static HostBuilderContext CreateHostBuilderContext(
            IHostEnvironment hostingEnvironment,
            IConfiguration hostConfiguration,
            IDictionary<object, object?> properties
        ) => new HostBuilderContext(properties.ToDictionary(z => z.Key, z => z.Value))
        {
            HostingEnvironment = hostingEnvironment,
            Configuration = hostConfiguration
        };

        private static IConfiguration BuildAppConfiguration(
            HostBuilderContext hostBuilderContext,
            IEnumerable<Action<HostBuilderContext, IConfigurationBuilder>> configureAppConfigActions
        )
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
               .SetBasePath(hostBuilderContext.HostingEnvironment.ContentRootPath)
               .AddConfiguration(hostBuilderContext.Configuration, shouldDisposeConfiguration: true);

            foreach (Action<HostBuilderContext, IConfigurationBuilder> buildAction in configureAppConfigActions)
            {
                buildAction(hostBuilderContext, configBuilder);
            }

            var appConfiguration = configBuilder.Build();
            hostBuilderContext.Configuration = appConfiguration;
            return appConfiguration;
        }

        private static IServiceCollection CreateServiceCollection(
            HostBuilderContext hostBuilderContext,
            List<Action<HostBuilderContext, IServiceCollection>> configureServicesActions
        )
        {
            var services = new ServiceCollection();
#pragma warning disable CS0618 // Type or member is obsolete
            services.AddSingleton<IHostingEnvironment>((HostingEnvironment)hostBuilderContext.HostingEnvironment);
#pragma warning restore CS0618 // Type or member is obsolete
            services.AddSingleton<IHostEnvironment>(hostBuilderContext.HostingEnvironment);
            services.AddSingleton(hostBuilderContext);
            // register configuration as factory to make it dispose with the service provider
            services.AddSingleton(_ => hostBuilderContext.Configuration);
            if (hostBuilderContext.Configuration is IConfigurationRoot configurationRoot)
                services.AddSingleton(_ => configurationRoot);
#pragma warning disable CS0618 // Type or member is obsolete
            services.AddSingleton(s => (IApplicationLifetime)s.GetService<IHostApplicationLifetime>());
#pragma warning restore CS0618 // Type or member is obsolete
            services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
            services.AddSingleton<IHostLifetime, ConsoleLifetime>();
            services.AddSingleton(typeof(IHost), typeof(HostBuilder).Assembly.GetType("Microsoft.Extensions.Hosting.Internal.Host"));
            // services.AddSingleton<IHost, Internal.Host>();
            services.AddOptions();
            services.AddLogging();

            foreach (Action<HostBuilderContext, IServiceCollection> configureServicesAction in configureServicesActions)
            {
                configureServicesAction(hostBuilderContext, services);
            }

            return services;
        }

        private static IServiceProvider CreateServiceProvider(
            HostBuilderContext hostBuilderContext,
            IServiceCollection services,
            List<IConfigureContainerAdapter> configureContainerActions,
            IServiceFactoryAdapter serviceProviderFactory
        )
        {
            object containerBuilder = serviceProviderFactory.CreateBuilder(services, hostBuilderContext);

            foreach (IConfigureContainerAdapter containerAction in configureContainerActions)
            {
                containerAction.ConfigureContainer(hostBuilderContext, containerBuilder);
            }

            var serviceProvider = serviceProviderFactory.CreateServiceProvider(containerBuilder);

            if (serviceProvider == null)
            {
                throw new InvalidOperationException($"The IServiceProviderFactory returned a null IServiceProvider.");
            }

            // resolve configuration explicitly once to mark it as resolved within the
            // service provider, ensuring it will be properly disposed with the provider
            _ = serviceProvider.GetService<IConfiguration>();
            return serviceProvider;
        }
    }
}