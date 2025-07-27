using Ardalis.GuardClauses;
using BuildingBlocks.Domain.ValueObjects;

namespace ProductService.Domain.ValueObjects
{
    public class Sku : ValueObject
    {
        public string Value { get; }

        private Sku(string value)
        {
            Value = value;
        }

        public static Sku For(string value)
        {
            Guard.Against.NullOrWhiteSpace(value, nameof(value));
            // اینجا می‌توانید قوانین بیشتری برای فرمت SKU اضافه کنید
            return new Sku(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Sku sku) => sku.Value;
    }
}
