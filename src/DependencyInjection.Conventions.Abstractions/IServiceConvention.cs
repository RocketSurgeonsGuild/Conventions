namespace Rocket.Surgery.Conventions.DependencyInjection
{
    /// <summary>
    /// IServiceConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IServiceConventionContext}" />
    public interface IServiceConvention : IConvention<IServiceConventionContext> { }
}