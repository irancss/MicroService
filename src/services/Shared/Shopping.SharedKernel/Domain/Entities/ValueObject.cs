namespace Shopping.SharedKernel.Domain.Entities;

public abstract class ValueObject
{
    // ... implementation for equality checks based on properties
    protected abstract IEnumerable<object> GetEqualityComponents();
    // ... Equals, GetHashCode, ==, != operators
}