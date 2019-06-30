using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    ///  IServiceProviderDictionary
    /// Implements the <see cref="System.Collections.Generic.IDictionary{System.Object, System.Object}" />
    /// Implements the <see cref="System.IServiceProvider" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IDictionary{System.Object, System.Object}" />
    /// <seealso cref="System.IServiceProvider" />
    public interface IServiceProviderDictionary : IDictionary<object, object>, IServiceProvider { }
}
