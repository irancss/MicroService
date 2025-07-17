using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Services;

namespace ShippingService.Application.Features.FreeShippingRules.Commands
{
    /// <summary>
    /// Command to calculate free shipping for an order
    /// </summary>
    public record CalculateFreeShippingCommand(
        string UserId,
        decimal OrderAmount,
        int ItemCount,
        decimal TotalWeight,
        string ProductCategory,
        string ShippingMethodId,
        string DestinationPostalCode,
        string DestinationCity
    ) : IRequest<FreeShippingCalculationResult>;
}
