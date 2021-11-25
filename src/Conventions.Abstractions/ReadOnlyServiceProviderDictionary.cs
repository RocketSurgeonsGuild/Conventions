using System.Collections.ObjectModel;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     ServiceProviderDictionary.
///     Implements the <see cref="IServiceProviderDictionary" />
/// </summary>
/// <seealso cref="IServiceProviderDictionary" />
public class ReadOnlyServiceProviderDictionary : IReadOnlyServiceProviderDictionary
{
    private readonly IReadOnlyDictionary<object, object?> _values;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ReadOnlyServiceProviderDictionary(IDictionary<object, object?>? values)
    {
        _values = new ReadOnlyDictionary<object, object?>(values ?? new Dictionary<object, object?>());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
    /// </summary>
    public ReadOnlyServiceProviderDictionary()
    {
        _values = new Dictionary<object, object?>();
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<object, object?>> GetEnumerator()
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
    public bool TryGetValue(object key, out object? value)
    {
        return _values.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public object? this[object key] => _values[key];

    /// <inheritdoc />
    public IEnumerable<object> Keys => _values.Keys;

    /// <inheritdoc />
    public IEnumerable<object?> Values => _values.Values;

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
