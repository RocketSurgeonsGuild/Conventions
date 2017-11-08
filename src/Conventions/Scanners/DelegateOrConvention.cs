using System;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContribution"></typeparam>
    /// <typeparam name="TDelegate"></typeparam>
    public class DelegateOrConvention<TContribution, TDelegate>
    {
        /// <summary>
        /// A nether case, if no delegate is found
        /// </summary>
        public static DelegateOrConvention<TContribution, TDelegate> None { get; } = new DelegateOrConvention<TContribution, TDelegate>();

        private DelegateOrConvention() { }
        
        /// <summary>
        /// Create a convention
        /// </summary>
        /// <param name="convention"></param>
        public DelegateOrConvention(TContribution convention)
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
        public TContribution Convention { get; }
        
        /// <summary>
        /// The delegate, only Convention or Delegate are available
        /// </summary>
        public Delegate Delegate { get; }

        /// <summary>
        /// Operator to get the convention implictly
        /// </summary>
        /// <param name="delegateOrContribution"></param>
        public static implicit operator TContribution(DelegateOrConvention<TContribution, TDelegate> delegateOrContribution)
        {
            return delegateOrContribution.Convention;
        }

        /// <summary>
        /// Operator to get the delegate implictly
        /// </summary>
        /// <param name="delegateOrContribution"></param>
        public static implicit operator Delegate(DelegateOrConvention<TContribution, TDelegate> delegateOrContribution)
        {
            return delegateOrContribution.Delegate;
        }

        /// <summary>
        /// Operator to create from a convention
        /// </summary>
        /// <param name="contribution"></param>
        public static implicit operator DelegateOrConvention<TContribution, TDelegate>(TContribution contribution)
        {
            return new DelegateOrConvention<TContribution, TDelegate>(contribution);
        }

        /// <summary>
        /// Operator to create from a delegate
        /// </summary>
        /// <param name="delegate"></param>
        public static implicit operator DelegateOrConvention<TContribution, TDelegate>(TDelegate @delegate)
        {
            return new DelegateOrConvention<TContribution, TDelegate>(@delegate);
        }
    }
}