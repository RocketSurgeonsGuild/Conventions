using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention extensions
    /// </summary>
    public static class ConventionHostBuilderExtensions
    {
        /// <summary>
        /// Configure the services delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureServices([NotNull] this ConventionContextBuilder container, ServiceConvention @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(@delegate);
            return container;
        }

        /// <summary>
        /// Configure the services delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureServices([NotNull] this ConventionContextBuilder container, Action<IConfiguration, IServiceCollection> @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(new ServiceConvention((context, configuration, services) => @delegate(configuration, services)));
            return container;
        }

        /// <summary>
        /// Configure the services delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureServices([NotNull] this ConventionContextBuilder container, Action<IServiceCollection> @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(new ServiceConvention((context, configuration, services) => @delegate(services)));
            return container;
        }

        /// <summary>
        /// Configure the logging delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureLogging([NotNull] this ConventionContextBuilder container, LoggingConvention @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(@delegate);
            return container;
        }

        /// <summary>
        /// Configure the logging delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureLogging([NotNull] this ConventionContextBuilder container, Action<IConfiguration, ILoggingBuilder> @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(new LoggingConvention((context, configuration, builder) => @delegate(configuration, builder)));
            return container;
        }

        /// <summary>
        /// Configure the logging delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureLogging([NotNull] this ConventionContextBuilder container, Action<ILoggingBuilder> @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(new LoggingConvention((context, configuration, builder) => @delegate(builder)));
            return container;
        }

        /// <summary>
        /// Configure the configuration delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureConfiguration([NotNull] this ConventionContextBuilder container, ConfigurationConvention @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(@delegate);
            return container;
        }

        /// <summary>
        /// Configure the configuration delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureConfiguration([NotNull] this ConventionContextBuilder container, Action<IConfiguration, IConfigurationBuilder> @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(new ConfigurationConvention((context, configuration, builder) => @delegate(configuration, builder)));
            return container;
        }

        /// <summary>
        /// Configure the configuration delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns><see cref="ConventionContextBuilder"/>.</returns>
        public static ConventionContextBuilder ConfigureConfiguration([NotNull] this ConventionContextBuilder container, Action<IConfigurationBuilder> @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.AppendDelegate(new ConfigurationConvention((context, configuration, builder) => @delegate(builder)));
            return container;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this ConventionContextBuilder context)
            where T : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (T?)context.Properties[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this ConventionContextBuilder context, string key)
            where T : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (T?)context.Properties[key];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>([NotNull] this ConventionContextBuilder builder, [NotNull] Func<T> factory)
            where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (builder.Properties[typeof(T)] is T value)
            {
                return value;
            }

            value = factory();
            builder.Set(value);

            return value;
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>([NotNull] this ConventionContextBuilder builder, string key, [NotNull] Func<T> factory)
            where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( builder.Properties[key] is T value ))
            {
                value = factory();
                builder.Set(value);
            }

            return value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="value">The value to save</param>
        public static ConventionContextBuilder Set<T>([NotNull] this ConventionContextBuilder context, T value)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Properties[typeof(T)] = value;
            return context;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static ConventionContextBuilder Set<T>([NotNull] this ConventionContextBuilder context, string key, T value)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Properties[key] = value;
            return context;
        }

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        public static bool IsUnitTestHost([NotNull] this ConventionContextBuilder context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.GetHostType() == HostType.UnitTestHost;
        }

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        public static HostType GetHostType(this ConventionContextBuilder context) => context.Properties.TryGetValue(typeof(HostType), out var hostType)
            ? (HostType)hostType!
            : HostType.Undefined;
    }
}