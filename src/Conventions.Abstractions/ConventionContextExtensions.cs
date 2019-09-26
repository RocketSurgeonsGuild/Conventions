using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention extensions
    /// </summary>
    public static class ConventionContextExtensions
    {
#nullable disable
        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IConventionContext context) => (T)context[typeof(T)];

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        public static T Get<T>(this IConventionContext context, string key) => (T)context[key];
#nullable restore

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        public static T GetOrAdd<T>(this IConventionContext context, Func<T> factory)
            where T : class
        {
            if (!(context[typeof(T)] is T value))
            {
                value = factory();
                context.Set(value);
            }
            return value;
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        public static T GetOrAdd<T>(this IConventionContext context, string key, Func<T> factory)
            where T : class
        {
            if (!(context[key] is T value))
            {
                value = factory();
                context.Set(value);
            }
            return value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(this IConventionContext context, T value) => context[typeof(T)] = value;

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(this IConventionContext context, string key, T value) => context[key] = value;

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        public static bool IsUnitTestHost(this IConventionContext context) => context.Get<HostType>() == HostType.UnitTestHost;

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        internal static HostType? GetHostType(this IConventionContext context) => context.Properties.TryGetValue(typeof(HostType), out var hostType) ? (HostType?)hostType : null;
    }
}
