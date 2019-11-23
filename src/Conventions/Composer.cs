using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;

#pragma warning disable IDE0058 // Expression value is never used

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A static composer that makes it easy to compose a context.
    /// </summary>
    public static class Composer
    {
        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="scanner">The provider.</param>
        /// <param name="context">The context.</param>
        public static void Register<TContext, TContribution, TDelegate>(
            [NotNull] IConventionScanner scanner,
            [NotNull] IConventionContext context
        )
            where TContext : IConventionContext
            where TContribution : IConvention<TContext>
            where TDelegate : Delegate
        {
            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Register(
                context,
                scanner.BuildProvider().Get<TContribution, TDelegate>(context.GetHostType()),
                new[] { typeof(TContribution), typeof(TDelegate) }
            );
        }

        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="context">The context.</param>
        public static void Register<TContext, TContribution, TDelegate>(
            [NotNull] IConventionProvider provider,
            [NotNull] IConventionContext context
        )
            where TContext : IConventionContext
            where TContribution : IConvention<TContext>
            where TDelegate : Delegate
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Register(
                context,
                provider.Get<TContribution, TDelegate>(context.GetHostType()),
                new[] { typeof(TContribution), typeof(TDelegate) }
            );
        }

        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="context">The context.</param>
        /// <param name="types">The types.</param>
        public static void Register(
            [NotNull] IConventionScanner scanner,
            [NotNull] IConventionContext context,
            IEnumerable<Type> types
        )
        {
            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Register(context, scanner.BuildProvider().GetAll(context.GetHostType()), types);
        }

        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="context">The context.</param>
        /// <param name="types">The types.</param>
        public static void Register(
            [NotNull] IConventionProvider provider,
            [NotNull] IConventionContext context,
            IEnumerable<Type> types
        )
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Register(context, provider.GetAll(context.GetHostType()), types);
        }

        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="context">The context.</param>
        /// <param name="types">The types.</param>
        public static void Register(
            [NotNull] IConventionScanner scanner,
            [NotNull] IConventionContext context,
            params Type[] types
        )
        {
            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Register(context, scanner.BuildProvider().GetAll(context.GetHostType()), types);
        }

        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="context">The context.</param>
        /// <param name="types">The types.</param>
        public static void Register(
            [NotNull] IConventionProvider provider,
            [NotNull] IConventionContext context,
            params Type[] types
        )
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Register(context, provider.GetAll(context.GetHostType()), types);
        }

        /// <summary>
        /// Executes the register.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="items">The items.</param>
        /// <param name="types">The types.</param>
        private static void Register(
            IConventionContext context,
            IEnumerable<DelegateOrConvention> items,
            IEnumerable<Type> types
        )
        {
            var enumerable = types as Type[] ?? types.ToArray();
            if (enumerable.Length == 0)
            {
                return;
            }

            var delegateTypes = enumerable.Where(typeof(Delegate).IsAssignableFrom).ToArray();
            var conventionTypes = enumerable.Except(delegateTypes).ToArray();

            var index = 0;
            foreach (var (convention, @delegate) in items)
            {
                try
                {
                    if (convention != null)
                    {
                        if (!conventionTypes.Any(type => type.IsInstanceOfType(convention)))
                        {
                            if (context.Logger.IsEnabled(LogLevel.Debug))
                            {
                                var conventionType = convention.GetType();
                                context.Logger.LogDebug(
                                    "Could not execute Convention {TypeName} from {AssemblyName} :: {@Types}",
                                    conventionType.FullName,
                                    conventionType.Assembly.GetName().Name,
                                    enumerable
                                );
                            }

                            continue;
                        }

                        if (context.Logger.IsEnabled(LogLevel.Debug))
                        {
                            var conventionType = convention.GetType();
                            context.Logger.LogDebug(
                                "Executing Convention {TypeName} from {AssemblyName}",
                                conventionType.FullName,
                                conventionType.Assembly.GetName().Name
                            );
                        }

                        Register(convention, context);
                        continue;
                    }

                    // ReSharper disable once UseNullPropagation
                    // ReSharper disable once InvertIf
                    if (@delegate != null)
                    {
                        if (!delegateTypes.Any(type => type.IsInstanceOfType(@delegate)))
                        {
                            if (context.Logger.IsEnabled(LogLevel.Debug))
                            {
                                context.Logger.LogDebug(
                                    "Could not execute Delegate {TypeName} :: {@Types}",
                                    @delegate.GetType().FullName,
                                    enumerable
                                );
                            }

                            continue;
                        }

                        if (context.Logger.IsEnabled(LogLevel.Debug))
                        {
                            context.Logger.LogDebug(
                                "Executing Delegate {TypeName}",
                                @delegate.GetType().FullName
                            );
                        }

                        @delegate.DynamicInvoke(context);
                        continue;
                    }

                    context.Logger.LogError("Convention or Delegate not available for one of the items");
                }
                catch (Exception e)
                {
                    if (convention != null)
                    {
                        context.Logger.LogError(
                            0,
                            e,
                            "Error invoking Convention {Convention}",
                            convention.GetType().FullName
                        );
                    }
                    else if (@delegate != null)
                    {
                        context.Logger.LogError(0, e, "Error invoking Delegate at index {Index}", index);
                    }
                    else
                    {
                        context.Logger.LogError(
                            "Unknown error invoking Convention or Delegate at index {Index}",
                            index
                        );
                    }

                    throw;
                }

                index++;
            }

            if (context.Logger.IsEnabled(LogLevel.Debug))
            {
                context.Logger.LogDebug("Found {Count} conventions or delegates", index + 1);
            }
        }

        private static readonly ConcurrentDictionary<Type, MethodInfo> _registerMethodCache =
            new ConcurrentDictionary<Type, MethodInfo>();

        private static readonly MethodInfo RegisterGenericMethod =
            typeof(Composer).GetTypeInfo().GetDeclaredMethod(nameof(RegisterGeneric))!;

        private static void Register(IConvention convention, IConventionContext context)
        {
            var interfaces = convention.GetType().GetTypeInfo().ImplementedInterfaces
               .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConvention<>))
               .Select(x => x.GetTypeInfo().GenericTypeArguments[0]);

            var contextTypes = context.GetType().GetTypeInfo().ImplementedInterfaces
               .Where(x => typeof(IConventionContext).IsAssignableFrom(x));

            var typesToRegister = interfaces
               .Join(contextTypes, x => x, x => x, (_, contextType) => contextType);

            foreach (var item in typesToRegister)
            {
                if (!_registerMethodCache.TryGetValue(item, out var method))
                {
                    method = RegisterGenericMethod.MakeGenericMethod(item);
                    _registerMethodCache.TryAdd(item, method);
                }

                method.Invoke(null, new object[] { convention, context });
            }
        }

        private static void RegisterGeneric<T>(IConvention<T> convention, T context)
            where T : IConventionContext => convention.Register(context);
    }
}