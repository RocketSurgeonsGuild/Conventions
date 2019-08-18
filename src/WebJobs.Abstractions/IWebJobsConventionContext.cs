using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.WebJobs
{
    /// <summary>
    ///  IWebJobsConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// Implements the <see cref="IWebJobsBuilder" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    /// <seealso cref="IWebJobsBuilder" />
    public interface IWebJobsConventionContext : IConventionContext, IWebJobsBuilder
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        IRocketEnvironment Environment { get; }
    }
}
