using BuildingBlocks.Application.CQRS.Queries;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Cart.Application.Queries;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Queries
{
    public class GetNextPurchaseCartQueryHandler : IQueryHandler<GetNextPurchaseCartQuery, NextPurchaseCartDto?>
    {
        private readonly INextPurchaseCartRepository _repository;
        private readonly ILogger<GetNextPurchaseCartQueryHandler> _logger;

        public GetNextPurchaseCartQueryHandler(INextPurchaseCartRepository repository, ILogger<GetNextPurchaseCartQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<NextPurchaseCartDto> Handle(GetNextPurchaseCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetNextPurchaseCartQuery for UserId: {UserId}", request.UserId);

            var cart = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (cart is null)
            {

                // یا یک Exception پرتاب کنید یا یک سبد خالی برگردانید.
                // برگرداندن سبد خالی معمولاً تجربه کاربری بهتری است.
                return new NextPurchaseCartDto(request.UserId, 0, new List<NextPurchaseItemDto>());
            }
            return cart.ToDto();
        }
    }
}
