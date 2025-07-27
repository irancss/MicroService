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
        private readonly ILogger<ClearActiveCartHandler> _logger;

        public ClearActiveCartHandler(IActiveCartRepository cartRepository, ILogger<ClearActiveCartHandler> logger)
        {
            _cartRepository = cartRepository;
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


            return new CartOperationResultDto(true, null, cart.ToDto());
        }
    }
}
