using System;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A pattern match class that is used to determine if a type is a <see cref="IConvention"/>, a <see cref="Delegate"/> or <see cref="None"/>
    /// </summary>
    /// <typeparam name="TConvention">The convention type</typeparam>
    /// <typeparam name="TDelegate">The delegate type</typeparam>
    public class DelegateOrConvention<TConvention, TDelegate>
    {
        /// <summary>
        /// A nether case, if no delegate is found
        /// </summary>
        public static DelegateOrConvention<TConvention, TDelegate> None { get; } = new DelegateOrConvention<TConvention, TDelegate>();

        private DelegateOrConvention() { }

        /// <summary>
        /// Create a convention
        /// </summary>
        /// <param name="convention"></param>
        public DelegateOrConvention(TConvention convention)
        {
            Convention = convention;
        }

        /// <summary>
        /// Create a delegate
        /// </summary>
        /// <param name="delegate"></param>
        public DelegateOrConvention(TDelegate @delegate)
        {
            Delegate = @delegate as Delegate;
        }

        /// <summary>
        /// The convention, only Convention or Delegate are available
        /// </summary>
        public TConvention Convention { get; }

        /// <summary>
        /// The delegate, only Convention or Delegate are available
        /// </summary>
        public Delegate Delegate { get; }

        /// <summary>
        /// Operator to get the convention implictly
        /// </summary>
        /// <param name="delegateOrContribution"></param>
        public static implicit operator TConvention(DelegateOrConvention<TConvention, TDelegate> delegateOrContribution)
        {
            return delegateOrContribution.Convention;
        }

        /// <summary>
        /// Operator to get the delegate implictly
        /// </summary>
        /// <param name="delegateOrContribution"></param>
        public static implicit operator Delegate(DelegateOrConvention<TConvention, TDelegate> delegateOrContribution)
        {
            return delegateOrContribution.Delegate;
        }

        /// <summary>
        /// Operator to create from a convention
        /// </summary>
        /// <param name="contribution"></param>
        public static implicit operator DelegateOrConvention<TConvention, TDelegate>(TConvention contribution)
        {
            return new DelegateOrConvention<TConvention, TDelegate>(contribution);
        }

        /// <summary>
        /// Operator to create from a delegate
        /// </summary>
        /// <param name="delegate"></param>
        public static implicit operator DelegateOrConvention<TConvention, TDelegate>(TDelegate @delegate)
        {
            return new DelegateOrConvention<TConvention, TDelegate>(@delegate);
        }
    }

    /// <summary>
    /// A pattern match class that is used to determine if a type is a <see cref="IConvention"/>, a <see cref="Delegate"/> or <see cref="None"/>
    /// </summary>
    public class DelegateOrConvention
    {
        /// <summary>
        /// A nether case, if no delegate is found
        /// </summary>
        public static DelegateOrConvention None { get; } = new DelegateOrConvention();

        private DelegateOrConvention() { }

        /// <summary>
        /// Create a convention
        /// </summary>
        /// <param name="convention"></param>
        public DelegateOrConvention(IConvention convention)
        {
            Convention = convention;
        }

        /// <summary>
        /// Create a delegate
        /// </summary>
        /// <param name="delegate"></param>
        public DelegateOrConvention(Delegate @delegate)
        {
            Delegate = @delegate;
        }

        /// <summary>
        /// The convention, only Convention or Delegate are available
        /// </summary>
        public IConvention Convention { get; }

        /// <summary>
        /// The delegate, only Convention or Delegate are available
        /// </summary>
        public Delegate Delegate { get; }

        /// <summary>
        /// Operator to get the delegate implictly
        /// </summary>
        /// <param name="delegateOrContribution"></param>
        public static implicit operator Delegate(DelegateOrConvention delegateOrContribution)
        {
            return delegateOrContribution.Delegate;
        }

        /// <summary>
        /// Operator to create from a delegate
        /// </summary>
        /// <param name="delegate"></param>
        public static implicit operator DelegateOrConvention(Delegate @delegate)
        {
            return new DelegateOrConvention(@delegate);
        }
    }
}
