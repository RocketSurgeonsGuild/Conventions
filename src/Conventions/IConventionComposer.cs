using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A composer that allows registration of delegates and interfaces that implement an interface like the delegate
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TContribution"></typeparam>
    /// <typeparam name="TDelegate"></typeparam>
    public interface IConventionComposer<TContext, TContribution, TDelegate> : IConvention<TContext>
        where TContribution : IConvention<TContext>
        where TContext : IConventionContext
    {
    }

    public interface IConventionComposer
    {
        void Register(IConventionContext context, Type type, params Type[] types);
        void Register(IConventionContext context, IEnumerable<Type> types);
    }
}
