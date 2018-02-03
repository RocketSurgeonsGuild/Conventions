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
    public abstract class ConventionComposerBase
    {
        protected void ExecuteRegister(IConventionContext context, List<DelegateOrConvention> items, IEnumerable<Type> types)
        {
            var enumerable = types as Type[] ?? types.ToArray();
            if (enumerable.Length == 0) return;

            var delegateTypes = enumerable.Where(typeof(Delegate).IsAssignableFrom).ToArray();
            var conventionTypes = enumerable.Except(delegateTypes).ToArray();

            context.Logger.LogDebug("Found {Count} conventions or delegates", items.Count);

            foreach (var itemWithIndex in items.Select((item, index) => new { item, index }))
            {
                var item = itemWithIndex.item;
                try
                {

                    if (item.Convention != null)
                    {
                        if (!conventionTypes.Any(type => type.IsInstanceOfType(item.Convention)))
                        {
                            context.Logger.LogDebug("Could not execute Convention {TypeName} from {AssemblyName} :: {@Types}",
                                item.Convention?.GetType()?.FullName,
                                item.Convention?.GetType()?.GetTypeInfo()?.Assembly?.GetName()?.Name,
                                enumerable);
                            continue;
                        }

                        context.Logger.LogDebug("Executing Convention {TypeName} from {AssemblyName}",
                            item.Convention?.GetType()?.FullName,
                            item.Convention?.GetType()?.GetTypeInfo()?.Assembly?.GetName()?.Name);
                        Register(item.Convention, context);
                        continue;
                    }

                    // ReSharper disable once UseNullPropagation
                    // ReSharper disable once InvertIf
                    if (item.Delegate != null)
                    {
                        if (!delegateTypes.Any(type => type.IsInstanceOfType(item.Delegate)))
                        {
                            context.Logger.LogDebug("Could not execute Delegate {TypeName} :: {@Types}",
                                item.Delegate.GetType()?.FullName,
                                enumerable);
                            continue;
                        }

                        context.Logger.LogDebug("Executing Delegate {TypeName}", item.Delegate.GetType()?.FullName);
                        item.Delegate.DynamicInvoke(context);
                        continue;
                    }

                    context.Logger.LogError("Convention or Delegate not available for one of the items");
                }
                catch (Exception e)
                {
                    if (item.Convention != null)
                        context.Logger.LogError(0, e, "Error invoking Convention {Convention}", item.Convention?.GetType()?.FullName);
                    else if (item.Delegate != null)
                        context.Logger.LogError(0, e, "Error invoking Delegate at index {Index}", itemWithIndex.index);
                    else
                        context.Logger.LogError("Unknown error invoking Convention or Delegate at index {Index}", itemWithIndex.index);
                    throw;
                }
            }
        }

        private readonly ConcurrentDictionary<Type, MethodInfo> _registerMethodCache = new ConcurrentDictionary<Type, MethodInfo>();

        private static readonly MethodInfo RegisterGenericMethod =
            typeof(ConventionComposerBase).GetTypeInfo().GetDeclaredMethod(nameof(RegisterGeneric));

        private void Register(IConvention convention, IConventionContext context)
        {
            var interfaces = convention.GetType().GetTypeInfo().ImplementedInterfaces
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetTypeInfo().GetGenericTypeDefinition() == typeof(IConvention<>))
                .Select(x => new { interfaceType = x, contextType = x.GetTypeInfo().GenericTypeArguments[0] });

            var contextTypes = context.GetType().GetTypeInfo().ImplementedInterfaces
                .Where(x => typeof(IConventionContext).IsAssignableFrom(x));

            var typesToRegister = interfaces
                .Join(contextTypes, x => x.contextType, x => x, (interfaceType, contextType) => contextType);

            foreach (var item in typesToRegister)
            {
                if (!_registerMethodCache.TryGetValue(item, out var method))
                {
                    method = RegisterGenericMethod.MakeGenericMethod(item);
                    _registerMethodCache.TryAdd(item, method);
                }

                method.Invoke(this, new object[] { convention, context });
            }
        }

        private void RegisterGeneric<T>(IConvention<T> convention, T context)
            where T : IConventionContext
        {
            convention.Register(context);
        }
    }


    /// <summary>
    /// Convention base compose, that calls all methods on register.
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    /// <typeparam name="TContribution">The contribution type</typeparam>
    /// <typeparam name="TDelegate">The delegate type</typeparam>
    public abstract class ConventionComposer<TContext, TContribution, TDelegate> : ConventionComposerBase, IConventionComposer<TContext, TContribution, TDelegate>
        where TContribution : IConvention<TContext>
        where TContext : IConventionContext
    {
        private readonly IConventionScanner _scanner;

        /// <summary>
        /// A base compose that does the composing of conventions and delegates
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="logger"></param>
        protected ConventionComposer(IConventionScanner scanner)
        {
            if (!typeof(Delegate).GetTypeInfo().IsAssignableFrom(typeof(TDelegate).GetTypeInfo()))
                throw new ArgumentException("TDelegate is not a Delegate");
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        /// <inheritdoc />
        public void Register(TContext context)
        {
            var items = _scanner.BuildProvider()
                .Get<TContribution, TDelegate>()
                .ToList();

            ExecuteRegister(context, items, new[] { typeof(TContribution), typeof(TDelegate) });
        }
    }

    /// <summary>
    /// Convention base compose, that calls all methods on register.
    /// </summary>
    public class ConventionComposer : ConventionComposerBase, IConventionComposer
    {
        private readonly IConventionScanner _scanner;

        /// <summary>
        /// A base compose that does the composing of conventions and delegates
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="logger"></param>
        public ConventionComposer(IConventionScanner scanner)
        {
            _scanner = scanner;
        }

        /// <inheritdoc />
        public void Register(IConventionContext context, IEnumerable<Type> types)
        {
            var items = _scanner.BuildProvider().GetAll().ToList();
            ExecuteRegister(context, items, types);
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
