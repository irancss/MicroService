using System;
using System.Collections.Generic;

namespace ProductService.Domain.ValueObjects 
{
    public class ProductName : ValueObject
    {
        public string Value { get; }

        private ProductName(string value)
        {
            Value = value;
        }

        public static ProductName Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name cannot be empty.", nameof(name));
            }
            if (name.Length > 100) // Example validation
            {
                throw new ArgumentException("Product name cannot be longer than 100 characters.", nameof(name));
            }
            return new ProductName(name);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(ProductName name) => name.Value;
        // public static explicit operator ProductName(string name) => Create(name); // Consider if explicit conversion is better
    }

    // Base class for Value Objects (place in a common location or ValueObjects folder)
    public abstract class ValueObject
    {
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject one, ValueObject two)
        {
            return EqualOperator(one, two);
        }

        public static bool operator !=(ValueObject one, ValueObject two)
        {
            return NotEqualOperator(one, two);
        }
    }
}