using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable IDE0058 // Expression value is never used

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ConventionContainerBuilder.
    /// Implements the <see cref="IConventionContainer{TBuilder, TConvention, TDelegate}" />
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t builder.</typeparam>
    /// <typeparam name="TConvention">The type of the t convention.</typeparam>
    /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
    /// <seealso cref="IConventionContainer{TBuilder, TConvention, TDelegate}" />
    public abstract class
        ConventionContainerBuilder<TBuilder, TConvention, TDelegate> : IConventionContainer<TBuilder, TConvention,
            TDelegate>
        where TBuilder : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionContainerBuilder{TBuilder, TConvention, TDelegate}" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">
        /// scanner
        /// or
        /// properties
        /// </exception>
        protected ConventionContainerBuilder(
            IConventionScanner scanner,
            IDictionary<object, object?> properties
        )
        {
            Scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));

            if (!Properties.TryGetValue(typeof(IConventionScanner), out var _))
            {
                Properties[typeof(IConventionScanner)] = Scanner;
            }
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Object.</returns>
        public virtual object? this[object item]
        {
            get => Properties.TryGetValue(item, out var value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary<object, object?> Properties { get; }

        /// <summary>
        /// Gets the scanner.
        /// </summary>
        /// <value>The scanner.</value>
        public IConventionScanner Scanner { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder AppendConvention(params TConvention[] conventions)
        {
            Scanner.AppendConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder AppendConvention(IEnumerable<TConvention> conventions)
        {
            Scanner.AppendConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TBuilder.</returns>
        public TBuilder AppendConvention<T>()
            where T : TConvention
        {
            Scanner.AppendConvention<T>();
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder AppendDelegate(params TDelegate[] delegates)
        {
            Scanner.AppendDelegate(delegates.Cast<Delegate>().ToArray());
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder AppendDelegate(IEnumerable<TDelegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder PrependConvention(params TConvention[] conventions)
        {
            Scanner.PrependConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder PrependConvention(IEnumerable<TConvention> conventions)
        {
            Scanner.PrependConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TBuilder.</returns>
        public TBuilder PrependConvention<T>()
            where T : TConvention
        {
            Scanner.PrependConvention<T>();
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder PrependDelegate(params TDelegate[] delegates)
        {
            Scanner.PrependDelegate(delegates.Cast<Delegate>().ToArray());
            return (TBuilder)(object)this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>TBuilder.</returns>
        public TBuilder PrependDelegate(IEnumerable<TDelegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return (TBuilder)(object)this;
        }
    }
}