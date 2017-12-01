using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A static composer that makes it easy to compose a context.
    /// </summary>
    public static class Composer
    {
        class ComposerImpl<TContext, TContribution, TDelegate> : ConventionComposer<TContext, TContribution, TDelegate>
            where TContribution : IConvention<TContext>
            where TContext : IConventionContext
        { public ComposerImpl(IConventionScanner scanner) : base(scanner) { } }

        /// <summary>
        /// Calls register on the any items found from the scanner that match either TContribution or TDelegate.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TContribution"></typeparam>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="scanner"></param>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public static void Register<TContext, TContribution, TDelegate>(
            IConventionScanner scanner,
            TContext context)
            where TContribution : IConvention<TContext>
            where TContext : IConventionContext
        {
            new ComposerImpl<TContext, TContribution, TDelegate>(scanner).Register(context);
        }
    }
}
