using Shared.Kernel.CQRS;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Commands;

// Shipping Method Commands
public record CreateShippingMethodCommand(
    string Name,
    string? Description,
    decimal BaseCost,
    bool RequiresTimeSlot = false
) : ICommand<Guid>;

public record UpdateShippingMethodCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal BaseCost
) : ICommand;

public record DeleteShippingMethodCommand(Guid Id) : ICommand;

// Cost Rule Commands
public record AddCostRuleToMethodCommand(
    Guid ShippingMethodId,
    int RuleType,
    string Value,
    decimal Amount,
    bool IsPercentage = false
) : ICommand;

// Restriction Rule Commands
public record AddRestrictionRuleToMethodCommand(
    Guid ShippingMethodId,
    int RuleType,
    string Value
) : ICommand;

// Time Slot Commands
public record CreateTimeSlotTemplateCommand(
    Guid ShippingMethodId,
    int DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int Capacity
) : ICommand<Guid>;

public record ReserveTimeSlotCommand(
    Guid ShippingMethodId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string CustomerId,
    string? OrderId = null
) : ICommand<Guid>;
