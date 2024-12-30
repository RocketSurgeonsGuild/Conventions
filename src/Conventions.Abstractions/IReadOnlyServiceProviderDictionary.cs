namespace Rocket.Surgery.Conventions;

/// <summary>
///     IServiceProviderDictionary
///     Implements the <see cref="IDictionary{TKey,TValue}" />
///     Implements the <see cref="IServiceProvider" />
/// </summary>
/// <seealso cref="IDictionary{Object, Object}" />
/// <seealso cref="IServiceProvider" />
public interface IReadOnlyServiceProviderDictionary : IReadOnlyDictionary<object, object>, IServiceProvider
{
    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    T? Get<T>() where T : notnull => TryGetValue(typeof(T), out var value) && value is T t
        ? t
        : default;

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    T Require<T>()
        where T : notnull => TryGetValue(typeof(T), out var value) && value is T t
        ? t
        : throw new KeyNotFoundException($"The value of type {typeof(T).Name} was not found in the context");

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    T? Get<T>(string key)
        where T : notnull => TryGetValue(typeof(T), out var value) && value is T t
        ? t
        : default;

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    T Require<T>(string key)
        where T : notnull => TryGetValue(key, out var value) && value is T t
        ? t
        : throw new KeyNotFoundException($"The value of type {typeof(T).Name} with the {key} was not found in the context");
}
