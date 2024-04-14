#if NET7_0_OR_GREATER
using ReadOnlyPropertiesType = System.Collections.Generic.IReadOnlyDictionary<object, object>;
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;
using PropertiesDictionary = System.Collections.ObjectModel.ReadOnlyDictionary<object, object>;
using RealDictionary = System.Collections.Generic.Dictionary<object, object>;
using PropertiesKeyValuePair = System.Collections.Generic.KeyValuePair<object, object>;
#else
using ReadOnlyPropertiesType = System.Collections.Generic.IReadOnlyDictionary<object, object?>;
using PropertiesType = System.Collections.Generic.IDictionary<object, object?>;
using PropertiesDictionary = System.Collections.ObjectModel.ReadOnlyDictionary<object, object?>;
using RealDictionary = System.Collections.Generic.Dictionary<object, object?>;
using PropertiesKeyValuePair = System.Collections.Generic.KeyValuePair<object, object?>;
#endif

namespace Rocket.Surgery.Conventions;

/// <summary>
///     ServiceProviderDictionary.
///     Implements the <see cref="IServiceProviderDictionary" />
/// </summary>
/// <seealso cref="IServiceProviderDictionary" />
public class ReadOnlyServiceProviderDictionary : IReadOnlyServiceProviderDictionary
{
    private readonly ReadOnlyPropertiesType _values;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ReadOnlyServiceProviderDictionary(PropertiesType? values)
    {
        _values = new PropertiesDictionary(values ?? new RealDictionary());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
    /// </summary>
    public ReadOnlyServiceProviderDictionary()
    {
        _values = new RealDictionary();
    }

    /// <inheritdoc />
    public IEnumerator<PropertiesKeyValuePair> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return ( (System.Collections.IEnumerable)_values ).GetEnumerator();
    }

    /// <inheritdoc />
    public int Count => _values.Count;

    /// <inheritdoc />
    public bool ContainsKey(object key)
    {
        return _values.ContainsKey(key);
    }

    /// <inheritdoc />
#if NET7_0_OR_GREATER
    public bool TryGetValue(object key, out object value)
#else
    public bool TryGetValue(object key, out object? value)
#endif
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return _values.TryGetValue(key, out value!);
    }

    /// <inheritdoc />
#if NET7_0_OR_GREATER
    public object this[object key] => _values[key];
#else
    public object? this[object key] => _values[key];
#endif

    /// <inheritdoc />
    public IEnumerable<object> Keys => _values.Keys;

    /// <inheritdoc />

#if NET7_0_OR_GREATER
    public IEnumerable<object> Values => _values.Values;
#else
    public IEnumerable<object?> Values => _values.Values;
#endif

    /// <summary>
    ///     Gets the service.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>System.Object.</returns>
    public object? GetService(Type serviceType)
    {
        return _values.TryGetValue(serviceType, out var v) ? v : null;
    }
}
