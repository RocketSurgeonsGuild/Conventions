using System;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A static composer that makes it easy to compose a context.
    /// </summary>
    public static class Composer
    {
        /// <summary>
        /// ComposerImpl.
        /// Implements the <see cref="ConventionComposer{TContext, TContribution, TDelegate}" />
        /// </summary>
        /// <typeparam name="TContext">The type of the t context.</typeparam>
        /// <typeparam name="TContribution">The type of the t contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
        /// <seealso cref="ConventionComposer{TContext, TContribution, TDelegate}" />
        class ComposerImpl<TContext, TContribution, TDelegate> : ConventionComposer<TContext, TContribution, TDelegate>
            where TContribution : IConvention<TContext>
            where TContext : IConventionContext
            where TDelegate : Delegate
        { public ComposerImpl(IConventionScanner scanner) : base(scanner) { } }

        /// <summary>
        /// Calls register on the any items found from the scanner that match either TContribution or TDelegate.
        /// </summary>
        /// <typeparam name="TContext">The type of the t context.</typeparam>
        /// <typeparam name="TContribution">The type of the t contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
        /// <param name="scanner">The scanner.</param>
        /// <param name="context">The context.</param>
        public static void Register<TContext, TContribution, TDelegate>(
            IConventionScanner scanner,
            TContext context)
            where TContribution : IConvention<TContext>
            where TContext : IConventionContext
            where TDelegate : Delegate
        {
            new ComposerImpl<TContext, TContribution, TDelegate>(scanner).Register(context);
        }
    }
}
