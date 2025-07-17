namespace Shopping.SharedKernel.Domain.Events;

public abstract class DomainEvent
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}