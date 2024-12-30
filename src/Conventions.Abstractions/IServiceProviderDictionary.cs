namespace Rocket.Surgery.Conventions;

/// <summary>
///     IServiceProviderDictionary
///     Implements the <see cref="IDictionary{TKey,TValue}" />
///     Implements the <see cref="IServiceProvider" />
/// </summary>
/// <seealso cref="IDictionary{Object, Object}" />
/// <seealso cref="IServiceProvider" />
[PublicAPI]
public interface IServiceProviderDictionary : IDictionary<object, object>, IServiceProvider
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
        where T : notnull => TryGetValue(key, out var value) && value is T t
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

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    T GetOrAdd<T>(Func<T> factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(factory);

        if (TryGetValue(typeof(T), out var o) && o is T value) return value;

        value = factory();
        Set(value);
        return value;
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    T GetOrAdd<T>(string key, Func<T> factory)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (TryGetValue(key, out var o) && o is T value) return value;

        value = factory();
        Set(value);
        return value;
    }

    /// <summary>
    ///     Set key to the value
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to save</param>
    IServiceProviderDictionary Set<T>(T value) where T : notnull
    {
        this[typeof(T)] = value;
        return this;
    }

    /// <summary>
    ///     Set key to the value
    /// </summary>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    IServiceProviderDictionary Set(Type key, object value)
    {
        this[key] = value;
        return this;
    }

    /// <summary>
    ///     Set key to the value
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    IServiceProviderDictionary Set<T>(string key, T value) where T : notnull
    {
        this[key] = value;
        return this;
    }

    /// <summary>
    ///     Set key to the value if the type is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to save</param>
    IServiceProviderDictionary AddIfMissing<T>(T value) where T : notnull
    {
        if (TryGetValue(typeof(T), out _))
            return this;

        this[typeof(T)] = value;
        return this;
    }

    /// <summary>
    ///     Set key to the value if the key is missing
    /// </summary>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    IServiceProviderDictionary AddIfMissing(Type key, object value)
    {
        if (TryGetValue(key, out _))
            return this;

        this[key] = value;
        return this;
    }

    /// <summary>
    ///     Set key to the value if the key is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    IServiceProviderDictionary AddIfMissing<T>(string key, T value) where T : notnull
    {
        if (TryGetValue(key, out _))
            return this;

        this[key] = value;
        return this;
    }
}
