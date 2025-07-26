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