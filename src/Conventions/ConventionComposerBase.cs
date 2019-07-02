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
    /// ConventionComposerBase.
    /// </summary>
    public abstract class ConventionComposerBase
    {
        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="items">The items.</param>
        /// <param name="types">The types.</param>
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
                                item.Convention?.GetType().FullName,
                                item.Convention?.GetType().GetTypeInfo().Assembly.GetName().Name,
                                enumerable);
                            continue;
                        }

                        context.Logger.LogDebug("Executing Convention {TypeName} from {AssemblyName}",
                            item.Convention?.GetType().FullName,
                            item.Convention?.GetType().GetTypeInfo().Assembly.GetName().Name);
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
                                item.Delegate.GetType().FullName,
                                enumerable);
                            continue;
                        }

                        context.Logger.LogDebug("Executing Delegate {TypeName}", item.Delegate.GetType().FullName);
                        item.Delegate.DynamicInvoke(context);
                        continue;
                    }

                    context.Logger.LogError("Convention or Delegate not available for one of the items");
                }
                catch (Exception e)
                {
                    if (item.Convention != null)
                        context.Logger.LogError(0, e, "Error invoking Convention {Convention}", item.Convention.GetType().FullName);
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
}
