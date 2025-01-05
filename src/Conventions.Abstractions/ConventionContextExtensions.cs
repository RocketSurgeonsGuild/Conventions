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
    public static T? Get<T>(this IConventionContext context) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Properties.Get<T>();
    }

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <returns>T.</returns>
    public static T Require<T>(this IConventionContext context)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Properties.TryGetValue(typeof(T), out var value) && value is T t
            ? t
            : throw new KeyNotFoundException($"The value of type {typeof(T).Name} was not found in the context");
    }

    /// <summary>
    ///     Get a value by key from the context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T? Get<T>(this IConventionContext context, string key) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Properties.Get<T>(key);
    }

    /// <summary>
    ///     Get a value by type from the context or throw
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The context</param>
    /// <param name="key">The key where the value is saved</param>
    /// <returns>T.</returns>
    public static T Require<T>(this IConventionContext context, string key)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Properties.TryGetValue(key, out var value) && value is T t
            ? t
            : throw new KeyNotFoundException($"The value of type {typeof(T).Name} with the {key} was not found in the context");
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
    public static T GetOrAdd<T>(this IConventionContext context, string key, Func<T> factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(factory);
        return context.Properties.GetOrAdd(key, factory);
    }

    /// <summary>
    ///     Get a value by type from the context
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="context">The context</param>
    /// <param name="value">The value to save</param>
    public static IConventionContext Set<T>(this IConventionContext context, T value) where T : notnull
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
    public static IConventionContext Set(this IConventionContext context, Type key, object value)
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
    public static IConventionContext Set<T>(this IConventionContext context, string key, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        context.Properties.Set(key, value);
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
    public static IConventionContext AddIfMissing(this IConventionContext context, Type key, object value)
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
    public static IConventionContext AddIfMissing<T>(this IConventionContext context, string key, T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Properties.AddIfMissing(key, value);
        return context;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static bool IsUnitTestHost(this IConventionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.GetHostType() == HostType.UnitTest;
    }

    /// <summary>
    ///     Check if this is a test host (to allow conventions to behave differently during unit tests)
    /// </summary>
    /// <param name="context">The context</param>
    public static HostType GetHostType(this IConventionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Properties.TryGetValue(typeof(HostType), out var hostType)
         && ( hostType is HostType ht || ( hostType is string str && Enum.TryParse(str, true, out ht) ) )
                ? ht
                : HostType.Undefined;
    }

    /// <summary>
    /// Register a set of conventions
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ValueTask RegisterConventions(this IConventionContext context, Action<ConventionExecutor> configure)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(configure);
        var executor = new ConventionExecutor(context);
        configure(executor);
        return executor.ExecuteAsync();
    }
}

/// <summary>
/// A class to help with executing conventions
/// </summary>
/// <remarks>
/// This class uses <see cref="ConventionExceptionPolicyDelegate"/> to handle exceptions
/// </remarks>
/// <param name="context"></param>
public class ConventionExecutor(IConventionContext context)
{
    private readonly List<Action<object>> _conventionHandlers = [];
    private readonly List<Func<object, ValueTask>> _asyncConventionHandlers = [];

    /// <summary>
    /// Add a synchronous convention
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="TConvention"></typeparam>
    /// <returns></returns>
    public ConventionExecutor AddHandler<TConvention>(Action<TConvention> action)
    {
        _conventionHandlers.Add(
            o =>
            {
                if (o is not TConvention convention) return;
                try
                {
                    action(convention);
                }
                catch (Exception ex) when (!context.ExceptionPolicy(ex))
                {
                    throw;
                }
            }
        );
        return this;
    }

    /// <summary>
    /// Add an asynchronous convention
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="TConvention"></typeparam>
    /// <returns></returns>
    public ConventionExecutor AddHandler<TConvention>(Func<TConvention, ValueTask> action)
    {
        _asyncConventionHandlers.Add(
            async o =>
            {
                if (o is not TConvention convention) return;
                try
                {
                    await action(convention).ConfigureAwait(false);
                }
                catch (Exception ex) when (!context.ExceptionPolicy(ex))
                {
                    throw;
                }
            }
        );
        return this;
    }

    /// <summary>
    /// Run all the conventions
    /// </summary>
    public async ValueTask ExecuteAsync()
    {
        foreach (var convention in context.Conventions.GetAll())
        {
            foreach (var handler in _conventionHandlers)
            {
                handler(convention);
            }

            foreach (var handler in _asyncConventionHandlers)
            {
                await handler(convention).ConfigureAwait(false);
            }
        }
    }
}
