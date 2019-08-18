using System;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    ///  IConventionBuilder
    /// Implements the <see cref="IConventionContainer{TBuilder, TConvention, TDelegate}" />
    /// </summary>
    /// <typeparam name="TBuilder">The type of the t builder.</typeparam>
    /// <typeparam name="TConvention">The type of the t convention.</typeparam>
    /// <typeparam name="TDelegate">The type of the t delegate.</typeparam>
    /// <seealso cref="IConventionContainer{TBuilder, TConvention, TDelegate}" />
    public interface IConventionBuilder<out TBuilder, in TConvention, in TDelegate> : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TBuilder : IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
    }
}
