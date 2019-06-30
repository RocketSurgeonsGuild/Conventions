using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ConventionContainerBuilder.
    /// Implements the <see cref="Rocket.Surgery.Conventions.IConventionContainer{TBuilder, TConvention, TDelegate}" />
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t builder.</typeparam>
    /// <typeparam name="TConvention">The type of the t convention.</typeparam>
    /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
    /// <seealso cref="Rocket.Surgery.Conventions.IConventionContainer{TBuilder, TConvention, TDelegate}" />
    public abstract class ConventionContainerBuilder<TBuilder, TConvention, TDelegate> : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TBuilder : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        protected ConventionContainerBuilder(
            IConventionScanner scanner,
            IDictionary<object, object> properties)
        {
            Scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));

            if (!Properties.TryGetValue(typeof(IConventionScanner), out _))
                Properties[typeof(IConventionScanner)] = Scanner;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Object.</returns>
        public virtual object this[object item]
        {
            get => Properties.TryGetValue(item, out object value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary<object, object> Properties { get; }

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
        public TBuilder AppendConvention<T>() where T : TConvention
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
            Scanner.AppendDelegate(delegates);
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
        public TBuilder PrependConvention<T>() where T : TConvention
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
            Scanner.PrependDelegate(delegates);
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
