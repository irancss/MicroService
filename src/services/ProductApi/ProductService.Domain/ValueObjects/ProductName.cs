using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using BuildingBlocks.Domain.ValueObjects;

namespace ProductService.Domain.ValueObjects
{
    public class ProductName : ValueObject
    {
        public string Value { get; }

        private ProductName(string value)
        {
            Value = value;
        }

        public static ProductName For(string value)
        {
            Guard.Against.NullOrWhiteSpace(value, nameof(value));

            // [اصلاح شد] استفاده از Guard های صحیح برای طول رشته
            Guard.Against.MinLength(value, 3, nameof(value));
            Guard.Against.MaxLength(value, 150, nameof(value));

            return new ProductName(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(ProductName name) => name.Value;
    }
}