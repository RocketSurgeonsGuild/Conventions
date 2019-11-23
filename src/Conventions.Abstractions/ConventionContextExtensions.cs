using System;
using JetBrains.Annotations;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention extensions
    /// </summary>
    public static class ConventionContextExtensions
    {
        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T Get<T>([NotNull] this IConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (T)context[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T Get<T>([NotNull] this IConventionContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (T)context[key];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>([NotNull] this IConventionContext context, [NotNull] Func<T> factory)
            where T : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( context[typeof(T)] is T value ))
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
        [NotNull]
        public static T GetOrAdd<T>([NotNull] this IConventionContext context, string key, [NotNull] Func<T> factory)
            where T : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( context[key] is T value ))
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
        public static void Set<T>([NotNull] this IConventionContext context, T value)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context[typeof(T)] = value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>([NotNull] this IConventionContext context, string key, T value)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context[key] = value;
        }

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        public static bool IsUnitTestHost([NotNull] this IConventionContext context)
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
        internal static HostType GetHostType(this IConventionContext context)
            => context.Properties.TryGetValue(typeof(HostType), out var hostType)
                ? (HostType)hostType!
                : HostType.Undefined;
    }
}