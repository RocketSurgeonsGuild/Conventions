#pragma warning disable IDE0058 // Expression value is never used
using System.Collections;
using PropertiesType = System.Collections.Generic.IDictionary<object, object>;
using PropertiesDictionary = System.Collections.Generic.Dictionary<object, object>;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     ServiceProviderDictionary.
///     Implements the <see cref="IServiceProviderDictionary" />
/// </summary>
/// <seealso cref="IServiceProviderDictionary" />
[PublicAPI]
public class ServiceProviderDictionary : IServiceProviderDictionary, IReadOnlyServiceProviderDictionary
{
    private readonly PropertiesType _values;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ServiceProviderDictionary(PropertiesType? values)
    {
        _values = values ?? new PropertiesDictionary();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
    /// </summary>
    public ServiceProviderDictionary()
    {
        _values = new PropertiesDictionary();
    }

    IEnumerable<object> IReadOnlyDictionary<object, object>.Keys => Keys;

    IEnumerable<object> IReadOnlyDictionary<object, object>.Values => Values;

    /// <summary>
    ///     Gets or sets the <see cref="object" /> with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.Object.</returns>
    public object this[object key]
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        get => _values.TryGetValue(key, out var b) ? b : null!;
        set => _values[key] = value;
    }

    /// <summary>
    ///     Gets the keys.
    /// </summary>
    /// <value>The keys.</value>
    public ICollection<object> Keys => _values.Keys;

    /// <summary>
    ///     Gets the values.
    /// </summary>
    /// <value>The values.</value>
    public ICollection<object> Values => _values.Values;

    /// <summary>
    ///     Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count => _values.Count;

    /// <summary>
    ///     Gets a value indicating whether this instance is read only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => false;

    /// <summary>
    ///     Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(object key, object value)
    {
        _values[key] = value;
    }

    /// <summary>
    ///     Adds the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(KeyValuePair<object, object> item)
    {
        _values.Add(item.Key, item.Value);
    }

    /// <summary>
    ///     Clears this instance.
    /// </summary>
    public void Clear()
    {
        _values.Clear();
    }

    /// <summary>
    ///     Determines whether this instance contains the object.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
    public bool Contains(KeyValuePair<object, object> item)
    {
        return _values.Contains(item);
    }

    /// <summary>
    ///     Determines whether the specified key contains key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(object key)
    {
        return _values.ContainsKey(key);
    }

    /// <summary>
    ///     Copies to.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
    {
        _values.CopyTo(array, arrayIndex);
    }

    /// <summary>
    ///     Gets the enumerator.
    /// </summary>
    /// <returns>IEnumerator{KeyValuePair{System.Object, System.Object}}.</returns>
    public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    /// <summary>
    ///     Removes the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Remove(object key)
    {
        return _values.Remove(key);
    }

    /// <summary>
    ///     Removes the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool Remove(KeyValuePair<object, object> item)
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        return _values.Remove(item);
    }

    /// <summary>
    ///     Tries the get value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool TryGetValue(object key, out object value)
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        return _values.TryGetValue(key, out value!);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

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