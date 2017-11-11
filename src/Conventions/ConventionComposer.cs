using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
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

    /// <summary>
    /// Convention base compose, that calls all methods on register.
    /// </summary>
    public class ConventionComposer : IConventionComposer
    {
        private readonly ILogger _logger;
        private readonly IConventionScanner _scanner;

        /// <summary>
        /// A base compose that does the composing of conventions and delegates
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="logger"></param>
        public ConventionComposer(IConventionScanner scanner, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        public void Register(IConventionContext context, Type type, params Type[] types)
        {
            Register(context, new[] { type }.Concat(types));
        }

        public void Register(IConventionContext context, IEnumerable<Type> types)
        {
            var items = _scanner.BuildProvider().GetAll().ToList();

            var enumerable = types as Type[] ?? types.ToArray();
            if (enumerable.Length == 0) return;

            var delegateTypes = enumerable.Where(typeof(Delegate).IsAssignableFrom).ToArray();
            var conventionTypes = enumerable.Except(delegateTypes).ToArray();

            _logger.LogInformation("Found {Count} conventions or delegates", items.Count);

            foreach (var item in items)
            {
                if (item == DelegateOrConvention.None)
                {
                    _logger.LogError("Convention or Delegate not available for one of the items");
                    continue;
                }

                if (item.Convention != null)
                {
                    if (!conventionTypes.Any(type => type.IsInstanceOfType(item.Convention))) continue;

                    _logger.LogDebug("Executing Convention {TypeName} from {AssemblyName}", item.Convention.GetType().FullName, item.Convention.GetType().GetTypeInfo().Assembly.GetName().Name);
                    Register(item.Convention, context);
                }

                // ReSharper disable once UseNullPropagation
                // ReSharper disable once InvertIf
                if (item.Delegate != null)
                {
                    if (!delegateTypes.Any(type => type.IsInstanceOfType(item.Delegate))) continue;

                    _logger.LogDebug("Executing Delegate {TypeName}", item.Delegate.GetType().FullName);
                    item.Delegate.DynamicInvoke(context);
                }
            }
        }

        private readonly ConcurrentDictionary<Type, MethodInfo> _registerMethodCache = new ConcurrentDictionary<Type, MethodInfo>();

        private void Register(IConvention convention, IConventionContext context)
        {
            if (!_registerMethodCache.TryGetValue(convention.GetType(), out var method))
            {
                method = convention.GetType().GetTypeInfo().GetDeclaredMethod(nameof(Register));
                _registerMethodCache.TryAdd(convention.GetType(), method);
            }
            method.Invoke(convention, new object[] { context });
        }
    }
}
