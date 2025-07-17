using AutoMapper;
using Shared.Kernel.CQRS;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries;
using ShippingService.Application.Services;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Repositories;

namespace ShippingService.Application.Handlers.Queries;

public class GetAvailableShippingOptionsQueryHandler : IQueryHandler<GetAvailableShippingOptionsQuery, IEnumerable<AvailableShippingOptionDto>>
{
    private readonly IShippingMethodRepository _shippingMethodRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IProductServiceClient _productServiceClient;
    private readonly IMapper _mapper;

    public GetAvailableShippingOptionsQueryHandler(
        IShippingMethodRepository shippingMethodRepository,
        ITimeSlotRepository timeSlotRepository,
        IProductServiceClient productServiceClient,
        IMapper mapper)
    {
        _shippingMethodRepository = shippingMethodRepository;
        _timeSlotRepository = timeSlotRepository;
        _productServiceClient = productServiceClient;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AvailableShippingOptionDto>> Handle(GetAvailableShippingOptionsQuery request, CancellationToken cancellationToken)
    {
        // Get all active shipping methods
        var shippingMethods = await _shippingMethodRepository.GetAllActiveAsync(cancellationToken);
        
        // Convert cart items to domain entities and enrich with product information
        var enrichedCartItems = await EnrichCartItemsWithProductInfo(request.CartItems, cancellationToken);
        
        var availableOptions = new List<AvailableShippingOptionDto>();
        var deliveryDate = request.PreferredDeliveryDate ?? DateTime.Now.AddDays(1);

        foreach (var method in shippingMethods)
        {
            // Check restriction rules first
            if (!method.IsAllowedForCart(enrichedCartItems))
                continue;

            // Calculate final cost with all cost rules applied
            var finalCost = method.CalculateFinalCost(enrichedCartItems, deliveryDate);

            var option = new AvailableShippingOptionDto
            {
                ShippingMethodId = method.Id,
                Name = method.Name,
                Description = method.Description,
                FinalCost = finalCost,
                RequiresTimeSlot = method.RequiresTimeSlot
            };

            // If method requires time slots, get available ones
            if (method.RequiresTimeSlot)
            {
                var availableTimeSlots = await GetAvailableTimeSlots(method.Id, deliveryDate, cancellationToken);
                option.AvailableTimeSlots = availableTimeSlots.ToList();
                
                // Only include methods that have available time slots
                if (!option.AvailableTimeSlots.Any())
                    continue;
            }

            availableOptions.Add(option);
        }

        return availableOptions;
    }

    private async Task<List<CartItem>> EnrichCartItemsWithProductInfo(List<CartItemDto> cartItems, CancellationToken cancellationToken)
    {
        var enrichedItems = new List<CartItem>();

        foreach (var item in cartItems)
        {
            // Call product service to get additional product information
            var productInfo = await _productServiceClient.GetProductByIdAsync(item.ProductId, cancellationToken);
            
            enrichedItems.Add(new CartItem
            {
                ProductId = item.ProductId,
                Category = productInfo?.Category ?? item.Category,
                Quantity = item.Quantity,
                Weight = productInfo?.Weight ?? item.Weight,
                UnitPrice = item.UnitPrice
            });
        }

        return enrichedItems;
    }

    private async Task<IEnumerable<AvailableTimeSlotDto>> GetAvailableTimeSlots(Guid shippingMethodId, DateTime fromDate, CancellationToken cancellationToken)
    {
        var availableSlots = new List<AvailableTimeSlotDto>();
        var templates = await _timeSlotRepository.GetTemplatesByShippingMethodAsync(shippingMethodId, cancellationToken);
        
        // Check next 7 days for available slots
        for (int i = 0; i < 7; i++)
        {
            var checkDate = DateOnly.FromDateTime(fromDate.AddDays(i));
            var dayOfWeek = (Domain.Enums.DayOfWeek)((int)checkDate.DayOfWeek);
            
            var dayTemplates = templates.Where(t => t.DayOfWeek == dayOfWeek && t.IsActive);
            
            foreach (var template in dayTemplates)
            {
                var availableCapacity = await _timeSlotRepository.GetAvailableCapacityAsync(template.Id, checkDate, cancellationToken);
                
                if (availableCapacity > 0)
                {
                    availableSlots.Add(new AvailableTimeSlotDto
                    {
                        TemplateId = template.Id,
                        Date = checkDate,
                        StartTime = template.StartTime,
                        EndTime = template.EndTime,
                        AvailableCapacity = availableCapacity
                    });
                }
            }
        }

        return availableSlots;
    }
}

public class GetAvailableTimeSlotsQueryHandler : IQueryHandler<GetAvailableTimeSlotsQuery, IEnumerable<AvailableTimeSlotDto>>
{
    private readonly ITimeSlotRepository _repository;

    public GetAvailableTimeSlotsQueryHandler(ITimeSlotRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AvailableTimeSlotDto>> Handle(GetAvailableTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        var availableSlots = new List<AvailableTimeSlotDto>();
        var templates = await _repository.GetTemplatesByShippingMethodAsync(request.ShippingMethodId, cancellationToken);
        
        var currentDate = request.StartDate;
        while (currentDate <= request.EndDate)
        {
            var dayOfWeek = (Domain.Enums.DayOfWeek)((int)currentDate.DayOfWeek);
            var dayTemplates = templates.Where(t => t.DayOfWeek == dayOfWeek && t.IsActive);
            
            foreach (var template in dayTemplates)
            {
                var availableCapacity = await _repository.GetAvailableCapacityAsync(template.Id, currentDate, cancellationToken);
                
                if (availableCapacity > 0)
                {
                    availableSlots.Add(new AvailableTimeSlotDto
                    {
                        TemplateId = template.Id,
                        Date = currentDate,
                        StartTime = template.StartTime,
                        EndTime = template.EndTime,
                        AvailableCapacity = availableCapacity
                    });
                }
            }
            
            currentDate = currentDate.AddDays(1);
        }

        return availableSlots;
    }
}
