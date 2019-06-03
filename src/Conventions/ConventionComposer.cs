using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Convention base compose, that calls all methods on register.
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    /// <typeparam name="TContribution">The contribution type</typeparam>
    /// <typeparam name="TDelegate">The delegate type</typeparam>
    public abstract class ConventionComposer<TContext, TContribution, TDelegate> : ConventionComposerBase, IConventionComposer<TContext, TContribution, TDelegate>
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
