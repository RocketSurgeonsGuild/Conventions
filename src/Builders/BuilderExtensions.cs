namespace Rocket.Surgery.Builders
{
    /// <summary>
    /// Class IContributionContextExtensions.
    /// </summary>
    /// TODO Edit XML Comment Template for IContributionContextExtensions
    public static class BuilderExtensions
    {
        public static T Get<T>(this IBuilder context)
        {
            return (T)context[typeof(T)];
        }

        public static TContext Set<TContext, T>(this TContext context, T value)
            where TContext : IBuilder
        {
            context[typeof(T)] = value;
            return context;
        }
    }
}
