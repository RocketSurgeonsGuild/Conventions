using System.Collections.Generic;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// A marker interface to indicate a type is a convention
    /// </summary>
    public interface IConvention { }

    /// <summary>
    /// A default interface that can be used to create a convention with a known context type
    /// context is used to house all the data that the convention requires to do it's job
    /// This can be things like a service collection, container builder, logger, etc.
    /// Implements the <see cref="Rocket.Surgery.Conventions.IConvention" />
    /// </summary>
    /// <typeparam name="TContext">The convention type that contains all the values for this convention to work</typeparam>
    /// <seealso cref="Rocket.Surgery.Conventions.IConvention" />
    public interface IConvention<in TContext> : IConvention
        where TContext : IConventionContext
    {
        /// <summary>
        /// A method that is called to register a given convention at runtime.
        /// </summary>
        /// <param name="context">The context.</param>
        void Register(TContext context);
    }
}
