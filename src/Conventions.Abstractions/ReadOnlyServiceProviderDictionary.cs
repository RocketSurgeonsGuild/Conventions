using System.Collections;
using PropertiesDictionary = System.Collections.ObjectModel.ReadOnlyDictionary<object, object>;
using PropertiesKeyValuePair = System.Collections.Generic.KeyValuePair<object, object>;
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;
using ReadOnlyPropertiesType = System.Collections.Generic.IReadOnlyDictionary<object, object>;
using RealDictionary = System.Collections.Generic.Dictionary<object, object>;

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
    public ReadOnlyServiceProviderDictionary(PropertiesType? values) => _values = new PropertiesDictionary(values ?? new RealDictionary());

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
    /// </summary>
    public ReadOnlyServiceProviderDictionary() => _values = new RealDictionary();

    /// <inheritdoc />
    public IEnumerator<PropertiesKeyValuePair> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_values ).GetEnumerator();

    /// <inheritdoc />
    public int Count => _values.Count;

    /// <inheritdoc />
    public bool ContainsKey(object key) => _values.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(object key, out object value) =>
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        _values.TryGetValue(key, out value!);

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
    public object? GetService(Type serviceType) => _values.TryGetValue(serviceType, out var v) ? v : null;
}
