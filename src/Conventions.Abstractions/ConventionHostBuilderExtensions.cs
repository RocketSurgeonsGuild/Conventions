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
        /// <param name="serviceProviderDictionary">The context</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IServiceProviderDictionary serviceProviderDictionary) => (T)serviceProviderDictionary[typeof(T)];

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IServiceProviderDictionary serviceProviderDictionary, string key) => (T)serviceProviderDictionary[key];
#nullable restore

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The context</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(this IServiceProviderDictionary serviceProviderDictionary, T value) => serviceProviderDictionary[typeof(T)] = value;

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(this IServiceProviderDictionary serviceProviderDictionary, string key, T value) => serviceProviderDictionary[key] = value;
    }
}
