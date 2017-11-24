namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// The base context marker interface to define this as a context
    /// </summary>
    public interface IConventionContext
    {
        /// <summary>
        /// Allows a context to hold additional information for conventions to consume such as configuration objects
        /// </summary>
        object this[object item] { get; set; }
    }
}
