using BuildingBlocks.Application.CQRS.Queries;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Cart.Application.Queries;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Queries
{
    public class GetActiveCartQueryHandler : IQueryHandler<GetActiveCartQuery, CartDto?>
    {
        private readonly IActiveCartRepository _repository;
        private readonly ILogger<GetActiveCartQueryHandler> _logger;

        public GetActiveCartQueryHandler(IActiveCartRepository repository, ILogger<GetActiveCartQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CartDto?> Handle(GetActiveCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetActiveCartQuery for CartId: {CartId}", request.CartId);

            var cart = await _repository.GetByIdAsync(request.CartId, cancellationToken);

            // The ToDto() extension method, defined in Mappers, handles the conversion.
            return cart?.ToDto();
        }
    }
}
