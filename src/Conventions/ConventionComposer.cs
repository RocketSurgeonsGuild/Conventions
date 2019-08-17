using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Convention base compose, that calls all methods on register.
    /// Implements the <see cref="IConventionComposer{TContext, TContribution, TDelegate}" />
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    /// <typeparam name="TContribution">The contribution type</typeparam>
    /// <typeparam name="TDelegate">The delegate type</typeparam>
    /// <seealso cref="IConventionComposer{TContext, TContribution, TDelegate}" />
    [Obsolete("This class will be removed in the future version.  Replaced by Composer.Register<TContext, TContribution, TDelegate>.")]
    public abstract class ConventionComposer<TContext, TContribution, TDelegate> : IConventionComposer<TContext, TContribution, TDelegate>
        where TContribution : IConvention<TContext>
        where TContext : IConventionContext
        where TDelegate : Delegate
    {
        private readonly IConventionScanner _scanner;

        /// <summary>
        /// A base compose that does the composing of conventions and delegates
        /// </summary>
        /// <param name="scanner"></param>
        protected ConventionComposer(IConventionScanner scanner)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <inheritdoc />
        public void Register(TContext context)
        {
            Composer.Register<TContext, TContribution, TDelegate>(_scanner.BuildProvider(), context);
        }
    }

    /// <summary>
    /// Convention base compose, that calls all methods on register.
    /// Implements the <see cref="IConventionComposer{TContext, TContribution, TDelegate}" />
    /// </summary>
    /// <seealso cref="IConventionComposer{TContext, TContribution, TDelegate}" />
    [Obsolete("This class will be removed in the future version.  Replaced by Composer.Register<TContext, TContribution, TDelegate>.")]
    public class ConventionComposer : IConventionComposer
    {
        private readonly IConventionScanner _scanner;

        /// <summary>
        /// A base compose that does the composing of conventions and delegates
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        [Obsolete("Convention Composer will be come a static class in a future version")]
        public ConventionComposer(IConventionScanner scanner)
        {
            _scanner = scanner;
        }

        /// <summary>
        /// Uses all the conventions and calls the register method for all of them.
        /// </summary>
        /// <param name="context">The valid context for the types</param>
        /// <param name="types">The types to compose with.  This type will either be a <see cref="T:System.Delegate" /> that takes <see cref="T:Rocket.Surgery.Conventions.IConventionContext" />, or a type that implements <see cref="T:Rocket.Surgery.Conventions.IConvention`1" /></param>
        /// <inheritdoc />
        public void Register(IConventionContext context, IEnumerable<Type> types)
        {
            Composer.Register(_scanner.BuildProvider(), context, types);
        }
    }
}
