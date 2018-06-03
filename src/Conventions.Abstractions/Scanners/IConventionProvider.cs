using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Interface IConventionProvider
    /// </summary>
    /// TODO Edit XML Comment Template for IConventionProvider
    public interface IConventionProvider
    {
        /// <summary>
        /// Gets the conventions.
        /// </summary>
        /// <typeparam name="TContribution"></typeparam>
        /// <typeparam name="TDelegate"></typeparam>
        /// <returns>DelegateOrConvention&lt;TContribution, TDelegate&gt;</returns>
        /// TODO Edit XML Comment Template for Get`1
        IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>()
            where TContribution : IConvention
            where TDelegate : Delegate;

        /// <summary>
        /// Gets a all the conventions from the provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<DelegateOrConvention> GetAll();
    }
}
