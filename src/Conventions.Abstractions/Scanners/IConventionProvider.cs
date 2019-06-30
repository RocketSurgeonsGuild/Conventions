using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    ///  IConventionProvider
    /// </summary>
    public interface IConventionProvider
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TContribution">The type of the contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <returns>IEnumerable&lt;DelegateOrConvention&gt;.</returns>
        IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>()
            where TContribution : IConvention
            where TDelegate : Delegate;

        /// <summary>
        /// Gets a all the conventions from the provider
        /// </summary>
        /// <returns>IEnumerable&lt;DelegateOrConvention&gt;.</returns>
        IEnumerable<DelegateOrConvention> GetAll();
    }
}
