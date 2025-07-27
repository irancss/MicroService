using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.IntegrationEventHandlers;
using Cart.Application.Interfaces;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Mappers;

namespace Cart.Application.Handlers.Commands
{
    public class ClearActiveCartHandler : ICommandHandler<ClearActiveCartCommand, CartOperationResultDto>
    {
        private readonly IActiveCartRepository _cartRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ClearActiveCartHandler> _logger;

        public ClearActiveCartHandler(IActiveCartRepository cartRepository, IEventBus eventBus, ILogger<ClearActiveCartHandler> logger)
        {
            _cartRepository = cartRepository;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<CartOperationResultDto> Handle(ClearActiveCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
            if (cart is null)
            {
                // اگر سبد وجود ندارد، یک نتیجه موفق با سبد خالی برگردان
                return new CartOperationResultDto(true, null, CartDto.CreateEmpty(request.CartId, request.UserId));
            }

            cart.Clear();

            await _cartRepository.SaveAsync(cart, cancellationToken);

            _logger.LogInformation("Cart {CartId} for user {UserId} has been cleared.", request.CartId, request.UserId);

            // انتشار رویداد برای آزاد کردن موجودی تمام آیتم‌ها
            // ارسال یک سبد خالی به سرویس Inventory اطلاع می‌دهد که تمام رزروها باید لغو شوند.
            var integrationEvent = new ActiveCartUpdatedIntegrationEvent(
                cart.UserId, cart.Id, 0, 0, new List<CartItemDetails>()
            );
            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            return new CartOperationResultDto(true, null, cart.ToDto());
        }
    }
}
