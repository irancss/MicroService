namespace NotificationService.Domain.ValueObjects;

public record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty", nameof(value));
        
        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format", nameof(value));
            
        Value = value.ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static implicit operator string(EmailAddress email) => email.Value;
    public static implicit operator EmailAddress(string email) => new(email);
}

public record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));
            
        // Basic validation - you can enhance this
        var cleanedValue = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        if (!cleanedValue.StartsWith("+") && !cleanedValue.All(char.IsDigit))
            throw new ArgumentException("Invalid phone number format", nameof(value));
            
        Value = cleanedValue;
    }

    public static implicit operator string(PhoneNumber phone) => phone.Value;
    public static implicit operator PhoneNumber(string phone) => new(phone);
}
