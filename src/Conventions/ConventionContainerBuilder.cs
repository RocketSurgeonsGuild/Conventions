using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionContainerBuilder<TBuilder, TConvention, TDelegate> : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TBuilder : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        protected ConventionContainerBuilder(
            IRocketEnvironment environment, 
            IConventionScanner scanner, 
            IDictionary<object, object> properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            Properties = properties ?? new Dictionary<object, object>();
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual object this[object item]
        {
            get => Properties.TryGetValue(item, out object value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        public IDictionary<object, object> Properties { get; }

        public IConventionScanner Scanner { get; }
        public IRocketEnvironment Environment { get; }

        public TBuilder AppendConvention(params TConvention[] conventions)
        {
            Scanner.AppendConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        public TBuilder AppendConvention(IEnumerable<TConvention> conventions)
        {
            Scanner.AppendConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        public TBuilder AppendDelegate(params TDelegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return (TBuilder)(object)this;
        }

        public TBuilder AppendDelegate(IEnumerable<TDelegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return (TBuilder)(object)this;
        }

        public TBuilder PrependConvention(params TConvention[] conventions)
        {
            Scanner.PrependConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        public TBuilder PrependConvention(IEnumerable<TConvention> conventions)
        {
            Scanner.PrependConvention(conventions.Cast<IConvention>());
            return (TBuilder)(object)this;
        }

        public TBuilder PrependDelegate(params TDelegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return (TBuilder)(object)this;
        }

        public TBuilder PrependDelegate(IEnumerable<TDelegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return (TBuilder)(object)this;
        }
    }
}
