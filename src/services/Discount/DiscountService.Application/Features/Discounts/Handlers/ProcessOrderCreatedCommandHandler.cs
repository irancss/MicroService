using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using DiscountService.Application.Features.Discounts.Commands;
using DiscountService.Application.Interfaces;
using DiscountService.Domain.Entities;

namespace DiscountService.Application.Features.Discounts.Handlers;

/// <summary>
/// Handler for processing order created events
/// </summary>
public class ProcessOrderCreatedCommandHandler : IRequestHandler<ProcessOrderCreatedCommand, bool>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IDiscountUsageHistoryRepository _usageHistoryRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProcessOrderCreatedCommandHandler> _logger;

    public ProcessOrderCreatedCommandHandler(
        IDiscountRepository discountRepository,
        IDiscountUsageHistoryRepository usageHistoryRepository,
        ICacheService cacheService,
        ILogger<ProcessOrderCreatedCommandHandler> logger)
    {
        _discountRepository = discountRepository;
        _usageHistoryRepository = usageHistoryRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<bool> Handle(ProcessOrderCreatedCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing order created event for Order: {OrderId}, User: {UserId}, Discount: {DiscountId}", 
                request.OrderId, request.UserId, request.DiscountId);

            if (!request.DiscountId.HasValue || request.DiscountAmount <= 0)
            {
                _logger.LogInformation("No discount applied for order {OrderId}", request.OrderId);
                return true;
            }

            var discount = await _discountRepository.GetByIdAsync(request.DiscountId.Value);
            if (discount == null)
            {
                _logger.LogWarning("Discount {DiscountId} not found for order {OrderId}", request.DiscountId, request.OrderId);
                return false;
            }

            // Record usage history
            var usageHistory = new DiscountUsageHistory
            {
                Id = Guid.NewGuid(),
                DiscountId = request.DiscountId.Value,
                UserId = request.UserId,
                OrderId = request.OrderId,
                DiscountAmount = request.DiscountAmount,
                CartTotal = request.CartTotal,
                FinalTotal = request.FinalTotal,
                CouponCode = request.CouponCode,
                UsedAt = request.OrderCreatedAt,
                UserEmail = request.UserEmail
            };

            await _usageHistoryRepository.AddAsync(usageHistory);

            // Update discount usage count
            await _discountRepository.UpdateUsageCountAsync(request.DiscountId.Value);

            // Invalidate relevant caches
            await InvalidateCaches(discount);

            _logger.LogInformation("Successfully processed discount usage for order {OrderId}", request.OrderId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order created event for order {OrderId}", request.OrderId);
            throw;
        }
    }

    private async Task InvalidateCaches(Discount discount)
    {
        // Clear automatic discounts cache if needed
        if (discount.IsAutomatic)
        {
            await _cacheService.RemoveAsync("automatic_discounts");
        }

        // Clear coupon cache if it has a coupon code
        if (!string.IsNullOrEmpty(discount.CouponCode))
        {
            await _cacheService.RemoveAsync($"coupon_{discount.CouponCode}");
        }

        // Clear discount specific cache
        await _cacheService.RemoveAsync($"discount_{discount.Id}");
    }
}
