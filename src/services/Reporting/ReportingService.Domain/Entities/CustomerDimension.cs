namespace ReportingService.Domain.Entities;

/// <summary>
/// Customer dimension table for star schema
/// </summary>
public class CustomerDimension : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Segment { get; private set; } = string.Empty; // Premium, Standard, Basic
    public DateTime RegistrationDate { get; private set; }
    public bool IsActive { get; private set; }

    private CustomerDimension() { } // EF Core

    public CustomerDimension(
        Guid customerId, 
        string email, 
        string firstName, 
        string lastName, 
        string country, 
        string city, 
        string segment, 
        DateTime registrationDate, 
        bool isActive = true)
    {
        CustomerId = customerId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Country = country;
        City = city;
        Segment = segment;
        RegistrationDate = registrationDate;
        IsActive = isActive;
    }

    public void UpdateCustomer(string email, string firstName, string lastName, string country, string city, string segment)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Country = country;
        City = city;
        Segment = segment;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
