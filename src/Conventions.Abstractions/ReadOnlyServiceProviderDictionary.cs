using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ServiceProviderDictionary.
    /// Implements the <see cref="IServiceProviderDictionary" />
    /// </summary>
    /// <seealso cref="IServiceProviderDictionary" />
    public class ReadOnlyServiceProviderDictionary : IReadOnlyServiceProviderDictionary
    {
        private readonly IReadOnlyDictionary<object, object?> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public ReadOnlyServiceProviderDictionary(IDictionary<object, object?>? values)
            => _values = new ReadOnlyDictionary<object, object?>(values ?? new Dictionary<object, object?>());

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
        /// </summary>
        public ReadOnlyServiceProviderDictionary() => _values = new Dictionary<object, object?>();

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<object, object?>> GetEnumerator() => _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_values ).GetEnumerator();

        /// <inheritdoc />
        public int Count => _values.Count;

        /// <inheritdoc />
        public bool ContainsKey(object key) => _values.ContainsKey(key);

        /// <inheritdoc />
        public bool TryGetValue(object key, out object? value) => _values.TryGetValue(key, out value);

        /// <inheritdoc />
        public object? this[object key] => _values[key];

        /// <inheritdoc />
        public IEnumerable<object> Keys => _values.Keys;

        /// <inheritdoc />
        public IEnumerable<object?> Values => _values.Values;

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>System.Object.</returns>
        public object? GetService(Type serviceType) => _values.TryGetValue(serviceType, out var v) ? v : null;
    }
}