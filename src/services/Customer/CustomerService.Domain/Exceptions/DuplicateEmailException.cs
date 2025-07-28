using BuildingBlocks.Domain;

namespace CustomerService.Domain.Exceptions
{
    public class DuplicateEmailException : DomainException
    {
        public DuplicateEmailException(string email)
            : base($"A customer with the email '{email}' already exists.")
        {
        }
    }
}
