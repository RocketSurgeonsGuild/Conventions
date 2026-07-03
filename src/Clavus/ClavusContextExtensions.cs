using Clavus.Infrastructure;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace Clavus;

/// <summary>
///     Base convention extensions
/// </summary>
[PublicAPI]
public static class ClavusContextExtensions
{
    /// <summary>
    ///     Set key to the value if the type is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="value">The value to save</param>
    public static IClavusContext AddIfMissing<T>(this IClavusContext context, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Properties.AddIfMissing(value);
        return context;
    }

    /// <summary>
    ///     Set key to the value if the key is missing
    /// </summary>
    /// <param name="context">The properties</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static IClavusContext AddIfMissing(this IClavusContext context, Type key, object value)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Properties.AddIfMissing(key, value);
        return context;
    }

    /// <summary>
    ///     Set key to the value if the key is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The properties</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static IClavusContext AddIfMissing<T>(this IClavusContext context, string key, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Properties.AddIfMissing(key, value);
        return context;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this IClavusContext context) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Properties.Get<T>();
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this IClavusContext context, string key) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Properties.Get<T>(key);
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static HostType GetHostType(this IClavusContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Properties.TryGetValue(typeof(HostType), out var hostType)
         && ( hostType is HostType ht || ( hostType is string str && Enum.TryParse(str, true, out ht) ) )
                ? ht
                : HostType.Undefined;
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    public static T GetOrAdd<T>(this IClavusContext context, Func<T> factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(factory);
        return context.Properties.GetOrAdd(factory);
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    public static T GetOrAdd<T>(this IClavusContext context, string key, Func<T> factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(factory);
        return context.Properties.GetOrAdd(key, factory);
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static bool IsUnitTestHost(this IClavusContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.GetHostType() == HostType.UnitTest;
    }

    /// <summary>
    ///     Register a set of conventions
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ValueTask RegisterConventions(this IClavusContext context, Action<CalvusExecutor> configure)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(configure);
        var executor = new CalvusExecutor(context);
        configure(executor);
        return executor.ExecuteAsync();
    }

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <returns>T.</returns>
    public static T Require<T>(this IClavusContext context)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Properties.TryGetValue(typeof(T), out var value) && value is T t
            ? t
            : throw new KeyNotFoundException($"The value of type {typeof(T).Name} was not found in the context");
    }

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T Require<T>(this IClavusContext context, string key)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Properties.TryGetValue(key, out var value) && value is T t
            ? t
            : throw new KeyNotFoundException($"The value of type {typeof(T).Name} with the {key} was not found in the context");
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="value">The value to save</param>
    public static IClavusContext Set<T>(this IClavusContext context, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(value);

        context.Properties.Set(value);
        return context;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static IClavusContext Set(this IClavusContext context, Type key, object value)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(key);

        context.Properties.Set(key, value);
        return context;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static IClavusContext Set<T>(this IClavusContext context, string key, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        context.Properties.Set(key, value);
        return context;
    }
}
