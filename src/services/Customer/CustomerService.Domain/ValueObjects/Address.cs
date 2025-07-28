using BuildingBlocks.Domain.ValueObjects;

namespace CustomerService.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }
        public bool IsPrimary { get; private set; }

        // EF Core constructor
        private Address() { }

        public Address(string street, string city, string state, string country, string zipCode, bool isPrimary = false)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
            IsPrimary = isPrimary;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}
