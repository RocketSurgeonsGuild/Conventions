using System;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public abstract class ConventionContainerBuilder<TBuilder, TConvention, TDelegate> : Builder, IConventionContainer<TBuilder, TConvention, TDelegate>
        where TBuilder : IBuilder
        where TConvention : IConvention
    {
        protected readonly IConventionScanner Scanner;

        protected ConventionContainerBuilder(IConventionScanner scanner)
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
