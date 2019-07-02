using System;
using System.Collections.Generic;
// ReSharper disable UnusedTypeParameter

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A composer that allows registration of delegates and interfaces that implement an interface like the delegate
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    /// <typeparam name="TContribution">The contribution type</typeparam>
    /// <typeparam name="TDelegate">The delegate Type</typeparam>
    /// <seealso cref="IConvention{TContext}" />
    public interface IConventionComposer<in TContext, TContribution, TDelegate> : IConvention<TContext>
        where TContribution : IConvention<TContext>
        where TContext : IConventionContext
        where TDelegate : Delegate
    {
    }

    /// <summary>
    /// Takes a list of conventions and composes them with the given context
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{TContext}" />
    public interface IConventionComposer
    {
        /// <summary>
        /// Uses all the conventions and calls the register method for all of them.
        /// </summary>
        /// <param name="context">The valid context for the types</param>
        /// <param name="types">The types to compose with.  This type will either be a <see cref="Delegate" /> that takes <see cref="IConventionContext" />, or a type that implements <see cref="IConvention{IConventionContext}" /></param>
        void Register(IConventionContext context, IEnumerable<Type> types);
    }
}
