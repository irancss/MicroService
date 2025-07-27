using BuildingBlocks.Abstractions;
using BuildingBlocks.Application.CQRS.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Cart.Domain.Entities;
using Cart.Domain.Events;

namespace Cart.Application.Handlers.Commands;

public class AddItemToNextPurchaseCartHandler : ICommandHandler<AddItemToNextPurchaseCommand, NextPurchaseCartDto>
    {
        private readonly INextPurchaseCartRepository _cartRepository;
        private readonly ICatalogClient _catalogClient;
        private readonly IInventoryClient _inventoryClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddItemToNextPurchaseCartHandler> _logger;

        public AddItemToNextPurchaseCartHandler(
            INextPurchaseCartRepository cartRepository,
            ICatalogClient catalogClient,
            IInventoryClient inventoryClient,
            IUnitOfWork unitOfWork,
            ILogger<AddItemToNextPurchaseCartHandler> logger)
        {
            _cartRepository = cartRepository;
            _catalogClient = catalogClient;
            _inventoryClient = inventoryClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<NextPurchaseCartDto> Handle(AddItemToNextPurchaseCommand request, CancellationToken cancellationToken)
        {
            var productInfo = await _catalogClient.GetProductInfoAsync(request.ProductId, cancellationToken);
            if (productInfo is null || !productInfo.IsActive)
                throw new ApplicationException("Product not found or is inactive.");

            var price = await _inventoryClient.GetCurrentPriceAsync(request.ProductId, cancellationToken);
            if (price is null)
                throw new ApplicationException("Could not retrieve product price.");

            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                       ?? NextPurchaseCart.Create(request.UserId);

            var newItem = NextPurchaseItem.Create(
                productInfo.Id,
                productInfo.Name,
                price.Value,
                productInfo.ImageUrl,
                request.VariantId
            );

            // این متد خودش رویداد دامنه را هم ایجاد می‌کند
            cart.AddItem(newItem);
            
            // Repsitory را برای ردیابی تغییرات فراخوانی می‌کنیم
            await _cartRepository.SaveAsync(cart, cancellationToken);

            // تغییرات را از طریق UnitOfWork به صورت تراکنشی ذخیره می‌کنیم
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Item {ProductId} added to next-purchase cart for user {UserId}", request.ProductId, request.UserId);

            // [اصلاح شد] ما خود موجودیت `cart` را که در حافظه داریم به DTO تبدیل می‌کنیم
            return cart.ToDto();
        }
        }
