using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Domain.ValueObjects
{
    /// <summary>
    /// A base class for creating value objects.
    /// Value objects are objects that we determine their equality through their constituent properties.
    /// They are immutable and their identity is based on the combination of their values.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// When overridden in a derived class, returns all components of the value object
        /// that are used for equality comparison.
        /// </summary>
        /// <returns>An IEnumerable of the components of the value object.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// by comparing their equality components.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Serves as the default hash function. The hash code is calculated based on
        /// the equality components.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            // This implementation is recommended by Josh Bloch for generating hash codes.
            // It combines the hash codes of the equality components.
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate(17, (current, next) => current * 23 + next);
        }

        /// <summary>
        /// Determines whether two value objects are equal.
        /// </summary>
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            // Handles cases where one or both are null.
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        /// <summary>
        /// Determines whether two value objects are not equal.
        /// </summary>
        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !(left == right);
        }
    }
}