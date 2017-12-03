using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A pattern match class that is used to determine if a type is a <see cref="IConvention"/>, a <see cref="Delegate"/> or <see cref="None"/>
    /// </summary>
    public struct DelegateOrConvention : IEquatable<DelegateOrConvention>
    {
        /// <summary>
        /// A nether case, if no delegate is found
        /// </summary>
        public static DelegateOrConvention None { get; } = new DelegateOrConvention();

        /// <summary>
        /// Create a convention
        /// </summary>
        /// <param name="convention"></param>
        public DelegateOrConvention(IConvention convention)
        {
            Convention = convention;
            Delegate = default;
        }

        /// <summary>
        /// Create a delegate
        /// </summary>
        /// <param name="delegate"></param>
        public DelegateOrConvention(Delegate @delegate)
        {
            Convention = default;
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

        public static bool operator ==(DelegateOrConvention convention1, DelegateOrConvention convention2)
        {
            return convention1.Equals(convention2);
        }

        public static bool operator !=(DelegateOrConvention convention1, DelegateOrConvention convention2)
        {
            return !(convention1 == convention2);
        }

        public override bool Equals(object obj)
        {
            return obj is DelegateOrConvention && Equals((DelegateOrConvention)obj);
        }

        public bool Equals(DelegateOrConvention other)
        {
            return EqualityComparer<IConvention>.Default.Equals(Convention, other.Convention) &&
                   EqualityComparer<Delegate>.Default.Equals(Delegate, other.Delegate);
        }

        public override int GetHashCode()
        {
            var hashCode = 190459212;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IConvention>.Default.GetHashCode(Convention);
            hashCode = hashCode * -1521134295 + EqualityComparer<Delegate>.Default.GetHashCode(Delegate);
            return hashCode;
        }
    }
}
