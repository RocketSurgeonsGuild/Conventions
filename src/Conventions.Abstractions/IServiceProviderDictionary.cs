using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// IServiceProviderDictionary
    /// Implements the <see cref="IDictionary{TKey,TValue}" />
    /// Implements the <see cref="IServiceProvider" />
    /// </summary>
    /// <seealso cref="IDictionary{Object, Object}" />
    /// <seealso cref="IServiceProvider" />
    public interface IServiceProviderDictionary : IDictionary<object, object?>, IServiceProvider { }
}