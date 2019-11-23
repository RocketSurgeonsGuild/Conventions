using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.WebJobs
{
    /// <summary>
    /// IWebJobsConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IWebJobsConventionContext}" />
    public interface IWebJobsConvention : IConvention<IWebJobsConventionContext> { }
}