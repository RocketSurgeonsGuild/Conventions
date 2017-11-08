using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Convention base compose, that calls all methods on register.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TContribution"></typeparam>
    /// <typeparam name="TDelegate"></typeparam>
    public abstract class ConventionComposer<TContext, TContribution, TDelegate> : IConventionComposer<TContext, TContribution, TDelegate>
        where TContribution : IConvention<TContext>
        where TContext : IConventionContext
    {
        private readonly ILogger _logger;
        private readonly IConventionScanner _scanner;

        /// <summary>
        /// A base compose that does the composing of conventions and delegates
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="logger"></param>
        protected ConventionComposer(IConventionScanner scanner, ILogger logger)
        {
            if (!typeof(Delegate).GetTypeInfo().IsAssignableFrom(typeof(TDelegate).GetTypeInfo()))
                throw new ArgumentException("TDelegate is not a Delegate");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        /// <inheritdoc cref="IConventionComposerventionComposer{TContext,TContribution,TDelegate}"/>
        public void Register(TContext context)
        {
            var items = _scanner.BuildProvider()
                .Get<TContribution, TDelegate>()
                .ToList();

            _logger.LogInformation("Found {Count} conventions or delegates of {Type} for {Convention}", items.Count, typeof(TDelegate).FullName, typeof(TContribution).FullName);

            foreach (var item in items)
            {
                if (item == DelegateOrConvention<TContribution, TDelegate>.None)
                {
                    _logger.LogError("Convention or Delege not available for one of the items from {Type}", typeof(TContribution).FullName);
                    continue;
                }

                if (item.Convention != null)
                {
                    _logger.LogDebug("Executing Convention {TypeName} from {AssemblyName}", item.Convention.GetType().FullName, item.Convention.GetType().GetTypeInfo().Assembly.GetName().Name);
                    item.Convention.Register(context);
                }

                // ReSharper disable once UseNullPropagation
                // ReSharper disable once InvertIf
                if (item.Delegate != null)
                {
                    _logger.LogDebug("Executing Delegate {TypeName}", typeof(TDelegate).FullName);
                    item.Delegate.DynamicInvoke(context);
                }
            }
        }
    }
}
