using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public static class Composer
    {
        class ComposerImpl<TContext, TContribution, TDelegate> : ConventionComposer<TContext, TContribution, TDelegate>
            where TContribution : IConvention<TContext>
            where TContext : IConventionContext
        { public ComposerImpl(IConventionScanner scanner, ILogger logger) : base(scanner, logger) { } }

        public static void Register<TContext, TContribution, TDelegate>(
            IConventionScanner scanner,
            ILogger logger,
            TContext context)
            where TContribution : IConvention<TContext>
            where TContext : IConventionContext
        {
            new ComposerImpl<TContext, TContribution, TDelegate>(scanner, logger).Register(context);
        }
    }
}