using Ardalis.GuardClauses;
using BuildingBlocks.Domain.Entities;
using CustomerService.Domain.Events;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Entities
{
    public class Customer : AuditableEntity<Guid>
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string? PhoneNumber { get; private set; }

        private readonly List<Address> _addresses = new();
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        // EF Core constructor
        private Customer() { }

        private Customer(Guid id, string firstName, string lastName, string email, string? phoneNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public static Customer Create(string firstName, string lastName, string email, string? phoneNumber)
        {
            Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
            Guard.Against.NullOrWhiteSpace(email, nameof(email));
            // Simple email validation for demo
            Guard.Against.InvalidInput(email, nameof(email), e => e.Contains('@'), "Email format is invalid.");

            var customer = new Customer(Guid.NewGuid(), firstName, lastName, email, phoneNumber);

            // Add a domain event to be published after saving
            customer.AddDomainEvent(new CustomerCreatedDomainEvent(customer.Id, customer.FullName, customer.Email));

            return customer;
        }

        public void Update(string firstName, string lastName)
        {
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
        }

        public void AddAddress(Address address)
        {
            Guard.Against.Null(address, nameof(address));
            _addresses.Add(address);
        }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
