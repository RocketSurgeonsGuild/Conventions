#pragma warning disable CS8601 // Possible null reference assignment.

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Base convention extensions
/// </summary>
[PublicAPI]
public static class ConventionContextExtensions
{
    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this IConventionContext context) where T : class
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return (T?)context[typeof(T)];
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this IConventionContext context, string key) where T : class
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return (T?)context[key];
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    public static T GetOrAdd<T>(this IConventionContext context, Func<T> factory)
        where T : class
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        if (!( context[typeof(T)] is T value ))
        {
            value = factory();
            context.Set(value);
        }

        return value;
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="factory">The factory method in the event the type is not found</param>
    /// <returns>T.</returns>
    public static T GetOrAdd<T>(this IConventionContext context, string key, Func<T> factory)
        where T : class
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        if (!( context[key] is T value ))
        {
            value = factory();
            context.Set(value);
        }

        return value;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="value">The value to save</param>
    public static IConventionContext Set<T>(this IConventionContext context, T value)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context[typeof(T)] = value;
        return context;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static IConventionContext Set(this IConventionContext context, Type key, object value)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context[key] = value;
        return context;
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static IConventionContext Set<T>(this IConventionContext context, string key, T value)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context[key] = value;
        return context;
    }

    /// <summary>
    ///     Set key to the value if the type is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="value">The value to save</param>
    public static IConventionContext AddIfMissing<T>(this IConventionContext context, T value) where T : notnull
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        context.Properties.AddIfMissing(value);
        return context;
    }

    /// <summary>
    ///     Set key to the value if the key is missing
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The properties</param>
    /// <param name="key">The key where the value is saved</param>
    /// <param name="value">The value to save</param>
    public static IConventionContext AddIfMissing(this IConventionContext context, Type key, object value)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
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
    public static IConventionContext AddIfMissing<T>(this IConventionContext context, string key, T value) where T : notnull
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        context.Properties.AddIfMissing(key, value);
        return context;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static bool IsUnitTestHost(this IConventionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.GetHostType() == HostType.UnitTest;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static HostType GetHostType(this IConventionContext context)
    {
        return context.Properties.TryGetValue(typeof(HostType), out var hostType)
            // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
            ? (HostType)hostType!
            : HostType.Undefined;
    }
}