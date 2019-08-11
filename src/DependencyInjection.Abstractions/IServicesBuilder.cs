using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    /// IServicesBuilder.
    /// Implements the <see cref="IConventionBuilder{IServicesBuilder, IServiceConvention, ServiceConventionDelegate}" />
    /// Implements the <see cref="IServiceConvention" />
    /// Implements the <see cref="IServiceConventionContext" />
    /// Implements the <see cref="ServiceConventionDelegate" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{IServicesBuilder, IServiceConvention, ServiceConventionDelegate}" />
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConventionContext" />
    /// <seealso cref="ServiceConventionDelegate" />
    public interface IServicesBuilder : IConventionBuilder<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>, IServiceConventionContext
    {
        /// <summary>
        /// Build the service provider from this container
        /// </summary>
        /// <returns>IServiceProvider.</returns>
        IServiceProvider Build();
    }
}
