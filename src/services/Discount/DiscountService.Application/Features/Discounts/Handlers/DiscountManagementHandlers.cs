using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using DiscountService.Application.DTOs;
using DiscountService.Application.Features.Discounts.Commands;
using DiscountService.Application.Interfaces;
using DiscountService.Domain.Entities;

namespace DiscountService.Application.Features.Discounts.Handlers;

/// <summary>
/// Handler for creating a new discount
/// </summary>
public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, DiscountDto>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateDiscountCommandHandler> _logger;

    public CreateDiscountCommandHandler(
        IDiscountRepository discountRepository,
        ICacheService cacheService,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<CreateDiscountCommandHandler> logger)
    {
        _discountRepository = discountRepository;
        _cacheService = cacheService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DiscountDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new discount: {Name}", request.Name);

        var discount = new Discount
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CouponCode = request.CouponCode,
            Type = request.Type,
            Value = request.Value,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = true,
            IsAutomatic = request.IsAutomatic,
            IsCombinableWithOthers = request.IsCombinableWithOthers,
            MaxTotalUsage = request.MaxTotalUsage,
            MaxUsagePerUser = request.MaxUsagePerUser,
            MinimumCartAmount = request.MinimumCartAmount,
            MaximumDiscountAmount = request.MaximumDiscountAmount,
            Applicability = request.Applicability,
            ApplicableProductIds = request.ApplicableProductIds,
            ApplicableCategoryIds = request.ApplicableCategoryIds,
            BuyQuantity = request.BuyQuantity,
            GetQuantity = request.GetQuantity,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserEmail ?? "System",
            UpdatedBy = _currentUserService.UserEmail ?? "System"
        };

        var createdDiscount = await _discountRepository.AddAsync(discount);

        // Invalidate relevant caches
        await InvalidateCaches(createdDiscount);

        _logger.LogInformation("Discount created successfully: {Id}", createdDiscount.Id);

        return _mapper.Map<DiscountDto>(createdDiscount);
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
    }
}

/// <summary>
/// Handler for updating an existing discount
/// </summary>
public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, DiscountDto>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateDiscountCommandHandler> _logger;

    public UpdateDiscountCommandHandler(
        IDiscountRepository discountRepository,
        ICacheService cacheService,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<UpdateDiscountCommandHandler> logger)
    {
        _discountRepository = discountRepository;
        _cacheService = cacheService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DiscountDto> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating discount: {Id}", request.Id);

        var existingDiscount = await _discountRepository.GetByIdAsync(request.Id);
        if (existingDiscount == null)
        {
            throw new ArgumentException($"Discount with ID {request.Id} not found");
        }

        // Store old coupon code for cache invalidation
        var oldCouponCode = existingDiscount.CouponCode;
        var oldIsAutomatic = existingDiscount.IsAutomatic;

        // Update properties
        existingDiscount.Name = request.Name;
        existingDiscount.Description = request.Description;
        existingDiscount.CouponCode = request.CouponCode;
        existingDiscount.Type = request.Type;
        existingDiscount.Value = request.Value;
        existingDiscount.StartDate = request.StartDate;
        existingDiscount.EndDate = request.EndDate;
        existingDiscount.IsAutomatic = request.IsAutomatic;
        existingDiscount.IsCombinableWithOthers = request.IsCombinableWithOthers;
        existingDiscount.MaxTotalUsage = request.MaxTotalUsage;
        existingDiscount.MaxUsagePerUser = request.MaxUsagePerUser;
        existingDiscount.MinimumCartAmount = request.MinimumCartAmount;
        existingDiscount.MaximumDiscountAmount = request.MaximumDiscountAmount;
        existingDiscount.Applicability = request.Applicability;
        existingDiscount.ApplicableProductIds = request.ApplicableProductIds;
        existingDiscount.ApplicableCategoryIds = request.ApplicableCategoryIds;
        existingDiscount.BuyQuantity = request.BuyQuantity;
        existingDiscount.GetQuantity = request.GetQuantity;
        existingDiscount.UserId = request.UserId;
        existingDiscount.UpdatedAt = DateTime.UtcNow;
        existingDiscount.UpdatedBy = _currentUserService.UserEmail ?? "System";

        var updatedDiscount = await _discountRepository.UpdateAsync(existingDiscount);

        // Invalidate relevant caches
        await InvalidateCaches(updatedDiscount, oldCouponCode, oldIsAutomatic);

        _logger.LogInformation("Discount updated successfully: {Id}", updatedDiscount.Id);

        return _mapper.Map<DiscountDto>(updatedDiscount);
    }

    private async Task InvalidateCaches(Discount discount, string? oldCouponCode, bool oldIsAutomatic)
    {
        // Clear automatic discounts cache if needed
        if (discount.IsAutomatic || oldIsAutomatic)
        {
            await _cacheService.RemoveAsync("automatic_discounts");
        }

        // Clear old coupon cache
        if (!string.IsNullOrEmpty(oldCouponCode))
        {
            await _cacheService.RemoveAsync($"coupon_{oldCouponCode}");
        }

        // Clear new coupon cache
        if (!string.IsNullOrEmpty(discount.CouponCode))
        {
            await _cacheService.RemoveAsync($"coupon_{discount.CouponCode}");
        }
    }
}

/// <summary>
/// Handler for deleting a discount
/// </summary>
public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, bool>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteDiscountCommandHandler> _logger;

    public DeleteDiscountCommandHandler(
        IDiscountRepository discountRepository,
        ICacheService cacheService,
        ILogger<DeleteDiscountCommandHandler> logger)
    {
        _discountRepository = discountRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting discount: {Id}", request.Id);

        var discount = await _discountRepository.GetByIdAsync(request.Id);
        if (discount == null)
        {
            return false;
        }

        await _discountRepository.DeleteAsync(request.Id);

        // Invalidate relevant caches
        if (discount.IsAutomatic)
        {
            await _cacheService.RemoveAsync("automatic_discounts");
        }

        if (!string.IsNullOrEmpty(discount.CouponCode))
        {
            await _cacheService.RemoveAsync($"coupon_{discount.CouponCode}");
        }

        _logger.LogInformation("Discount deleted successfully: {Id}", request.Id);

        return true;
    }
}
