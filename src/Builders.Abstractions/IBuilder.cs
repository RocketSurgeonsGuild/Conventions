namespace Rocket.Surgery.Builders
{
    /// <summary>
    /// Interface IBuilder
    /// </summary>
    public interface IBuilder
    {
        object this[object item] { get; set; }
    }
}
