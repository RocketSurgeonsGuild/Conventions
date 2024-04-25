namespace Rocket.Surgery.Conventions;

/// <summary>
///     IConventionProvider
/// </summary>
public interface IConventionProvider
{
    /// <summary>
    ///     Gets this instance.  filtered by host type
    /// </summary>
    /// <typeparam name="TContribution">The type of the contribution.</typeparam>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    #pragma warning disable CA1716
    IEnumerable<object> Get<TContribution, TDelegate>()
        where TContribution : IConvention
        where TDelegate : Delegate;
    #pragma warning restore CA1716

    /// <summary>
    ///     Gets this instance.  filtered by host type
    /// </summary>
    /// <typeparam name="TContribution">The type of the contribution.</typeparam>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <typeparam name="TAsyncContribution">The type of the async contribution.</typeparam>
    /// <typeparam name="TAsyncDelegate">The type of the async delegate.</typeparam>
    #pragma warning disable CA1716
    IEnumerable<object> Get<TContribution, TDelegate, TAsyncContribution, TAsyncDelegate>()
        where TContribution : IConvention
        where TDelegate : Delegate
        where TAsyncContribution : IConvention
        where TAsyncDelegate : Delegate;
    #pragma warning restore CA1716

    /// <summary>
    ///     Gets a all the conventions from the provider filtered by host type
    /// </summary>
    IEnumerable<object> GetAll();
}