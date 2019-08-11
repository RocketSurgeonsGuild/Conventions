using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.WebJobs
{
    /// <summary>
    /// IWebJobsConventionBuilder.
    /// Implements the <see cref="IConventionBuilder{IWebJobsConventionBuilder, IWebJobsConvention, WebJobsConventionDelegate}" />
    /// Implements the <see cref="IWebJobsConventionBuilder" />
    /// Implements the <see cref="IWebJobsConvention" />
    /// Implements the <see cref="IWebJobsConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{IWebJobsConventionBuilder, IWebJobsConvention, WebJobsConventionDelegate}" />
    /// <seealso cref="IWebJobsConventionBuilder" />
    /// <seealso cref="IWebJobsConvention" />
    /// <seealso cref="IWebJobsConventionContext" />
    public interface IWebJobsConventionBuilder : IConventionBuilder<IWebJobsConventionBuilder, IWebJobsConvention, WebJobsConventionDelegate>, IWebJobsConventionContext
    {
        /// <summary>
        /// Build the service provider from this container
        /// </summary>
        void Build();
    }
}
