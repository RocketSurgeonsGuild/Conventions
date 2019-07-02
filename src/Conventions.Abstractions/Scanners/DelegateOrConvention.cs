using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A pattern match class that is used to determine if a type is a <see cref="IConvention" />, a <see cref="Delegate" /> or <see cref="None" />
    /// Implements the <see cref="DelegateOrConvention" />
    /// </summary>
    /// <seealso cref="DelegateOrConvention" />
    public struct DelegateOrConvention : IEquatable<DelegateOrConvention>
    {
        /// <summary>
        /// A nether case, if no delegate is found
        /// </summary>
        /// <value>The none.</value>
        public static DelegateOrConvention None { get; } = new DelegateOrConvention();

        /// <summary>
        /// Create a convention
        /// </summary>
        /// <param name="convention">The convention.</param>
        public DelegateOrConvention(IConvention convention)
        {
            Convention = convention;
            Delegate = default;
        }

        /// <summary>
        /// Create a delegate
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        public DelegateOrConvention(Delegate @delegate)
        {
            Convention = default;
            Delegate = @delegate;
        }

        /// <summary>
        /// The convention, only Convention or Delegate are available
        /// </summary>
        /// <value>The convention.</value>
        public IConvention Convention { get; }

        /// <summary>
        /// The delegate, only Convention or Delegate are available
        /// </summary>
        /// <value>The delegate.</value>
        public Delegate Delegate { get; }

        /// <summary>
        /// Operator to get the delegate implictly
        /// </summary>
        /// <param name="delegateOrContribution">The delegate or contribution.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Delegate(DelegateOrConvention delegateOrContribution)
        {
            return delegateOrContribution.Delegate;
        }

        /// <summary>
        /// Operator to create from a delegate
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator DelegateOrConvention(Delegate @delegate)
        {
            return new DelegateOrConvention(@delegate);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="convention1">The convention1.</param>
        /// <param name="convention2">The convention2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DelegateOrConvention convention1, DelegateOrConvention convention2)
        {
            return convention1.Equals(convention2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="convention1">The convention1.</param>
        /// <param name="convention2">The convention2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DelegateOrConvention convention1, DelegateOrConvention convention2)
        {
            return !(convention1 == convention2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is DelegateOrConvention && Equals((DelegateOrConvention)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(DelegateOrConvention other)
        {
            return EqualityComparer<IConvention>.Default.Equals(Convention, other.Convention) &&
                   EqualityComparer<Delegate>.Default.Equals(Delegate, other.Delegate);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
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
