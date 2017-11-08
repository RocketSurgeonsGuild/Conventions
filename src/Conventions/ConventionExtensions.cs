namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention extensions
    /// </summary>
    public static class ConventionExtensions
    {
        public static T Get<T>(this IConventionContext context)
        {
            return (T)context[typeof(T)];
        }

        public static TContext Set<TContext, T>(this TContext context, T value)
            where TContext : IConventionContext
        {
            context[typeof(T)] = value;
            return context;
        }
    }
}
