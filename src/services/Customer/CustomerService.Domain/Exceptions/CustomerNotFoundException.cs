using BuildingBlocks.Domain;

namespace CustomerService.Domain.Exceptions
{
    public class CustomerNotFoundException : DomainException
    {
        public CustomerNotFoundException(Guid customerId)
            : base($"Customer with ID '{customerId}' was not found.")
        {
        }
    }
}
