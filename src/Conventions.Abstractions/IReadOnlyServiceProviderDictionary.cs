namespace Rocket.Surgery.Conventions;

/// <summary>
///     IServiceProviderDictionary
///     Implements the <see cref="IDictionary{TKey,TValue}" />
///     Implements the <see cref="IServiceProvider" />
/// </summary>
/// <seealso cref="IDictionary{Object, Object}" />
/// <seealso cref="IServiceProvider" />
#if NET7_0_OR_GREATER
public interface IReadOnlyServiceProviderDictionary : IReadOnlyDictionary<object, object>, IServiceProvider;
#else
public interface IReadOnlyServiceProviderDictionary : IReadOnlyDictionary<object, object?>, IServiceProvider;
#endif

