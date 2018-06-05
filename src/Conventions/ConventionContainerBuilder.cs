using System;
using System.Collections.Generic;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionContainerBuilder<TBuilder, TConvention, TDelegate> : Builder, IConventionContainer<TBuilder, TConvention, TDelegate>
        where TBuilder : IBuilder
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        protected readonly IConventionScanner Scanner;

        protected ConventionContainerBuilder(IConventionScanner scanner, IDictionary<object, object> properties) : base(properties)
        {
            Scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        protected abstract TBuilder GetBuilder();

        public TBuilder PrependDelegate(TDelegate @delegate)
        {
            Scanner.PrependDelegate(@delegate as Delegate);
            return GetBuilder();
        }

        public TBuilder PrependConvention(TConvention convention)
        {
            Scanner.PrependConvention(convention);
            return GetBuilder();
        }

        public TBuilder AppendDelegate(TDelegate @delegate)
        {
            Scanner.AppendDelegate(@delegate as Delegate);
            return GetBuilder();
        }

        public TBuilder AppendConvention(TConvention convention)
        {
            Scanner.AppendConvention(convention);
            return GetBuilder();
        }
    }
}
