namespace Rocket.Surgery.Conventions;

/// <summary>
///     IConventionProvider
/// </summary>
public interface IConventionProvider
{
    /// <summary>
    ///     Gets a all the conventions from the provider filtered by host type
    /// </summary>
    IEnumerable<object> GetAll();
}
