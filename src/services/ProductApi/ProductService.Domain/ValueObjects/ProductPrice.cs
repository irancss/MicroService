using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain.ValueObjects;

namespace ProductService.Domain.ValueObjects
{
    public class ProductPrice : ValueObject
    {
        public decimal Value { get; }

        private ProductPrice(decimal value)
        {
            Value = value;
        }

        public static ProductPrice For(decimal value)
        {
            Guard.Against.NegativeOrZero(value, nameof(value));
            return new ProductPrice(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator decimal(ProductPrice price) => price.Value;
    }
}
