using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    ///  ServiceProviderDictionary.
    /// Implements the <see cref="IServiceProviderDictionary" />
    /// </summary>
    /// <seealso cref="IServiceProviderDictionary" />
    public class ServiceProviderDictionary : IServiceProviderDictionary
    {
        private readonly IDictionary<object, object> _values = new Dictionary<Object, object>();
        private readonly IDictionary<Type, object> _services = new Dictionary<Type, object>();
        /// <summary>
        /// Gets or sets the <see cref="object"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Object.</returns>
        public object this[object key]
        {
            get => key is Type t && _services.TryGetValue(t, out var v) ? v : _values.TryGetValue(key, out var b) ? b : null;
            set
            {
                if (key is Type t)
                {
                    _services[t] = value;
                }
                else
                {
                    _values[key] = value;
                }
            }
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<object> Keys => _values.Keys.Concat(_services.Keys).ToList();

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<object> Values => _values.Values.Concat(_services.Values).ToList();

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => _values.Count + _services.Count;

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(object key, object value)
        {
            if (key is Type t)
            {
                _services.Add(t, value);
            }
            else
            {
                _values.Add(key, value);
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<object, object> item)
        {
            if (item.Key is Type t)
            {
                _services.Add(t, item.Value);
            }
            else
            {
                _values.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _services.Clear();
            _values.Clear();
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(KeyValuePair<object, object> item)
        {
            if (item.Key is Type t)
            {
                return _services.Contains(new KeyValuePair<Type, object>(t, item.Value));
            }
            else
            {
                return _values.Contains(item);
            }
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(object key)
        {
            if (key is Type t)
            {
                return _services.ContainsKey(t);
            }
            else
            {
                return _values.ContainsKey(key);
            }
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="NotSupportedException">Copy to is not supported</exception>
        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            throw new NotSupportedException("Copy to is not supported ");
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator{KeyValuePair{System.Object, System.Object}}.</returns>
        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return _values.Concat(_services.Select(z => new KeyValuePair<Object, object>(z.Key, z.Value))).GetEnumerator();
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(object key)
        {
            if (key is Type t)
            {
                return _services.Remove(t);
            }
            else
            {
                return _values.Remove(key);
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(KeyValuePair<object, object> item)
        {
            if (item.Key is Type t)
            {
                return _services.Remove(new KeyValuePair<Type, object>(t, item.Value));
            }
            else
            {
                return _values.Remove(item);
            }
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGetValue(object key, out object value)
        {
            if (key is Type t)
            {
                return _services.TryGetValue(t, out value);
            }
            else
            {
                return _values.TryGetValue(key, out value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>System.Object.</returns>
        public object GetService(Type serviceType)
        {
            return _services.TryGetValue(serviceType, out var v) ? v : null;
        }
    }
}
