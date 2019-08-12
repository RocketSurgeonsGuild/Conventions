using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    ///  IServiceConvention
    /// Implements the <see cref="IConvention{IServiceConventionContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IServiceConventionContext}" />
    public interface IServiceConvention : IConvention<IServiceConventionContext>
    {
    }
}
