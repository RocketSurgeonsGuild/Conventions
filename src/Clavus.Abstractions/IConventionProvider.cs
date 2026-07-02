namespace Rocket.Surgery.Clavus;

/// <summary>
///     IClavusProvider
/// </summary>
public interface IClavusProvider
{
    /// <summary>
    ///     Gets a all the conventions from the provider filtered by host type
    /// </summary>
    IEnumerable<object> GetAll();
}
