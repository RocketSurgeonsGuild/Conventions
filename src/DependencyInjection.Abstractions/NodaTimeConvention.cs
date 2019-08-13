using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.TimeZones;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.DependencyInjection;

[assembly: Convention(typeof(NodaTimeConvention))]

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    /// NodaTimeConvention.
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    public class NodaTimeConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IServiceConventionContext context)
        {
            context.Services.AddSingleton<IClock>(SystemClock.Instance);
            context.Services.AddSingleton<IDateTimeZoneProvider, DateTimeZoneCache>();
            context.Services.AddSingleton<IDateTimeZoneSource>(TzdbDateTimeZoneSource.Default);
        }
    }
}
