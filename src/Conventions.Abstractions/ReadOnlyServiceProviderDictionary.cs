using ReadOnlyPropertiesType = System.Collections.Generic.IReadOnlyDictionary<object, object>;
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;
using PropertiesDictionary = System.Collections.ObjectModel.ReadOnlyDictionary<object, object>;
using RealDictionary = System.Collections.Generic.Dictionary<object, object>;
using PropertiesKeyValuePair = System.Collections.Generic.KeyValuePair<object, object>;
using System.Collections;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     ServiceProviderDictionary.
///     Implements the <see cref="IServiceProviderDictionary" />
/// </summary>
/// <seealso cref="IServiceProviderDictionary" />
[PublicAPI]
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

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ( (IEnumerable)_values ).GetEnumerator();
    }

    /// <inheritdoc />
    public int Count => _values.Count;

    /// <inheritdoc />
    public bool ContainsKey(object key)
    {
        return _values.ContainsKey(key);
    }

    /// <inheritdoc />
    public bool TryGetValue(object key, out object value)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return _values.TryGetValue(key, out value!);
    }

    /// <inheritdoc />
    public object this[object key] => _values[key];

    /// <inheritdoc />
    public IEnumerable<object> Keys => _values.Keys;

    /// <inheritdoc />
    public IEnumerable<object> Values => _values.Values;

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
