using MediatR;

namespace BuildingBlocks.Domain.Events; // [اصلاح شد] Namespace برای تطابق

/// <summary>
/// [اصلاح شد] این فایل به IDomainEvent.cs تغییر نام داد و به یک اینترفیس تبدیل شد.
/// Represents an event that occurred within the domain.
/// Domain events are published and handled within the same service boundary using MediatR.
/// </summary>
public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}

/// <summary>
/// A base record for all domain events.
/// It implements the IDomainEvent interface and provides common properties.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// A unique identifier for the event instance.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// The Coordinated Universal Time (UTC) when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}