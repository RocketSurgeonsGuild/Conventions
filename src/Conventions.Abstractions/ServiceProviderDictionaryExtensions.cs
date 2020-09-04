using System;
using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention extensions
    /// </summary>
    public static class ServiceProviderDictionaryExtensions
    {
        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this IReadOnlyServiceProviderDictionary serviceProviderDictionary)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T?)serviceProviderDictionary[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this IReadOnlyServiceProviderDictionary serviceProviderDictionary, string key)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T?)serviceProviderDictionary[key];
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T?)serviceProviderDictionary[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, string key)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T?)serviceProviderDictionary[key];
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this ServiceProviderDictionary serviceProviderDictionary)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T?)serviceProviderDictionary[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T? Get<T>([NotNull] this ServiceProviderDictionary serviceProviderDictionary, string key)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T?)serviceProviderDictionary[key];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, [NotNull] Func<T> factory)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( serviceProviderDictionary[typeof(T)] is T value ))
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
        [NotNull]
        public static T GetOrAdd<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, string key, [NotNull] Func<T> factory)
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( serviceProviderDictionary[key] is T value ))
            {
                value = factory();
                serviceProviderDictionary.Set(value);
            }

            return value;
        }

        /// <summary>
        /// Set key to the value
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="value">The value to save</param>
        public static IServiceProviderDictionary Set<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, T value)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            serviceProviderDictionary[typeof(T)] = value;
            return serviceProviderDictionary;
        }

        /// <summary>
        /// Set key to the value
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static IServiceProviderDictionary Set<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, string key, T value)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            serviceProviderDictionary[key] = value;
            return serviceProviderDictionary;
        }

        /// <summary>
        /// Set key to the value if the type is missing
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="value">The value to save</param>
        public static IServiceProviderDictionary AddIfMissing<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, T value)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            if (serviceProviderDictionary.TryGetValue(typeof(T), out _))
                return serviceProviderDictionary;

            serviceProviderDictionary[typeof(T)] = value;
            return serviceProviderDictionary;
        }

        /// <summary>
        /// Set key to the value if the key is missing
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static IServiceProviderDictionary AddIfMissing<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, string key, T value)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            if (serviceProviderDictionary.TryGetValue(key, out _))
                return serviceProviderDictionary;

            serviceProviderDictionary[key] = value;
            return serviceProviderDictionary;
        }
    }
}