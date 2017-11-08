namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Marker interface for a convention
    /// </summary>
    /// TODO Edit XML Comment Template for IConvention
    public interface IConvention { }

    /// <summary>
    /// Contribution with a context type
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IConvention<in TContext> : IConvention
        where TContext : IConventionContext
    {
        /// <summary>
        /// Calls the convention to activate it
        /// </summary>
        /// <param name="context"></param>
        void Register(TContext context);
    }
}
