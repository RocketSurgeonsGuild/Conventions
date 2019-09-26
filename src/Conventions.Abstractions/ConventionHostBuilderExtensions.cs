using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention extensions
    /// </summary>
    public static class ConventionHostBuilderExtensions
    {

#nullable disable
        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IConventionHostBuilder context) => (T)context.ServiceProperties[typeof(T)];

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IConventionHostBuilder context, string key) => (T)context.ServiceProperties[key];
#nullable restore

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        public static T GetOrAdd<T>(this IConventionHostBuilder builder, Func<T> factory)
            where T : class
        {
            if (!(builder.ServiceProperties[typeof(T)] is T value))
            {
                value = factory();
                builder.Set(value);
            }
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
        public static T GetOrAdd<T>(this IConventionHostBuilder builder, string key, Func<T> factory)
            where T : class
        {
            if (!(builder.ServiceProperties[key] is T value))
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
        public static void Set<T>(this IConventionHostBuilder context, T value) => context.ServiceProperties[typeof(T)] = value;

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(this IConventionHostBuilder context, string key, T value) => context.ServiceProperties[key] = value;

#nullable disable
        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IServiceProviderDictionary serviceProviderDictionary) => (T)serviceProviderDictionary[typeof(T)];

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IServiceProviderDictionary serviceProviderDictionary, string key) => (T)serviceProviderDictionary[key];
#nullable restore

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        public static T GetOrAdd<T>(this IServiceProviderDictionary serviceProviderDictionary, Func<T> factory)
            where T : class
        {
            if (!(serviceProviderDictionary[typeof(T)] is T value))
            {
                value = factory();
                serviceProviderDictionary.Set(value);
            }
            return value;
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        public static T GetOrAdd<T>(this IServiceProviderDictionary serviceProviderDictionary, string key, Func<T> factory)
            where T : class
        {
            if (!(serviceProviderDictionary[key] is T value))
            {
                value = factory();
                serviceProviderDictionary.Set(value);
            }
            return value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(this IServiceProviderDictionary serviceProviderDictionary, T value) => serviceProviderDictionary[typeof(T)] = value;

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(this IServiceProviderDictionary serviceProviderDictionary, string key, T value) => serviceProviderDictionary[key] = value;

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        public static bool IsTestHost(this IConventionHostBuilder context) => context.ServiceProperties.IsTestHost();

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="serviceProviderDictionary">The properties</param>
        public static bool IsTestHost(this IServiceProviderDictionary serviceProviderDictionary) => serviceProviderDictionary.Get<HostType>() == HostType.TestHost;
    }
}
