using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Conventions.Reflection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension method to apply service conventions
    /// </summary>
    public static class RocketSurgeryServiceCollectionExtensions
    {
        /// <summary>
        /// Apply service conventions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conventionContext"></param>
        /// <returns></returns>
        public static IServiceCollection ApplyConventions(this IServiceCollection services, IConventionContext conventionContext)
        {
            var configuration = conventionContext.Get<IConfiguration>();
            if (configuration is null)
            {
                configuration = new ConfigurationBuilder().Build();
                conventionContext.Logger.LogWarning("Configuration was not found in context");
            }

            foreach (var item in conventionContext.Conventions.Get<IServiceConvention, ServiceConvention>())
            {
                if (item is IServiceConvention convention)
                {
                    convention.Register(conventionContext, configuration, services);
                }
                else if (item is ServiceConvention @delegate)
                {
                    @delegate(conventionContext, configuration, services);
                }
            }

            return services;
        }
    }

    class LoggingBuilder : ILoggingBuilder
    {
        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}