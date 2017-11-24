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
        /// <param name="context">The context</param>
        public static T Get<T>(this IConventionContext context)
        {
            return (T)context[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        public static T Get<T>(this IConventionContext context, string key)
        {
            return (T)context[key];
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="TContext">The context type</typeparam>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="value">The value to save</param>
        public static TContext Set<TContext, T>(this TContext context, T value)
            where TContext : IConventionContext
        {
            context[typeof(T)] = value;
            return context;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="TContext">The context type</typeparam>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static TContext Set<TContext, T>(this TContext context, string key, T value)
            where TContext : IConventionContext
        {
            context[key] = value;
            return context;
        }
    }
}
