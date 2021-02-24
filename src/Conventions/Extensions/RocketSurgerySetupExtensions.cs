using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.Conventions.Extensions
{
    /// <summary>
    /// Extension method to apply configuration conventions
    /// </summary>
    internal static class RocketSurgerySetupExtensions
    {
        /// <summary>
        /// Apply configuration conventions
        /// </summary>
        /// <param name="conventionContext"></param>
        /// <returns></returns>
        public static IConventionContext ApplyConventions(this IConventionContext conventionContext)
        {
            foreach (var item in conventionContext.Conventions.Get<ISetupConvention, SetupConvention>())
            {
                if (item is ISetupConvention convention)
                {
                    convention.Register(conventionContext);
                }
                else if (item is SetupConvention @delegate)
                {
                    @delegate(conventionContext);
                }
            }

            return conventionContext;
        }
    }
}