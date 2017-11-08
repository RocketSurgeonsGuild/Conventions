namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base contribution context interface that defines the indexer
    /// </summary>
    public interface IConventionContext
    {
        object this[object item] { get; set; }
    }
}
