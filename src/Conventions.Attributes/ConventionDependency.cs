using System;
using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Ensures the convention runs after the given <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="Attribute" />
    internal readonly struct ConventionDependency : IEquatable<ConventionDependency>, IConventionDependency
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="type"></param>
        public ConventionDependency(DependencyDirection direction, [NotNull] Type type)
        {
            Type = type;
            Direction = direction;
        }

        /// <summary>
        /// The <see cref="IConvention" /> type to link to
        /// </summary>
        [NotNull]
        public Type Type { get; }

        /// <summary>
        /// The <see cref="DependencyDirection" /> direction of this relationship
        /// </summary>
        public DependencyDirection Direction { get; }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ConventionDependency other) => Type == other.Type && Direction == other.Direction;

        /// <summary>
        /// Compare equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => obj is ConventionDependency other && Equals(other);

        /// <summary>
        /// Get hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ( Type.GetHashCode() * 397 ) ^ (int)Direction;
            }
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ConventionDependency left, ConventionDependency right) => left.Equals(right);

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ConventionDependency left, ConventionDependency right) => !left.Equals(right);
    }
}